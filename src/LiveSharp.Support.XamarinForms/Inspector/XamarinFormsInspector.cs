using LiveSharp.Runtime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace LiveSharp.Support.XamarinForms
{
    public class XamarinFormsInspector : ILiveSharpInspector
    {
        private int _instanceIds;
        public event EventHandler<string> SerializedInstanceUpdate;

        private INotifyPropertyChanged _currentBindingContext;
        private ActionDisposable _currentBindingContextDisposable;
        private ActionDisposable _currentPageSubscription;
        public ILiveSharpRuntime Runtime { get; set; }


        public void Render()
        {
            var currentBindingContext = _currentBindingContext;
            if (currentBindingContext != null) {
                var inspector = new InstanceInspector(currentBindingContext);
                var key = currentBindingContext.GetType().FullName;
                SendInstanceUpdate(key, inspector.Serialize());
            }
        }
        
        private void OnCurrentPageChanged(Page page)
        {
            _currentPageSubscription?.Dispose();

            if (page == null)
                return;

            page.BindingContextChanged += bindingContextChanged;

            _currentPageSubscription = new ActionDisposable(() => page.BindingContextChanged -= bindingContextChanged);

            Device.BeginInvokeOnMainThread(() =>
            {
                object bindingContext = null;
                try {
                    bindingContext = page.BindingContext;
                } catch {
                    // Page._properties might not be initialized yet, so BindingContext throws NRE 
                }

                if (bindingContext != null)
                    OnCurrentPageBindingContextChanged(bindingContext);
            });
            
            void bindingContextChanged(object sender, EventArgs e)
            {
                OnCurrentPageBindingContextChanged(page.BindingContext);
            }
        }
        
        
        public void SetCurrentContext(object context)
        {
            if (context is ContentPage contentPage) {
                OnCurrentPageChanged(contentPage);
            }
        }
        
        private void OnCurrentPageBindingContextChanged(object bindingContext)
        {
            if (bindingContext is INotifyPropertyChanged inpc)
            {
                if (_currentBindingContext != inpc)
                {
                    _currentBindingContextDisposable?.Dispose();

                    var instanceInspector = new InstanceInspector(inpc);

                    inpc.PropertyChanged += inpcPropertyChanged;

                    _currentBindingContext = inpc;
                    _currentBindingContextDisposable = new ActionDisposable(() => inpc.PropertyChanged -= inpcPropertyChanged);

                    void inpcPropertyChanged(object sender, PropertyChangedEventArgs e)
                    {
                        var existingInspector = instanceInspector.Properties.FirstOrDefault(pi => pi.PropertyInfo.Name == e.PropertyName);                        

                        if (existingInspector != null)
                        {
                            existingInspector.UpdateValue(inpc);
                        }
                        else
                        {
                            // If property was added without our knowledge (dynamic stuff)
                            var properties = InstanceInspector.GetAllProperties(inpc);
                            var property = properties.FirstOrDefault(p => p.Name == e.PropertyName);

                            if (property != null)
                                instanceInspector.Properties.Add(new PropertyInspector(property, inpc));
                        }

                        SendInstanceUpdate(instanceInspector.Key,instanceInspector.Serialize());
                    }
                    
                    SendInstanceUpdate(instanceInspector.Key,instanceInspector.Serialize());
                }
            }
        }

        private void SendInstanceUpdate(string key, string result)
        {
            // We can't list all instances like with Blazor support which targets .netcoreapp 3.1 
            //var newId = _instanceIds++;
            
            Runtime.SendBroadcast("-1 " + key + " " + result, 70, 6);
        }
    }
}
