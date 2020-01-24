using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace LiveSharp.Support.XamarinForms
{
    class InstanceInspector
    {
        public List<PropertyInspector> Properties { get; } = new List<PropertyInspector>();
        public List<MethodInspector> Methods { get; } = new List<MethodInspector>();

        private readonly Type _type;

        public InstanceInspector(INotifyPropertyChanged inpc)
        {
            _type = inpc.GetType();

            var properties = GetAllProperties(inpc);

            Properties.AddRange(properties.Select(pi => new PropertyInspector(pi, inpc)));
        }

        public string Serialize()
        {
            return 
$@"
<Instance TypeName=""{_type.FullName}"">
    <Properties>
        {string.Join("\n", Properties.Select(p => p.Serialize()))}
    </Properties>
</Instance>
";
        }

        public static PropertyInfo[] GetAllProperties(INotifyPropertyChanged inpc)
        {
            return inpc is IReflectableType reflectable
                    ? reflectable.GetTypeInfo().GetProperties()
                    : inpc.GetType().GetProperties();
        }
    }
}
