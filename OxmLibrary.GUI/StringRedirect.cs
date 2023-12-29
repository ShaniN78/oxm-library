// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringRedirect.cs" company="Mosh Productions">
//   All rights reserved.
// </copyright>
// <summary>
//   Builds a basic StringWriter on top of a text box
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace OxmLibrary.GUI
{
    /// <summary>
    /// Builds a basic StringWriter on top of a text box
    /// </summary>
    public class StringRedirect : StringWriter
    {
        /// <summary>
        /// Text box to write to.
        /// </summary>
        private TextBox localTextBox;

        /// <summary>
        /// Constructor that builds the writer.
        /// </summary>
        /// <param name="textBox"></param>
        public StringRedirect(TextBox textBox)
        {
            localTextBox = textBox;
        }

        /// <summary>
        /// Write an empty line
        /// </summary>
        public override void WriteLine()
        {
            WriteLine(string.Empty);
        }

        /// <summary>
        /// Writes a line to the text box.
        /// </summary>
        /// <param name="value">Text to write</param>
        public override void WriteLine(string value)
        {
            localTextBox.AppendText(value);
            localTextBox.AppendText(Environment.NewLine);
        }

        public override void WriteLine(string format, object arg0)
        {
            WriteLine(string.Format(format, arg0));
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            WriteLine(string.Format(format, arg0, arg1));
        }

        public override void  WriteLine(string format, object arg0, object arg1, object arg2)
        {
            WriteLine(string.Format(format, arg0, arg1, arg2));
        }

        public override void WriteLine(string format, params object[] arg)
        {
            WriteLine(string.Format(format, arg));
        }

    }
}