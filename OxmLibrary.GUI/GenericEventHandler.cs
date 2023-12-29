using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OxmLibrary.GUI
{
    public delegate void GenericEventHandler<T>(object sender, GenericEventArgs<T> Args);

    public class GenericEventArgs<T> : EventArgs
    {
        public T Item { get; set; }



        public GenericEventArgs(T init)
        {
            this.Item = init;
        }
    }

    public delegate void GenericEventHandler<T, T2>(object sender, GenericEventArgs<T, T2> Args);

    public class GenericEventArgs<T, T2> : EventArgs
    {
        public T Item { get; set; }

        public T2 Item2 { get; set; }

        public GenericEventArgs(T init, T2 init2)
        {
            this.Item = init;
            this.Item2 = init2;
        }
    }
}
