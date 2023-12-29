using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Xml.Linq;

namespace OxmLibrary
{
    internal static class ElementsParseCaching
    {
        private static Dictionary<string, Dictionary<string, string>> elementNameCache;
        //private static Hashtable elementNameCache;
        private static Dictionary<string, ElementTypeMatch> elementToTypeCache;
        //private static Hashtable elementNameCache;

        static ElementsParseCaching()
        {
            elementNameCache = new Dictionary<string, Dictionary<string, string>>();
            elementToTypeCache = new Dictionary<string, ElementTypeMatch>();
        }

        internal static Dictionary<string, string> GetDictionary(Type t, string Name)
        {
            if (!elementNameCache.ContainsKey(Name))
            {
                var result = (from typ in t.GetProperties()
                              where typ.GetIndexParameters().Length == 0
                              let custom = typ.GetCustomAttributes(typeof(OxmOverrideElementNameAttribute), false).OfType<OxmOverrideElementNameAttribute>().FirstOrDefault()
                              let name = custom != null ? custom.ElementName : typ.Name
                              select new { Name = typ.Name, Override = name }).ToDictionary(a => a.Name, b => b.Override);
                elementNameCache[Name] = result;
                return result;
            }
            return elementNameCache[Name];
        }

        public static ElementTypeMatch GetParseDictionary(Type elem, string Name)
        {
            ElementTypeMatch result;
            var local = elem.Namespace + Name;
            if (elementToTypeCache.TryGetValue(local, out result))
            {
                return result;
            }
            var names = GetDictionary(elem, local);
            var list = (from typ in elem.GetProperties()
                                           where typ.Name != "ElementName" && typ.GetGetMethod().GetParameters().Length == 0
                                           select new ElementTypeMapping(names[typ.Name], typ.PropertyType, typ)).ToList();
            //var dictionary = new Dictionary<string, ElementTypeMapping>();
            //list.ForEach(a => 
            //    {
            //        if (!dictionary.ContainsKey(a.IName))
            //            dictionary.Add(a.IName, a);
            //    });
            result = new ElementTypeMatch(list);

            elementToTypeCache[local] = result;
            return result;
        }
    }
}
