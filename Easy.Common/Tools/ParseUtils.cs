using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easy.Common
{
    internal static class ParseUtils
    {
        public static readonly IPropertyNameResolver DefaultPropertyNameResolver = new DefaultPropertyNameResolver();
        public static readonly IPropertyNameResolver LenientPropertyNameResolver = new LenientPropertyNameResolver();

        public static object NullValueType(Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
#else
            return type.GetDefaultValue();
#endif
        }

        public static object ParseObject(string value)
        {
            return value;
        }

        public static object ParseEnum(Type type, string value)
        {
            return Enum.Parse(type, value, false);
        }

        public static ParseStringDelegate GetSpecialParseMethod(Type type)
        {
            if (type == typeof(Uri))
                return x => new Uri(x.FromCsvField());

            //Warning: typeof(object).IsInstanceOfType(typeof(Type)) == True??
            if (type.InstanceOfType(typeof(Type)))
                return ParseType;

            if (type == typeof(Exception))
                return x => new Exception(x);

            if (type.IsInstanceOf(typeof(Exception)))
                return DeserializeTypeUtils.GetParseMethod(type);

            return null;
        }

        public static Type ParseType(string assemblyQualifiedName)
        {
            return AssemblyUtils.FindType(assemblyQualifiedName.FromCsvField());
        }

        public static object TryParseEnum(Type enumType, string str)
        {
            if (JsConfig.EmitLowercaseUnderscoreNames)
                str = str.Replace("_", "");

            return Enum.Parse(enumType, str, ignoreCase: true);
        }
    }
}
