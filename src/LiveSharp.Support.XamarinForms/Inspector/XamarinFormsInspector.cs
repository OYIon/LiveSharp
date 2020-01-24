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

        private ActionDisposable _currentPageSubscription;

        private INotifyPropertyChanged _currentBindingContext;
        private ActionDisposable _currentBindingContextDisposable;

        private readonly List<ActionDisposable> _mainPageDisposables = new List<ActionDisposable>();

        public void StartInspector()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Application.Current.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == "MainPage")
                        OnAppMainPageChanged(Application.Current.MainPage);
                };

                OnAppMainPageChanged(Application.Current.MainPage);
            });            
        }

        private void OnAppMainPageChanged(Page mainPage)
        {
            foreach (var disposable in _mainPageDisposables)            
                disposable.Dispose();

            _mainPageDisposables.Clear();
            
            if (mainPage is NavigationPage navigation)
            {
                navigation.Pushed += navigationHandler;
                navigation.Popped += navigationHandler;

                _mainPageDisposables.Add(new ActionDisposable(() => navigation.Pushed -= navigationHandler));
                _mainPageDisposables.Add(new ActionDisposable(() => navigation.Popped -= navigationHandler));

                OnAppMainPageChanged(navigation.CurrentPage);

                void navigationHandler(object sender, NavigationEventArgs args) {
                    OnAppMainPageChanged(navigation.CurrentPage);
                }
            }
            else if (mainPage is MasterDetailPage masterDetail)
            {   
                OnAppMainPageChanged(masterDetail.Detail);
            }
            else
            {
                OnCurrentPageChanged(mainPage);
            }
        }

        private void OnCurrentPageChanged(Page page)
        {
            _currentPageSubscription?.Dispose();

            if (page == null)
                return;

            page.BindingContextChanged += bindingContextChanged;

            _currentPageSubscription = new ActionDisposable(() => page.BindingContextChanged -= bindingContextChanged);

            OnCurrentPageBindingContextChanged(page.BindingContext);

            void bindingContextChanged(object sender, EventArgs e)
            {
                OnCurrentPageBindingContextChanged(page.BindingContext);
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

                    inpc.PropertyChanged += InpcPropertyChanged;

                    _currentBindingContext = inpc;
                    _currentBindingContextDisposable = new ActionDisposable(() => inpc.PropertyChanged -= InpcPropertyChanged);

                    void InpcPropertyChanged(object sender, PropertyChangedEventArgs e)
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
