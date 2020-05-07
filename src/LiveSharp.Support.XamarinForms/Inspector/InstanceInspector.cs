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
<div class='inspector-instance'>
    <div class='instance-type'>{_type.FullName}</div>
    {string.Join("\n", Properties.Select(p => p.Serialize()))}
</div>
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
