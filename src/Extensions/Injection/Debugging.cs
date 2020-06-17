using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.Injection
{
    public static class Debugging
    {

        public static string Signature(this object[] data)
        {
            return string.Join(", ", data?.Select(GetSignature) ?? Enumerable.Empty<string>());

            string GetSignature(object param)
            {
                if (null == param) return "null";
                if (param is Type) return $"Type {param}";

                return $"{param.GetType().Name} {param}";
            }
        }


        public static string Signature(this MethodBase selection)
        {
            var sb = new List<string>();
            foreach (var parameter in selection.GetParameters())
                sb.Add($"{parameter.ParameterType} {parameter.Name}");

            return string.Join(", ", sb);
        }
    }
}
