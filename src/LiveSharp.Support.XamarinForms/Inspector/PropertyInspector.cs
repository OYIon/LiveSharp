using System.Reflection;

namespace LiveSharp.Support.XamarinForms
{
    public class PropertyInspector
    {
        public PropertyInfo PropertyInfo { get; }
        public string Value { get; private set; }

        public PropertyInspector(PropertyInfo pi, object instance)
        {
            PropertyInfo = pi;
            UpdateValue(instance);
        }

        public string Serialize()
        {
            return $"<Property Name=\"{PropertyInfo.Name}\">{Value}</Property>";
        }

        internal void UpdateValue(object instance)
        {
            Value = SerializeValue(PropertyInfo.GetValue(instance));
        }

        private string SerializeValue(object val)
        {
            if (val == null)
                return "<Null />";

            if (val is string)
                return $"\"{val}\"";

            return val.ToString();
        }
    }
}
