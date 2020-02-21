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

        public void Initialize(ILiveSharpRuntime runtime)
        {
            _runtime = runtime;
        }
        
        public void HandleCall(object instance, string methodIdentifier, object[] args, Type[] argTypes)
        {
            if (instance is ContentPage && methodIdentifier.EndsWith(" Build "))
                _latestContentPage.SetTarget(instance);
        }
        
        public void HandleUpdate(Dictionary<string, IReadOnlyList<object>> updatedMethods)
        {
            var instances = updatedMethods.SelectMany(kvp => kvp.Value)
                .Where(i => i != null)
                .Distinct()
                .ToArray();

            Device.BeginInvokeOnMainThread(() => {
                var found = false;
                var updatedContexts = new HashSet<Type>();

                try {
                    foreach (var instance in instances) {
                        if (CallBuildMethod(instance))
                            found = true;
                    }
                    
                    if (!found) {
                        if (_latestContentPage.TryGetTarget(out var contentPage))
                            CallBuildMethod(contentPage);
                    }
                } catch (TargetInvocationException e) {
                    var inner = e.InnerException;
                        
                    while (inner is TargetInvocationException tie)
                        inner = tie.InnerException;

                    _runtime.Logger.LogError("Xamarin.Forms update handler failed", inner ?? e);
                }
            });
        }

        private static bool CallBuildMethod(object instance)
        {
            var buildMethod = instance.GetMethod("Build", true);

            if (buildMethod == null) 
                return false;
            
            buildMethod.Invoke(instance, null);
            
            return true;
        }
    }
}