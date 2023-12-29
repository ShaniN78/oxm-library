﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxmLibrary.CodeGeneration;

using System.Reflection;

namespace OxmLibrary.GUI
{
    [Serializable]
    public class PluginLocation
    {
        public string AssemblyName { get; set; }

        public string TypeName { get; set; }

        public string PlugInName { get; set; }

        public string FileLocation { get; set; }

        public PluginFileFormatSupport FileSupport { get; set; }


        public Type GetPlugIn()
        {
            if (!System.IO.File.Exists(FileLocation))
            {
                throw new System.IO.FileNotFoundException("Plugin no longer exists");
            }
            Assembly asmon = Assembly.LoadFrom(FileLocation);

            var type = asmon.GetType(this.TypeName.Split(',')[0].Trim(), false);// Type.GetType(this.TypeName, false);
            
            Type isIPlugin = type.GetInterface("IOxmGeneratorPlugin", true); //Check if it implelements the interface
            if (isIPlugin!= null)
            {
                return type;
            }
            return null;
        }
    }
}
