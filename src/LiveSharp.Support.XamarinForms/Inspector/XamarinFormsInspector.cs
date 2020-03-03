using LiveSharp.Runtime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace LiveSharp.Support.XamarinForms
{
    public class XamarinFormsInspector : ILiveSharpInspector
    {
        public event EventHandler<string> SerializedInstanceUpdate;

        private INotifyPropertyChanged _currentBindingContext;
        private ActionDisposable _currentBindingContextDisposable;
        private ActionDisposable _currentPageSubscription;

        public void StartInspector()
        {
           
        }
        
        private void OnCurrentPageChanged(Page page)
        {
            _currentPageSubscription?.Dispose();

            if (page == null)
                return;

            page.BindingContextChanged += bindingContextChanged;

            _currentPageSubscription = new ActionDisposable(() => page.BindingContextChanged -= bindingContextChanged);

            object bindingContext = null;
            try {
                bindingContext = page.BindingContext;
            } catch {
                // Page._properties might not be initialized yet, so BindingContext throws NRE 
            }

            if (bindingContext != null)
                OnCurrentPageBindingContextChanged(bindingContext);
            
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

                        SendInstanceUpdate(instanceInspector.Serialize());
                    }
                    
                    SendInstanceUpdate(instanceInspector.Serialize());
                }
            }
        }

        private void SendInstanceUpdate(string xml)
        {
            xml = $"<Inspector>{xml}</Inspector>";
            SerializedInstanceUpdate?.Invoke(this, xml);
        }
    }
}
