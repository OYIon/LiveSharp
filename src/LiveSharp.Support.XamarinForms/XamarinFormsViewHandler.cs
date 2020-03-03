using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LiveSharp.Support.XamarinForms;
using Xamarin.Forms;

namespace LiveSharp
{
    public class XamarinFormsViewHandler : ILiveSharpUpdateHandler
    {
        private readonly WeakReference<object> _latestContentPage = new WeakReference<object>(null);
        private ILiveSharpRuntime _runtime;
        private string _pageHotReloadMethodName;
        private bool _missingHotReloadMethodReported = false;
        
        public void Initialize(ILiveSharpRuntime runtime)
        {
            _runtime = runtime;
            
            if (runtime.Config.TryGetValue("pageHotReloadMethod", out var methodName))
                _pageHotReloadMethodName = methodName;
            else
                _pageHotReloadMethodName = "Build";
        }
        
        public void HandleCall(object instance, string methodIdentifier, object[] args, Type[] argTypes)
        {
            if (instance is ContentPage && methodIdentifier.EndsWith(" " + _pageHotReloadMethodName + " "))
                _latestContentPage.SetTarget(instance);
            
            if (instance is ContentPage contentPage)
                _runtime.Inspector?.SetCurrentContext(contentPage);
        }
        
        public void HandleUpdate(Dictionary<string, IReadOnlyList<object>> updatedMethods)
        {
            var instances = updatedMethods.SelectMany(kvp => kvp.Value)
                .Where(i => i != null)
                .Distinct()
                .ToArray();

            Device.BeginInvokeOnMainThread(() => {
                var found = false;

                try {
                    foreach (var instance in instances)
                        if (instance is ContentPage && CallHotReloadMethod(instance))
                            found = true;

                    if (!found)
                        if (_latestContentPage.TryGetTarget(out var contentPage))
                            CallHotReloadMethod(contentPage);
                } catch (TargetInvocationException e) {
                    var inner = e.InnerException;
                        
                    while (inner is TargetInvocationException tie)
                        inner = tie.InnerException;

                    _runtime.Logger.LogError("Xamarin.Forms update handler failed", inner ?? e);
                }
            });
        }

        private bool CallHotReloadMethod(object instance)
        {
            if (TryCallingRuntimeMethod(instance))
                return true;

            var hotReloadMethod = instance.GetMethod(_pageHotReloadMethodName, true);
            if (hotReloadMethod == null) {
                ReportMissingHotReloadMethod(instance);
                return false;
            }
            
            hotReloadMethod.Invoke(instance, null);
            
            return true;
        }

        private bool TryCallingRuntimeMethod(object instance)
        {
            var args = new object[0];
            var runtimeCode = _runtime.GetUpdate(instance, _pageHotReloadMethodName, new Type[0], args);
            
            if (runtimeCode != null) {
                _runtime.ExecuteVoid(runtimeCode, instance, args);
                return true;
            }
            
            return false;
        }

        private void ReportMissingHotReloadMethod(object instance)
        {
            if (!_missingHotReloadMethodReported) {
                _runtime.Logger.LogWarning("Unable to find `" + _pageHotReloadMethodName + "` method on `" + instance?.GetType().FullName + "` to perform hot-reload");
                _missingHotReloadMethodReported = true;
            }
        }
    }
}