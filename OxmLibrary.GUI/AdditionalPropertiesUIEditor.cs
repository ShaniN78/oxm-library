using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using OxmLibrary.CodeGeneration;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace OxmLibrary.GUI
{
    public class AdditionalPropertiesUIEditor : UITypeEditor
    {
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context != null && context.Instance != null && provider != null)
            {
                var ik = (GenerationAdditionalParametersCollection)value;
                var prov = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                var editor = new AdditionalsEditor();
                editor.AddRange(ik.Parameters.Values);
                prov.ShowDialog(editor);
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

    }


}
