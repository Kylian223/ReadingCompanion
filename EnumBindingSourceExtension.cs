using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace ReadingCompanion
{
    public class EnumBindingSourceExtension : MarkupExtension
    {
        public Type EnumType { get; private set; }

        public EnumBindingSourceExtension(Type enumtype)
        {
            if (enumtype == null || !enumtype.IsEnum)
                throw new Exception("enumtype must be of type Enum");

            EnumType = enumtype;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(EnumType);
        }
    }
}
