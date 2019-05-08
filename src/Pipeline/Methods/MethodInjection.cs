using System;
using System.Linq;
using System.Reflection;
using Unity.Injection;

namespace Unity
{
    public partial class MethodDiagnostic
    {
        public static Func<Type, InjectionMember, MethodInfo> InjectionValidatingSelector =
            (Type type, InjectionMember member) =>
            {
                MethodInfo? selection = null;
                var method = (InjectionMember<MethodInfo, object[]>)member;

                if (method.IsInitialized) throw new InvalidOperationException("Sharing an InjectionMethod between registrations is not supported");

                // Select Method
                foreach (var info in type.GetDeclaredMethods())
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
    }
}
