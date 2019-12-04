using System;
using System.Linq;
using System.Reflection;

namespace Unity.Injection
{
    public static class Validating
    {
        public static Func<Type, InjectionMember, ConstructorInfo> ConstructorSelector =
            (Type type, InjectionMember member) =>
            {
                ConstructorInfo selection = null;
                var ctor = (InjectionMember<ConstructorInfo, object[]>)member;

                if (ctor.IsInitialized) throw new InvalidOperationException("Sharing an InjectionConstructor between registrations is not supported");

                // Select Constructor
                foreach (var info in ctor.DeclaredMembers(type))
                {
                    if (!ctor.Data.MatchMemberInfo(info)) continue;

                    if (null != selection)
                    {
                        throw new ArgumentException(
                            $"Constructor .ctor({ctor.Data.Signature()}) is ambiguous, it could be matched with more than one constructor on type {type?.Name}.");
                    }

                    selection = info;
                }

                // Validate
                if (null != selection) return selection;

                throw new ArgumentException(
                    $"Injected constructor .ctor({ctor.Data.Signature()}) could not be matched with any public constructors on type {type?.Name}.");
            };


        public static Func<Type, InjectionMember, MethodInfo> MethodSelector =
            (Type type, InjectionMember member) =>
            {
                MethodInfo selection = null;
                var method = (InjectionMember<MethodInfo, object[]>)member;

                if (method.IsInitialized) throw new InvalidOperationException("Sharing an InjectionMethod between registrations is not supported");

                // Select Method
                foreach (var info in type.GetDeclaredMethods())
                {
                    if (method.Name != info.Name || !method.Data.MatchMemberInfo(info)) continue;

                    if (null != selection)
                    {
                        throw new ArgumentException(
                            $"Method {method.Name}({method.Data.Signature()}) is ambiguous, it could be matched with more than one method on type {type?.Name}.");
                    }

                    selection = info;
                }

                // Validate
                if (null == selection)
                {
                    throw new ArgumentException(
                        $"Injected method {method.Name}({method.Data.Signature()}) could not be matched with any public methods on type {type?.Name}.");
                }

                if (selection.IsStatic)
                {
                    throw new ArgumentException(
                        $"Static method {method.Name} on type '{selection.DeclaringType.Name}' cannot be injected");
                }

                if (selection.IsPrivate)
                    throw new InvalidOperationException(
                        $"Private method '{method.Name}' on type '{selection.DeclaringType.Name}' cannot be injected");

                if (selection.IsFamily)
                    throw new InvalidOperationException(
                        $"Protected method '{method.Name}' on type '{selection.DeclaringType.Name}' cannot be injected");

                if (selection.IsGenericMethodDefinition)
                {
                    throw new ArgumentException(
                        $"Open generic method {method.Name} on type '{selection.DeclaringType.Name}' cannot be injected");
                }

                var parameters = selection.GetParameters();
                if (parameters.Any(param => param.IsOut))
                {
                    throw new ArgumentException(
                        $"Method {method.Name} on type '{selection.DeclaringType.Name}' cannot be injected. Methods with 'out' parameters are not injectable.");
                }

                if (parameters.Any(param => param.ParameterType.IsByRef))
                {
                    throw new ArgumentException(
                        $"Method {method.Name} on type '{selection.DeclaringType.Name}' cannot be injected. Methods with 'ref' parameters are not injectable.");
                }

                return selection;
            };


        public static Func<Type, InjectionMember, FieldInfo> FieldSelector =
            (Type type, InjectionMember member) =>
            {
                FieldInfo selection = null;
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



        public static Func<Type, InjectionMember, PropertyInfo> PropertySelector =
            (Type type, InjectionMember member) =>
            {
                PropertyInfo selection = null;
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
