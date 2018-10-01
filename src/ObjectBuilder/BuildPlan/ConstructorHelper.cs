using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Selection;
using Unity.Lifetime;

namespace Unity.ObjectBuilder.BuildPlan
{
    public static class ConstructorHelper
    {
        /// <summary>
        /// Build up the string that will represent the constructor signature
        /// in any exception message.
        /// </summary>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public static string CreateSignatureString(ConstructorInfo constructor)
        {
            string typeName = (constructor ?? throw new ArgumentNullException(nameof(constructor))).DeclaringType.FullName;
            ParameterInfo[] parameters = constructor.GetParameters();
            string[] parameterDescriptions = new string[parameters.Length];
            for (int i = 0; i < parameters.Length; ++i)
            {
                parameterDescriptions[i] = string.Format(CultureInfo.CurrentCulture,
                    "{0} {1}",
                    parameters[i].ParameterType.FullName,
                    parameters[i].Name);
            }

            return string.Format(CultureInfo.CurrentCulture,
                "{0}({1})",
                typeName,
                string.Join(", ", parameterDescriptions));
        }

        internal static bool IsInvalidConstructor(TypeInfo target, IBuilderContext context, SelectedConstructor selectedConstructor)
        {
            if (selectedConstructor.Constructor.GetParameters().Any(p => p.ParameterType.GetTypeInfo() == target))
            {
                var policy = (ILifetimePolicy)context.Policies.Get(context.BuildKey.Type, 
                    context.BuildKey.Name, 
                    typeof(ILifetimePolicy), out var _);
                if (null == policy?.GetValue())
                    return true;
            }

            return false;
        }
    }
}
