using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

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
            return $"<p class='member'><span class='member-name'>{PropertyInfo.Name}</span><span class='member-value'>{Value}</span></p>";
        }

        internal void UpdateValue(object instance)
        {
            Value = FormatData(PropertyInfo.GetValue(instance));
        }
        
        static string FormatData(object data, bool encode = true)
        {
            if (data == null)
                data = "null";
            else if (data is string)
                data = "\"" + data + "\"";
            else if (data is char)
                data = "'" + data + "'";
            else if (data is IList list)
                data = formatList(list);
            else if (data is IDictionary dict)
                data = formatDictionary(dict);
            else if (data is IEnumerable)
                data = data.GetType().GetTypeName();
            else if (data is Task)
            {
                var type = data.GetType();
                if (type.GenericTypeArguments.Length > 0)
                    data = "Task<" + string.Join(", ", type.GenericTypeArguments.Select(t => t.GetTypeName())) + ">";
                else
                    data = "Task";
            } else {
                var type = data.GetType();
            
                if (type.IsPrimitive) {
                    data = data.ToString();
                } else if (type.GetMethod("ToString", BindingFlags.Instance | BindingFlags.DeclaredOnly) != null) {
                    data = data.ToString();
                } else {
                    data = data.ToString();
                    //data = serializeObject(data);
                }
            }

            if (encode)
                return WebUtility.HtmlEncode(data.ToString());
            else
                return data.ToString();
                    
            string formatList(IList array)
            {
                var serializedObjects = array.OfType<object>().Take(10).Select((o, i) => i + ": " + FormatData(o, false));
                return "Count = " + array.Count + Environment.NewLine + string.Join(Environment.NewLine, serializedObjects);
            }
            
            string formatDictionary(IDictionary dict)
            {
                var serializedObjects = dict.OfType<DictionaryEntry>().Take(10).Select(e => FormatData(e.Key, false) + ": " + FormatData(e.Value, false));
                return "Count = " + dict.Count + Environment.NewLine + string.Join(Environment.NewLine, serializedObjects);
            }
            
            string serializeObject(object obj)
            {
                try {
                    var objectType = obj.GetType();
                    var properties = objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    var propertyValues = properties.Select(p => p.Name + ": " + FormatData(p.GetValue(obj), false));
                    return "{ " + string.Join(", ", propertyValues) + " }";
                }
                catch (Exception e) {
                    return "<Exception: " + e.Message + ">";
                }
            }
        }
    }
}
