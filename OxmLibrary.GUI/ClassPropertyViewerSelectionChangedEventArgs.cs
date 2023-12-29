using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxmLibrary;
using OxmLibrary.CodeGeneration;

namespace OxmLibrary.GUI
{
    public class ClassPropertyViewerSelectionChangedEventArgs : EventArgs
    {
        public ClassDescriptor SelectedItem { get; private set; }

        public string NewClass { get; private set; }

        public ClassPropertyViewerSelectionChangedEventArgs(ClassDescriptor selected)
        {
            this.SelectedItem = selected;
        }

        public ClassPropertyViewerSelectionChangedEventArgs(string selected)
        {
            this.NewClass = selected;
        }
    }
}
