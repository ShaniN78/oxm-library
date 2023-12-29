using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxmLibrary.GUI.Properties;

using System.Reflection;
using OxmLibrary.CodeGeneration;

namespace OxmLibrary.GUI
{
    public class PluginsHandler
    {
        public static PluginsHandler Instance { get; private set; }
        public List<PluginLocation> PlugIns { get; private set; }

        static PluginsHandler()
        {
            Instance = new PluginsHandler();
        }
        
        private PluginsHandler()
        {
            Settings.Default.Reload();
            if (Settings.Default.InstalledPlugIns == null)
            {
                Settings.Default.InstalledPlugIns = new List<PluginLocation>();
                Settings.Default.Save();
            }
            PlugIns = new List<PluginLocation>(Settings.Default.InstalledPlugIns.OfType<PluginLocation>());
        }

        public void Add(PluginLocation plug)
        {
            PlugIns.Add(plug);
            Settings.Default.InstalledPlugIns.Add(plug);
            Settings.Default.Save();
        }

        public bool Add(string fileName)
        {
            var getPluginLocation = getTypeFromDll(fileName);
            if (getPluginLocation != null)
                Add(getPluginLocation);
            return getPluginLocation != null;
        }

        public static PluginLocation getTypeFromDll(string dll)
        {
            Assembly asmon = Assembly.LoadFrom(dll);
            Module[] mods = asmon.GetLoadedModules(false);
            Type[] types = mods[0].GetTypes();
            foreach (Type type in types)
            {
                Type isIPlugin = type.GetInterface("IOxmGeneratorPlugin", true); //Check if it implelements the interface
                if (isIPlugin != null)
                {
                    var instance = Activator.CreateInstance(type) as IOxmGeneratorPlugin;
                    //ConstructorInfo[] cons = type.GetConstructors(); //Get constructors list
                    //plugIn = (IPlugIn)cons[0].Invoke(new object[] { "Hello" }); //First constructor should accept object
                    PluginLocation location = new PluginLocation();
                    location.AssemblyName = asmon.FullName;
                    location.TypeName = type.AssemblyQualifiedName;
                    location.FileLocation = dll;
                    location.FileSupport = instance.RequiresExternalFile;
                    location.PlugInName = instance.PluginName;
                    return location;
                }
            }
            return null;
        }

        public void Delete(PluginLocation plug)
        {
            PlugIns.Remove(plug);
            Settings.Default.InstalledPlugIns.Remove(plug);
            Settings.Default.Save();
        }
    }
}
