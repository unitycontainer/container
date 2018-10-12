using System;
using System.Linq;

#if NET40
namespace System.Reflection
{
    using Unity;

    internal class TypeInfo 
    {
        private const BindingFlags DeclaredOnlyLookup = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        private readonly Type _type;


        internal TypeInfo(Type type)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
        }


        public Assembly Assembly => _type.Assembly;

        public bool IsGenericTypeDefinition => _type.IsGenericTypeDefinition;

        public Type[] GenericTypeArguments => _type.GetGenericArguments();

        public Type[] GenericTypeParameters => _type.IsGenericTypeDefinition ? _type.GetGenericArguments()
                                                                             : Type.EmptyTypes;
        public string Name => _type.Name;

        public Type BaseType => _type.BaseType;

        public bool IsGenericType => _type.IsGenericType;

        public Type AsType() => _type;

        public bool IsAssignableFrom(TypeInfo typeInfo) => _type.IsAssignableFrom(typeInfo.AsType());

        public bool IsGenericParameter => _type.IsGenericParameter;

        public bool IsInterface => _type.IsInterface;

        public bool IsAbstract => _type.IsAbstract;

        public bool IsSubclassOf(Type type) => _type.IsSubclassOf(type);

        public bool IsValueType => _type.IsValueType;

        public bool ContainsGenericParameters => _type.ContainsGenericParameters;

        public bool IsConstructedGenericType => _type.IsGenericType && !_type.ContainsGenericParameters;

#region moved over from Type

        //// Fields

        public virtual EventInfo GetDeclaredEvent(String name)
        {
            return _type.GetEvent(name, DeclaredOnlyLookup);
        }
        public virtual FieldInfo GetDeclaredField(String name)
        {
            return _type.GetField(name, DeclaredOnlyLookup);
        }
        public virtual MethodInfo GetDeclaredMethod(String name)
        {
            return _type.GetMethod(name, DeclaredOnlyLookup);
        }

        public virtual IEnumerable<MethodInfo> GetDeclaredMethods(String name)
        {
            foreach (MethodInfo method in _type.GetMethods(DeclaredOnlyLookup))
            {
                if (method.Name == name)
                    yield return method;
            }
        }

        public virtual TypeInfo GetDeclaredNestedType(String name)
        {
            var nt = _type.GetNestedType(name, DeclaredOnlyLookup);
            return nt == null ? null : nt.GetTypeInfo();
        }

        public virtual PropertyInfo GetDeclaredProperty(String name)
        {
            return _type.GetProperty(name, DeclaredOnlyLookup);
        }


        //// Properties

        public virtual IEnumerable<ConstructorInfo> DeclaredConstructors => _type.GetConstructors(DeclaredOnlyLookup);

        public virtual IEnumerable<EventInfo> DeclaredEvents => _type.GetEvents(DeclaredOnlyLookup);

        public virtual IEnumerable<FieldInfo> DeclaredFields => _type.GetFields(DeclaredOnlyLookup);

        public virtual IEnumerable<MemberInfo> DeclaredMembers => _type.GetMembers(DeclaredOnlyLookup);

        public virtual IEnumerable<MethodInfo> DeclaredMethods => _type.GetMethods(DeclaredOnlyLookup);

        public virtual IEnumerable<TypeInfo> DeclaredNestedTypes
        {
            get
            {
                foreach (var t in _type.GetNestedTypes(DeclaredOnlyLookup))
                {
                    yield return t.GetTypeInfo();
                }
            }
        }

        public virtual IEnumerable<PropertyInfo> DeclaredProperties => _type.GetProperties(DeclaredOnlyLookup);


        public virtual IEnumerable<Type> ImplementedInterfaces => _type.GetInterfaces();

#endregion

        public override int GetHashCode()
        {
            return _type.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return _type.Equals(obj);
        }

        public static bool operator ==(TypeInfo left, TypeInfo right)
        {
            return left?.GetHashCode() == right?.GetHashCode();
        }

        public static bool operator !=(TypeInfo left, TypeInfo right)
        {
            return left?.GetHashCode() != right?.GetHashCode();
        }

        public Type GetGenericTypeDefinition()
        {
            return _type.GetGenericTypeDefinition();
        }
    }
}
#endif

namespace Unity
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;


    internal static class Compatibility
    {

        public static MethodInfo[] Get_Methods(this Type info, params object[] options)
        {
            IEnumerable<MethodInfo> GetMethodsHierarchical(Type type)
            {
                if (type == null)
                {
                    return Enumerable.Empty<MethodInfo>();
                }

                if (type == typeof(object))
                {
                    return type.GetTypeInfo().DeclaredMethods.Where(m => !m.IsStatic);
                }

                return type.GetTypeInfo().DeclaredMethods.Where(m => !m.IsStatic)
                    .Concat(GetMethodsHierarchical(type.GetTypeInfo().BaseType));
            }

            return GetMethodsHierarchical(info).ToArray();
        }


#if NETSTANDARD1_0
        public static ConstructorInfo[] GetConstructors(this Type type)
        {
            var ctors = type.GetTypeInfo().DeclaredConstructors;
            return ctors is ConstructorInfo[] array ? array : ctors.ToArray();
        }

        public static MethodInfo[] GetMethods(this Type info, object options)
        {
            IEnumerable<MethodInfo> GetMethodsHierarchical(Type type)
            {
                if (type == null)
                {
                    return Enumerable.Empty<MethodInfo>();
                }

                if (type == typeof(object))
                {
                    return type.GetTypeInfo().DeclaredMethods.Where(m => !m.IsStatic);
                }

                return type.GetTypeInfo().DeclaredMethods.Where(m => !m.IsStatic)
                    .Concat(GetMethodsHierarchical(type.GetTypeInfo().BaseType));
            }

            return GetMethodsHierarchical(info).ToArray();
        }
#endif

#if NET40
        public static Attribute GetCustomAttribute(this ParameterInfo parameter, Type type)
        {
            return parameter.GetCustomAttributes(false)
                            .OfType<DependencyResolutionAttribute>()
                            .FirstOrDefault();
        }

        public static TypeInfo GetTypeInfo(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return new TypeInfo(type);
        }

        public static Delegate CreateDelegate(this MethodInfo method, Type delegateType)
        {
            return Delegate.CreateDelegate(delegateType, method);
        }

        public static Delegate CreateDelegate(this MethodInfo method, Type delegateType, object target)
        {
            return Delegate.CreateDelegate(delegateType, target, method);
        }
#else
        public static MethodInfo GetGetMethod(this PropertyInfo info, bool _)
        {
            return info.GetMethod;
        }

        public static MethodInfo GetSetMethod(this PropertyInfo info, bool _)
        {
            return info.SetMethod;
        }
#endif
    }

#if NETSTANDARD1_0

    [Flags]
    public enum BindingFlags
    {
        Default = 0,
        IgnoreCase = 1,
        DeclaredOnly = 2,
        Instance = 4,
        Static = 8,
        Public = 16,
        NonPublic = 32,
        FlattenHierarchy = 64,
        InvokeMethod = 256,
        CreateInstance = 512,
        GetField = 1024,
        SetField = 2048,
        GetProperty = 4096,
        SetProperty = 8192
    }
#endif

}