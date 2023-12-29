using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxmLibrary;
using System.IO;

namespace OxmLibrary.CodeGeneration
{
    public class CodeTemplateWrapper
    {
        private CodeTemplateBase currentCodeGenerator;
        private OxmGenerator3 generator;
        private GenerationConfiguration Config;

        public CodeTemplateWrapper(CodeTemplateBase template, OxmGenerator3 generator)
        {
            this.currentCodeGenerator = template;
            this.generator = generator;
            this.Config = generator.Config;
        }

        /// <summary>
        /// The add factory layer.
        /// </summary>
        /// <param name="theFile">
        /// The the file.
        /// </param>        
        private void AddFactoryLayer(TextWriter theFile, CodeTemplateBase currentCodeGenerator)
        {            
            TextHelper.FactoryName = Config.ProjectName + "Factory";
            currentCodeGenerator.WriteFactoryLayer(theFile, Config.AddSerializeableAttribute, Config.ProjectName, generator.ClassDetails);
        }

        /// <summary>
        /// Write file to a string builder. Builds the factory layer and the classes.
        /// </summary>
        /// <param name="theFile">
        /// The the file.
        /// </param>
        /// <param name="count">
        /// The count of classes in the file.
        /// </param>
        /// <param name="localName">
        /// The local name of the element
        /// </param>
        /// <param name="path">
        /// The path to write to.
        /// </param>
        private void WriteFile(TextWriter theFile, TextWriter newFile, int count)//, string localName, string path)
        {
            string[] splitup = theFile.ToString().Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            
            int depth = 0;
            for (int i = 0; i < splitup.Length; i++)
            {
                if (currentCodeGenerator.DecreaseIndentBeforeWriting(splitup[i]))
                {
                    depth--;
                }

                var line = splitup[i];
                if (depth > 0)
                    line = line.PadLeft(splitup[i].Length + (depth * Config.Indent));
                newFile.WriteLine(line);

                if (currentCodeGenerator.DecreaseIndentAfterWriting(splitup[i]))
                {
                    depth--;
                }


                if (currentCodeGenerator.CheckForEmptyLine(splitup[i], i < splitup.Length - 1 ? splitup[i + 1] : string.Empty))
                {
                    newFile.WriteLine(" ");
                }

                if (currentCodeGenerator.IncreaseIndentAfterWriting(splitup[i]))
                {
                    depth++;
                }
            }            
        }

        /// <summary>
        /// The create classes cs method. Adds the classes to a code file.
        /// </summary>
        /// <param name="theFile">
        /// The the file.
        /// </param>
        /// <param name="StartAt">
        /// The start at.
        /// </param>
        /// <returns>
        /// The create classes cs.
        /// </returns>
        private int CreateClasses(TextWriter theFile, int StartAt)
        {
            string[] allKeys = generator.classes.Keys.ToArray();
            int temp = StartAt;
            for (; StartAt < generator.classes.Count; StartAt++)
            {
                if (StartAt - temp > Config.MaxClassesInFile)
                    break;
                if (Config.UseTargetNamespace)
                    theFile.Write(currentCodeGenerator.GetAttribute("DataContract"));
                if (Config.AddSerializeableAttribute)
                    theFile.Write(currentCodeGenerator.GetAttribute("Serializable"));
                theFile.Write(generator.classes[allKeys[StartAt]]);
            }

            return StartAt;
        }

        /// <summary>
        /// The create enums method
        /// </summary>
        /// <param name="theFile">
        /// The the file.
        /// </param>
        /// <param name="i">
        /// The index of the class, for multiple file creation.
        /// </param>
        /// <returns>
        /// The create enumerations method
        /// </returns>
        private int CreateEnums(TextWriter theFile, int i)
        {
            string[] allKeys = generator.enumerations.Keys.ToArray();
            int internalI = i - generator.classes.Count;
            for (; internalI < generator.enumerations.Count; internalI++)
            {
                generator.CodeGenerator.WriteEnumeration(theFile, generator.enumerations[allKeys[internalI]]);                
            }

            return internalI + generator.classes.Count;
        }

        /// <summary>
        /// Generate code from the oxm generator class.
        /// Use this method after creating this object.
        /// </summary>
        /// <param name="localName"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool WriteFile(string localName, string path)
        {
            WriteFile((count) =>
                {
                    var fileName = string.Format(path, count > 1 ? count.ToString() : string.Empty, currentCodeGenerator.Language);
                      //+ localName + string.Format("oxm{0}.{1}", count > 1 ? count.ToString() : string.Empty, currentCodeGenerator.Language);
                    StreamWriter newFile = File.CreateText(fileName);
                    return newFile;

                }, (stream) => { stream.Flush(); stream.Close(); });
            
            return true;
        }

        private void WriteFile(Func<int, TextWriter> GetStream, Action<TextWriter> CloseStream)//, string localName, string path)
        {
            int i = 0, runningCount = 1;
            var theFile = new StringWriter();

            do
            {
                foreach (var @namespace in TextHelper.STANDARDNAMESPACES)
                {
                    currentCodeGenerator.WriteNameSpaceDecleration(theFile, @namespace);
                }
                foreach (var @namespace in Config.GetAdditionalRequiredNamespaces())
                {
                    currentCodeGenerator.WriteNameSpaceDecleration(theFile, @namespace);
                }
                if (generator.PlugIn != null)
                {
                    foreach (var @namespace in generator.PlugIn.Namespaces)
                    {
                        currentCodeGenerator.WriteNameSpaceDecleration(theFile, @namespace);
                    }
                }

                currentCodeGenerator.WriteFileTemplateHeader(theFile, Config.CurrentNamespace);
                if (runningCount == 1)
                {
                    if (generator.PlugIn != null)
                        generator.PlugIn.WritePlugin(theFile);
                    AddFactoryLayer(theFile, currentCodeGenerator);
                }

                i = CreateClasses(theFile, i);
                if (i < Config.MaxClassesInFile && i >= generator.classes.Count)
                {
                    i = CreateEnums(theFile, i);
                }

                currentCodeGenerator.WriteFileTemplateFooter(theFile);
                var newFile = GetStream(runningCount);
                WriteFile(theFile, newFile, runningCount);
                newFile.Flush();
                CloseStream(newFile);
                theFile = new StringWriter();
                runningCount++;
            }
            while (i < generator.classes.Count + generator.enumerations.Count);
        }

        internal void WriteFile(string p, Stream stream)
        {
            var memory = new StreamWriter(stream);
            WriteFile((count) => memory, st => { });
        }
    }
}
