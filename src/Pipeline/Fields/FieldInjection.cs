using System;
using System.Reflection;
using Unity.Injection;

namespace Unity.Pipeline
{
    public partial class FieldDiagnostic
    {
        public static Func<Type, InjectionMember, FieldInfo> InjectionValidatingSelector =
            (Type type, InjectionMember member) =>
            {
                FieldInfo? selection = null;
                var field = (InjectionMember<FieldInfo, object>)member;

                if (field.IsInitialized) throw new InvalidOperationException(
                    "Sharing an InjectionField between registrations is not supported");

                // Select Field
                foreach (var info in type.GetDeclaredFields())
                {
                    if (info.Name != field.Name) continue;

                    selection = info;
                    break;
                }

                // Validate
                if (null == selection)
                {
                    throw new ArgumentException(
                        $"Injected field '{field.Name}' could not be matched with any public field on type '{type?.Name}'.");
                }

                if (selection.IsStatic)
                    throw new InvalidOperationException(
                        $"Static field '{selection.Name}' on type '{type?.Name}' cannot be injected");

                if (selection.IsInitOnly)
                    throw new InvalidOperationException(
                        $"Readonly field '{selection.Name}' on type '{type?.Name}' cannot be injected");

                if (selection.IsPrivate)
                    throw new InvalidOperationException(
                        $"Private field '{selection.Name}' on type '{type?.Name}' cannot be injected");

                if (selection.IsFamily)
                    throw new InvalidOperationException(
                        $"Protected field '{selection.Name}' on type '{type?.Name}' cannot be injected");

                if (!field.Data.Matches(selection.FieldType))
                    throw new ArgumentException(
                        $"Injected data '{field.Data}' could not be matched with type of field '{selection.FieldType.Name}'.");

                return selection;
            };
    }
}
