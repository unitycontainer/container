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

                // Select Method
                foreach (var info in method.DeclaredMembers(type))
                {
                    if (!method.Data.MatchMemberInfo(info)) continue;

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
                        $"The method {type?.Name}.{method.Name}({method.Data.Signature()}) is static. Static methods cannot be injected.");
                }

                if (selection.IsGenericMethodDefinition)
                {
                    throw new ArgumentException(
                        $"The method {type?.Name}.{method.Name}({method.Data.Signature()}) is an open generic method. Open generic methods cannot be injected.");
                }

                var parameters = selection.GetParameters();
                if (parameters.Any(param => param.IsOut))
                {
                    throw new ArgumentException(
                        $"The method {type?.Name}.{method.Name}({method.Data.Signature()}) has at least one out parameter. Methods with out parameters cannot be injected.");
                }

                if (parameters.Any(param => param.ParameterType.IsByRef))
                {
                    throw new ArgumentException(
                        $"The method {type?.Name}.{method.Name}({method.Data.Signature()}) has at least one ref parameter.Methods with ref parameters cannot be injected.");
                }

                return selection;
            };


        public static Func<Type, InjectionMember, FieldInfo> FieldSelector =
            (Type type, InjectionMember member) =>
            {
                FieldInfo selection = null;
                var field = (InjectionMember<FieldInfo, object>)member;

                // Select Field
                foreach (var info in field.DeclaredMembers(type))
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

                if (selection.IsInitOnly)
                {
                    throw new ArgumentException(
                        $"Field '{selection.Name}' on type '{type?.Name}' is Read Only and can not be injected.");
                }

                if (!field.Data.Matches(selection.FieldType))
                {
                    throw new ArgumentException(
                        $"Injected data '{field.Data}' could not be matched with type of field '{selection.FieldType.Name}'.");
                }

                return selection;
            };



        public static Func<Type, InjectionMember, PropertyInfo> PropertySelector =
            (Type type, InjectionMember member) =>
            {
                PropertyInfo selection = null;
                var property = (InjectionMember<PropertyInfo, object>)member;

                // Select Property
                foreach (var info in property.DeclaredMembers(type))
                {
                    if (info.Name != property.Name) continue;

                    selection = info;
                    break;
                }

                // Validate
                if (null == selection)
                {
                    throw new ArgumentException(
                        $"Injected property '{property.Name}' could not be matched with any public property on type '{type?.Name}'.");
                }

                if (!selection.CanWrite)
                {
                    throw new ArgumentException(
                        $"Property '{selection.Name}' on type '{type?.Name}' is Read Only and can not be injected.");
                }

                if (!property.Data.Matches(selection.PropertyType))
                {
                    throw new ArgumentException(
                        $"Injected data '{property.Data}' could not be matched with type of property '{selection.PropertyType.Name}'.");
                }

                return selection;
            };
    }
}
