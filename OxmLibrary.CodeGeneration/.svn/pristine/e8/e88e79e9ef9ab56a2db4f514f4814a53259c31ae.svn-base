namespace OxmLibrary.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    [Serializable]
    public class GenerationConfiguration : INotifyPropertyChanged, ICloneable
    {
        #region Fields

        private const string DEFAULTCOLLECTIONTYPE = "System.Collections.Generic.List";

        private bool addSerializeableAttribute;
        private PropertiesMode automaticProperties;
        private bool enableDataBinding;
        private bool setDefaultValuesInConstructor;
        private bool useCustomCollections;

        private GenerationAdditionalParametersCollection additionalParameters;


        private int indent;
        private string customCollectionType;

        private static readonly GenerationConfiguration defaultConfiguration;

        #endregion Fields

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        [Browsable(true)]
        [DisplayName("Additional Parameters")]
        [Description("Expand to edit")]        
        [XmlIgnore]
        public GenerationAdditionalParametersCollection AdditionalParameters
        {
            get { return additionalParameters; }
            set { additionalParameters = value; }
        }

        /// <summary>
        /// gets or sets a value indicating whether the custom collection name
        /// or an array.
        /// </summary>
        [Category("Collections")]
        [Browsable(true)]
        [DisplayName("Collection Type")]
        [Description("Name of Collection, Including Full TextWriter")]
        public string CustomCollectionType
        {
            get { return customCollectionType ?? string.Empty; }
            set { customCollectionType = value; }
        }

        [Browsable(false)]
        public string CollectionTypeForGeneration
        {
            get
            {
                return CustomCollectionType.Substring(CustomCollectionType.LastIndexOf('.') + 1);
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public CodeTemplateBase currentCodeGenerator
        {
            get;
            set;
        }

        [Browsable(false)]
        [XmlIgnore]
        [Editor("ComboBox", "CodeTemplateHandling")]
        public string Language
        {
            get
            {
                return currentCodeGenerator.DisplayName;
            }
            set
            {

                OnPropertyChanged("Language");
            }
        }


        /// <summary>
        /// gets or sets a value indicating whether the collections are custom collection
        /// or an array.
        /// </summary>
        [Category("Collections")]
        [Browsable(true)]
        [DisplayName("Use Collections")]
        [Description("If disabled, arrays will be used")]
        public bool UseCustomCollections
        {
            get { return useCustomCollections; }
            set
            {
                if (value && useCustomCollections != value)
                {
                    customCollectionType = string.IsNullOrEmpty(customCollectionType) ? DEFAULTCOLLECTIONTYPE : customCollectionType;
                }

                useCustomCollections = value;
                OnPropertyChanged("UseCustomerCollections");
            }
        }

        /// <summary>
        /// How many spaces per indentation
        /// </summary>
        [Category("Formatting")]
        [Browsable(true)]
        [Description("Amount of spaces per indentation level")]
        public int Indent
        {
            get { return indent; }
            set
            {
                indent = value;
                OnPropertyChanged("Indent");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to Add Serialize able Attribute to
        /// each generated class
        /// </summary>
        [Category("Behaviour")]
        [Browsable(true)]
        [DisplayName("Serializable Classes")]
        [Description("Should the classes be serializable?")]
        public bool AddSerializeableAttribute
        {
            get { return addSerializeableAttribute; }
            set
            {
                addSerializeableAttribute = value;
                OnPropertyChanged("AddSerializeableAttribute");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating wheter the properties should be full
        /// or automatic.
        /// </summary>
        [Category("Properties")]
        [Browsable(true)]
        public PropertiesMode AutomaticProperties
        {
            get { return automaticProperties; }
            set
            {
                if (value != automaticProperties)
                {
                    automaticProperties = value;
                    if (value == PropertiesMode.Automatic)
                        EnableDataBinding = false;
                    OnPropertyChanged("AutomaticProperties");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whetger the properties should be binding aware
        /// via INotfiftPropertyChanged Interface
        /// </summary>
        [Category("Properties")]
        [Browsable(true)]
        [DisplayName("Enable DataBinding")]
        [Description("Enables adding PropertyChanged call to the setter of the properties")]
        public bool EnableDataBinding
        {
            get { return enableDataBinding; }
            set
            {
                if (value != enableDataBinding)
                {
                    enableDataBinding = value;
                    if (value && AutomaticProperties == PropertiesMode.Automatic)
                        AutomaticProperties = PropertiesMode.Full;
                    OnPropertyChanged("EnableDataBinding");
                }
            }
        }

        private bool useTargetNamespace;
        /// <summary>
        /// gets or sets a value indicating whether the XSD parser will index
        /// the XSD files before parsing. Useful for big XSD files.
        /// </summary>
        [Category("Behaviour")]
        [Browsable(true)]
        [DisplayName("Data Contract Ready")]
        [Description("To comply with WCF message contract it will add xmlns attribute to all XSD with TargetNamespace attribute")]
        public bool UseTargetNamespace
        {
            get
            {
                return useTargetNamespace;
            }
            set
            {
                useTargetNamespace = value;
                OnPropertyChanged("UseTargetNamespace");
            }
        }


        private bool performIndexation;

        /// <summary>
        /// gets or sets a value indicating whether the XSD parser will index
        /// the XSD files before parsing. Useful for big XSD files.
        /// </summary>
        [Category("Behaviour")]
        [Browsable(true)]
        [DisplayName("Perform XSD indexing")]
        [Description("When set XSD parser will index XSD files before parsing. Useful for big XSD files.")]
        public bool PerformIndexation
        {
            get
            {
                return performIndexation;
            }
            set
            {
                performIndexation = value;
                OnPropertyChanged("PerformIndexation");
            }
        }

        private string currentNamespace;

        /// <summary>
        /// gets or sets the name space for the generated classes.
        /// </summary>
        [Category("Code")]
        [Browsable(true)]
        [DisplayName("Namespace")]
        [Description("Sets the name space for the generated classes.")]
        public string CurrentNamespace
        {
            get
            {
                return currentNamespace;
            }
            set
            {
                currentNamespace = value;
                OnPropertyChanged("CurrentNamespace");
            }
        }

        private int maxClassesInFile;

        /// <summary>
        /// gets or sets a value indicating the maximum number of classes in each file
        /// when creating multiple files.
        /// </summary>
        [Category("Code")]
        [Browsable(true)]
        [DisplayName("Maximum Classes in file:")]
        [Description("A value indicating the maximum number of classes in each file when creating multiple files.")]
        public int MaxClassesInFile
        {
            get
            {
                return maxClassesInFile;
            }
            set
            {
                maxClassesInFile = value;
                OnPropertyChanged("MaxClassesInFile");
            }
        }

        private string projectName;

        /// <summary>
        /// gets or sets the name of the project
        /// Will also set the name of the factory class
        /// </summary>
        [Category("Code")]
        [Browsable(true)]
        [DisplayName("Project Name")]
        [Description("The name of the project. Will also set the name of the factory class")]
        public string ProjectName
        {
            get
            {
                return projectName;
            }
            set
            {
                projectName = value;
                OnPropertyChanged("ProjectName");
            }
        }


        /// <summary>
        /// gets or sets a value indicating whether the constructor should
        /// assign default values (if they are available)
        /// </summary>
        [Category("Behaviour")]
        [Browsable(true)]
        [DisplayName("Default Values")]
        [Description("When set will Assign default values in the constructor")]
        public bool SetDefaultValuesInConstructor
        {
            get { return setDefaultValuesInConstructor; }
            set
            {
                setDefaultValuesInConstructor = value;
                OnPropertyChanged("SetDefaultValuesInConstructor");
            }
        }

        /// <summary>
        /// Default Configuration.
        /// </summary>
        public static GenerationConfiguration DefaultConfiguration
        {
            get { return (GenerationConfiguration)defaultConfiguration.Clone(); }
        }

        #endregion Properties

        static GenerationConfiguration()
        {
            defaultConfiguration = new GenerationConfiguration();
            defaultConfiguration.addSerializeableAttribute = true;
            defaultConfiguration.automaticProperties = PropertiesMode.Automatic;
            defaultConfiguration.enableDataBinding = false;
            defaultConfiguration.setDefaultValuesInConstructor = true;
            defaultConfiguration.Indent = 4;
            defaultConfiguration.UseCustomCollections = false;
            defaultConfiguration.currentNamespace = "DDos.OxmGenerator";
            defaultConfiguration.projectName = "New1";
            defaultConfiguration.maxClassesInFile = 30;
        }

        public GenerationConfiguration()
        {
            additionalParameters = new GenerationAdditionalParametersCollection();
        }

        #region Methods

        public void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }

        public object Clone()
        {
            GenerationConfiguration config = new GenerationConfiguration();
            config.AutomaticProperties = automaticProperties;
            config.EnableDataBinding = enableDataBinding;
            config.SetDefaultValuesInConstructor = setDefaultValuesInConstructor;
            config.AddSerializeableAttribute = addSerializeableAttribute;
            config.useCustomCollections = useCustomCollections;
            config.customCollectionType = customCollectionType;
            config.indent = indent;
            config.currentNamespace = currentNamespace;
            config.projectName = projectName;
            config.maxClassesInFile = maxClassesInFile;
            return config;
        }

        public bool AddAdditionalParameter<T>(string key, T initialValue, string displayName)
        {
            return additionalParameters.Add(key, initialValue, displayName);
        }

        public T GetAdditionalParameter<T>(string key)
        {
            return additionalParameters.Get<T>(key);
        }

        public List<string> GetAdditionalRequiredNamespaces()
        {
            List<string> TextWriters = new List<string>();
            if (enableDataBinding)
                TextWriters.Add("System.ComponentModel");
            if (useTargetNamespace)
                TextWriters.Add("System.Runtime.Serialization");
            if (useCustomCollections)
            {
                var customCollectionTextWriter = customCollectionType.Substring(0, customCollectionType.LastIndexOf('.'));
                TextWriters.Add(customCollectionTextWriter);
            }
            return TextWriters;
        }
        #endregion
    }
}