using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace OxmLibrary.CodeGeneration
{
    public class GenerationAdditionalParametersConverter : MultilineStringConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                GenerationAdditionalParametersCollection collect = (GenerationAdditionalParametersCollection)value;

                return string.Join(";", (from i in collect.Parameters
                                         select i.Key + ":" + i.Value.Value).ToArray());
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                GenerationAdditionalParametersCollection collect = new GenerationAdditionalParametersCollection();
                var result =  (from i in ((string)value).Split(';')
                               let split = i.Split(':')
                               select new GenerationAdditionalParameters(split[0], split[1], split[0])).ToList();
                result.ForEach(a => collect.Add(a));
                return result;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
