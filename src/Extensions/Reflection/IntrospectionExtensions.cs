using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace System.Reflection
{
#if NET40

    internal class TypeInfo 
    {
        private const BindingFlags DeclaredOnlyLookup = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        private Type _type;


        internal TypeInfo(Type type)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
        }


        public Assembly Assembly => _type.Assembly;

        public bool IsArray => _type.IsArray;

        public bool IsGenericTypeDefinition => _type.IsGenericTypeDefinition;

        public Type[] GenericTypeArguments => _type.GetGenericArguments();

        public Type[] GenericTypeParameters => _type.IsGenericTypeDefinition ? _type.GetGenericArguments()
                                                                             : Type.EmptyTypes;
        public string Name => _type.Name;

        public Type BaseType => _type.BaseType;

        public bool IsGenericType => _type.IsGenericType;

        public Type AsType() => _type;

        public bool IsAssignableFrom(Type type) => _type.IsAssignableFrom(type);

        public bool IsAssignableFrom(TypeInfo typeInfo) => _type.IsAssignableFrom(typeInfo.AsType());

        public bool IsGenericParameter => _type.IsGenericParameter;

        public bool IsInterface => _type.IsInterface;

        public bool IsAbstract => _type.IsAbstract;

        public bool IsSubclassOf(Type type) => _type.IsSubclassOf(type);

        public bool IsValueType => _type.IsValueType;

        public bool ContainsGenericParameters => _type.ContainsGenericParameters;

        public Type GetGenericTypeDefinition() => _type.GetGenericTypeDefinition();

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

        public virtual System.Reflection.TypeInfo GetDeclaredNestedType(String name)
        {
            var nt = _type.GetNestedType(name, DeclaredOnlyLookup);
            if (nt == null)
            {
                return null; //the extension method GetTypeInfo throws for null
            }
            else
            {
                return nt.GetTypeInfo();
            }
        }

        public virtual PropertyInfo GetDeclaredProperty(String name)
        {
            return _type.GetProperty(name, DeclaredOnlyLookup);
        }


        //// Properties

        public virtual IEnumerable<ConstructorInfo> DeclaredConstructors
        {
            get
            {
                return _type.GetConstructors(DeclaredOnlyLookup);
            }
        }

        public virtual IEnumerable<EventInfo> DeclaredEvents
        {
            get
            {
                return _type.GetEvents(DeclaredOnlyLookup);
            }
        }

        public virtual IEnumerable<FieldInfo> DeclaredFields
        {
            get
            {
                return _type.GetFields(DeclaredOnlyLookup);
            }
        }

        public virtual IEnumerable<MemberInfo> DeclaredMembers
        {
            get
            {
                return _type.GetMembers(DeclaredOnlyLookup);
            }
        }

        public virtual IEnumerable<MethodInfo> DeclaredMethods
        {
            get
            {
                return _type.GetMethods(DeclaredOnlyLookup);
            }
        }
        public virtual IEnumerable<System.Reflection.TypeInfo> DeclaredNestedTypes
        {
            get
            {
                foreach (var t in _type.GetNestedTypes(DeclaredOnlyLookup))
                {
                    yield return t.GetTypeInfo();
                }
            }
        }

        public virtual IEnumerable<PropertyInfo> DeclaredProperties
        {
            get
            {
                return _type.GetProperties(DeclaredOnlyLookup);
            }
        }


        public virtual IEnumerable<Type> ImplementedInterfaces
        {
            get
            {
                return _type.GetInterfaces();
            }
        }


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

    }
#endif


    internal static class IntrospectionExtensions
    {
#if NET40

        public static Attribute GetCustomAttribute(this ParameterInfo info, Type type)
        {
            return info.GetCustomAttributes(type, true)
                       .Cast<Attribute>()
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
        
        public static MethodInfo GetMethodInfo(this Delegate method)
        {
            return method.Method;
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
}

