using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using System.Collections;

namespace OxmLibrary
{
    public class ElementTypeMapping
    {
        public string NativeName { get; set; }

        public string IName { get; set; }

        public Type DataType { get; set; }

        public PropertyInfo Setter { get; set; }

        public bool IsCollection { get; set; }

        public bool IsGenericCollection { get; set; }

        public bool IsAttribute { get; set; }

        public bool IsElement { get; set; }

        public bool IsOxmType { get; set; }

        public ElementCollectionAccessor InnerType { get; set; }

        public MethodInfo SetMethod { get; set; }

        public MethodInfo GetMethod { get; set; }

        public Type ParseType { get; set; }

        public bool IsParseTypeString { get; set; }

        public int EnumCount { get; private set; }

        public Dictionary<string, string> EnumValues { get; private set; }

        public string EnumValue(string value)
        {
            return EnumValues[value];
        }

        public ElementTypeMapping(ElementTypeMapping mapping)
        {
            this.DataType = mapping.DataType;
            this.GetMethod = mapping.GetMethod;
            this.IName = mapping.IName;
            this.InnerType = mapping.InnerType;

            this.IsAttribute = mapping.IsAttribute;
            this.IsElement = mapping.IsElement;
            this.IsCollection = mapping.IsCollection;
            this.IsGenericCollection = mapping.IsGenericCollection;
            this.IsOxmType = mapping.IsOxmType;
            this.IsParseTypeString = mapping.IsParseTypeString;

            this.NativeName = mapping.NativeName;
            this.ParseType = mapping.ParseType;
            this.SetMethod = mapping.SetMethod;
            this.Setter = mapping.Setter;

            if (this.ParseType.IsEnum)
            {
                EnumValues = TypeHandling.GetListItems(this.ParseType);
                EnumCount = EnumValues.Count;
            }

        }

        public ElementTypeMapping(string IName, Type DataType, PropertyInfo Setter)
        {
            this.NativeName = Setter.Name;
            this.IName = IName;
            this.Setter = Setter;

            ParseType = DataType.IsGenericType
                    ? (DataType.GetGenericArguments()[0])
                    : (!DataType.HasElementType ? DataType : DataType.GetElementType());

            if (ParseType.IsEnum)
            {
                EnumValues = TypeHandling.GetListItems(this.ParseType);
                EnumCount = EnumValues.Count;
            }

            this.DataType = DataType;
            SetMethod = Setter.GetSetMethod();
            GetMethod = Setter.GetGetMethod();

            IsParseTypeString = (ParseType == typeof(string));
            IsOxmType = !ParseType.FullName.StartsWith("System");
            IsGenericCollection = DataType.IsGenericType;
            IsCollection = DataType.IsArray || DataType.IsGenericType;
            IsAttribute = Setter.GetCustomAttributes(typeof(OxmXmlAttributeAttribute), false).Any();
            IsElement = !Setter.Name.EndsWith("InnerText") && !IsAttribute;

            if (IsGenericCollection)
                InnerType = new ElementCollectionAccessor(Setter);
        }

        public object GetValue(object from)
        {
            return GetMethod.Invoke(from, null);
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", NativeName, DataType.Name);
        }
    }

    public class ElementTypeMatch : List<ElementTypeMapping>
    {
        public bool ContainsAttributes { get; private set; }

        public int ElementsCount { get; private set; }

        public ElementTypeMatch()
            : base()
        {
        }

        public ElementTypeMatch(IEnumerable mappingList)
            : this((from ElementTypeMapping i in mappingList select i).ToList())
        {
        }

        public ElementTypeMatch(List<ElementTypeMapping> mappings)
            : base(mappings)
        {
            ContainsAttributes = mappings.Any(map => map.IsAttribute);
            ElementsCount = mappings.Count(a => a.IsElement);
        }

        public new IEnumerable<ElementTypeMapping> FindAll(Predicate<ElementTypeMapping> CheckFor)
        {
            foreach (var mapping in this)
            {
                if (CheckFor(mapping))
                    yield return mapping;
            }
        }

        public ElementTypeMapping this[string indexer]
        {
            get
            {
                var res = this.FirstOrDefault(a => a.NativeName == indexer);
                return res != null ? res : this.FirstOrDefault(a => a.IName == indexer);
            }
        }

        public bool TryGetValue(string indexer, out ElementTypeMapping result)
        {
            result = this[indexer];
            return result != null;
        }


        public IEnumerable<ElementWritingWrapper> FindAllWrappers(Predicate<ElementWritingWrapper> CheckFor)
        {
            foreach (var mapping in this.OfType<ElementWritingWrapper>())
            {
                if (CheckFor(mapping))
                    yield return mapping;
            }
        }
    }
}
