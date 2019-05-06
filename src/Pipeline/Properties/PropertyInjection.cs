using System;
using System.Reflection;
using Unity.Injection;

namespace Unity.Pipeline
{
    public partial class PropertyDiagnostic
    {
        public static Func<Type, InjectionMember, PropertyInfo> InjectionValidatingSelector =
            (Type type, InjectionMember member) =>
            {
                PropertyInfo? selection = null;
                var property = (InjectionMember<PropertyInfo, object>)member;

                if (property.IsInitialized) throw new InvalidOperationException("Sharing an InjectionProperty between registrations is not supported");

                // Select Property
                foreach (var info in type.GetDeclaredProperties())
                {
                    if (info.Name != property.Name) continue;

                    selection = info;
                    break;
                }

                // Validate
                if (null == selection)
                {
                    throw new ArgumentException(
                        $"Injected property '{property.Name}' could not be matched with any property on type '{type?.Name}'.");
                }

                if (!selection.CanWrite)
                    throw new InvalidOperationException(
                        $"Readonly property '{selection.Name}' on type '{type?.Name}' cannot be injected");

                if (0 != selection.GetIndexParameters().Length)
                    throw new InvalidOperationException(
                        $"Indexer '{selection.Name}' on type '{type?.Name}' cannot be injected");

                var setter = selection.GetSetMethod(true);

                if (setter.IsStatic)
                    throw new InvalidOperationException(
                        $"Static property '{selection.Name}' on type '{type?.Name}' cannot be injected");

                if (setter.IsPrivate)
                    throw new InvalidOperationException(
                        $"Private property '{selection.Name}' on type '{type?.Name}' cannot be injected");

                if (setter.IsFamily)
                    throw new InvalidOperationException(
                        $"Protected property '{selection.Name}' on type '{type?.Name}' cannot be injected");

                if (!property.Data.Matches(selection.PropertyType))
                {
                    throw new ArgumentException(
                        $"Injected data '{property.Data}' could not be matched with type of property '{selection.PropertyType.Name}'.");
                }

                return selection;
            };
    }
}
