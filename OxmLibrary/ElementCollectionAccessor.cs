namespace OxmLibrary
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public class ElementCollectionAccessor
    {
        #region Fields

        private IList getCollection;
        private MethodInfo getter;
        private bool IsIList;
        private object previousObject;
        private MethodInfo setter;

        #endregion Fields

        #region Constructors

        public ElementCollectionAccessor(PropertyInfo GenericCollectionProperty)
        {
            var types = GenericCollectionProperty.PropertyType.GetGenericArguments();
            CollectionType = types[0];
            IsOxmType = !CollectionType.FullName.StartsWith("System");
            CollectionTypeName = IsOxmType ? types[0].Name.Substring(0, types[0].Name.Length - 3) : types[0].Name;

            DataType = GenericCollectionProperty.PropertyType;
            AddMethod = GenericCollectionProperty.PropertyType.GetMethod("Add", types);
            getter = GenericCollectionProperty.GetGetMethod();
            setter = GenericCollectionProperty.GetSetMethod();
            IsIList = typeof(IList).IsAssignableFrom(GenericCollectionProperty.PropertyType);
        }

        #endregion Constructors

        #region Properties

        public MethodInfo AddMethod
        {
            get; set;
        }

        public Type CollectionType
        {
            get; set;
        }

        public string CollectionTypeName
        {
            get; set;
        }

        public bool IsOxmType
        {
            get; set;
        }

        private Type DataType
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public void AddToCollection(ElementBase obj, object item, object collection)
        {
            if (IsIList)
            {
                ((IList)collection).Add(item);
            }
            else
            {
                AddMethod.Invoke(obj, new object[] { item });
            }
        }

        public void AddToCollection(ElementBase obj, object item)
        {
            if (IsIList)
            {
                if (previousObject != obj)
                {
                    getCollection = (IList)GetCollection(obj);
                    previousObject = obj;
                }
                if (getCollection == null)
                {
                    getCollection = (IList)Activator.CreateInstance(DataType);
                    SetCollection(obj, getCollection);
                }
                getCollection.Add(item);
            }
            else
            {
                var getCollection = (ICollection)GetCollection(obj);
                if (getCollection == null)
                {
                    getCollection = (ICollection)Activator.CreateInstance(DataType);
                    SetCollection(obj, getCollection);
                }
                AddMethod.Invoke(obj, new object[] { item });
            }
        }

        internal object GetCollection(ElementBase obj)
        {
            return getter.Invoke(obj, null);
        }

        internal void SetCollection(ElementBase elementBase, object collection)
        {
            setter.Invoke(elementBase, new object[] { collection });
        }

        #endregion Methods
    }
}