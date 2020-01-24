using System;

namespace LiveSharp.Support.XamarinForms
{
    class ActionDisposable : IDisposable
    {
        private readonly Action _action;

        public ActionDisposable(Action action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public void Dispose()
        {
            _action?.Invoke();
        }
    }
}
