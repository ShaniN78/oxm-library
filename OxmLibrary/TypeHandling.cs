using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace OxmLibrary
{
    public static class TypeHandling
    {
        public static readonly Type MyType = typeof(ElementBase);
        private static readonly Type objectType = typeof(object);

        /// <summary>
        /// data types ladder, the higher - the more general classes
        /// The data types.
        /// </summary>
        public static readonly Dictionary<int, string> DataTypesLadder = new Dictionary<int, string>
                                                                             {
                                                                                 { 5, "string" },
                                                                                 { 4, "float" },
                                                                                 { 3, "long" },
                                                                                 { 2, "int" },
                                                                                 { 1, "bool" },
                                                                                 { 0, "DateTime" }
                                                                             };        
        public static readonly List<string> BasicDataTypes = DataTypesLadder.Select(a => a.Value).ToList();

        private static readonly SortedList<string, MethodInfo> parsers = new SortedList<string, MethodInfo>();
        private static readonly SortedList<string, object> defaults = new SortedList<string, object>();
        
        static TypeHandling()
        {
            //Add all base data types to the parsing methods cache
            AddType(typeof(int));
            AddType(typeof(uint));
            AddType(typeof(long));
            AddType(typeof(float));
            AddType(typeof(bool));
            AddType(typeof(double));
            AddType(typeof(DateTime));
            AddType(typeof(char));
        }

        public static bool IsKnownPrimiveType(string typeName)
        {
            return (BasicDataTypes.Contains(typeName, StringComparer.OrdinalIgnoreCase));
        }

        public static object DefaultValue(Type prop)
        {
            if (defaults.ContainsKey(prop.Name))
            {
                return defaults[prop.Name];
            }
            return null;
        }

        public static void AddType(Type prop)
        {
            if (!parsers.ContainsKey(prop.Name))
            {
                
                Type[] types = { typeof(string), prop.MakeByRefType() };
                var method = prop.GetMethod("TryParse", types);
                AddType(prop, method);
            }            
        }
        
        public static void AddType(Type prop, MethodInfo method)
        {
            if (!parsers.ContainsKey(prop.Name))
            {
                defaults.Add(prop.Name, prop.IsValueType ? Activator.CreateInstance(prop) : string.Empty);
                parsers.Add(prop.Name, method);
            }
        }

        public static object Parse(Type prop, string value)
        {            
            var args = new object[] { value, null };
            bool legal = (bool)parsers[prop.Name].Invoke(null, args);
            return args[1];
        }
        
        public static bool DerivesFromElementBase(Type type)
        {
            while (type.BaseType != objectType) 
            {
                type = type.BaseType;
                if (type == MyType)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// try to downgrade data type using the data types ladder <see>DataTypesLadder</see>
        /// </summary>
        /// <param name="p">
        /// The first type.
        /// </param>
        /// <param name="p_2">
        /// The second type.
        /// </param>
        /// <returns>
        /// The down grade.
        /// </returns>
        public static string DownGrade(string p, string p_2)
        {
            if (p == p_2)
                return p;
            int key1 = (from KeyValuePair<int, string> kvp1 in DataTypesLadder
                        where kvp1.Value == p
                        select kvp1.Key).SingleOrDefault();
            int key2 = (from KeyValuePair<int, string> kvp2 in DataTypesLadder
                        where kvp2.Value == p_2
                        select kvp2.Key).SingleOrDefault();
            string newtype = DataTypesLadder[Math.Max(key1, key2)];

            if (newtype != null)
                return newtype;
            return p;
        }

        public static Dictionary<string, string> GetListItems(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ApplicationException("GetListItems does not support non-enum types");
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (FieldInfo field in enumType.GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public))
            {
                string value = Enum.GetName(enumType, field.GetValue(null));
                string display = value;
                var valueAttribute = field.GetCustomAttributes(false).OfType<System.Xml.Serialization.XmlEnumAttribute>().FirstOrDefault();
                if (valueAttribute != null)
                    display = valueAttribute.Name;
                
                list.Add(value, display);
            }
            return list;
        }
    }
}
