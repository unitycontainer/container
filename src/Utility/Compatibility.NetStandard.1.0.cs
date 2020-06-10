using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity
{
    internal static class Compatibility_NetStandard_1_0
    {
        public static FieldInfo[] GetFields(this Type type, BindingFlags _)
        {
            return type.GetTypeInfo()
                       .DeclaredFields
                       .Where(field => !field.IsStatic)
                       .ToArray();
        }

        public static PropertyInfo[] GetProperties(this Type type, BindingFlags _)
        {
            return type.GetTypeInfo()
                       .DeclaredProperties
                       .ToArray();
        }

        public static MethodInfo[] GetMethods(this Type type, BindingFlags _)
        {
            return type.GetTypeInfo()
                       .DeclaredMethods
                       .Where(method => !method.IsStatic)
                       .ToArray();
        }

        public static IEnumerable<ConstructorInfo> GetConstructors(this Type type)
        {
            return type.GetTypeInfo()
                       .DeclaredConstructors
                       .Where(ctor => !ctor.IsFamily && !ctor.IsPrivate && !ctor.IsStatic);
        }

        public static FieldInfo? GetField(this Type type, string name)
        {
            return type.GetTypeInfo()
                       .DeclaredFields
                       .FirstOrDefault(p => p.Name == name);
        }


        public static PropertyInfo? GetProperty(this Type type, string name)
        {
            return type.GetTypeInfo()
                       .DeclaredProperties
                       .FirstOrDefault(p => p.Name == name);
        }

        public static MethodInfo GetSetMethod(this PropertyInfo info, bool _)
        {
            return info.SetMethod;
        }

        public static bool IsAssignableFrom(this Type match, Type type)
        { 
            var typeInfo = type.GetTypeInfo();
            var matchInfo = match.GetTypeInfo();

            return matchInfo.IsAssignableFrom(typeInfo);
        }
    }
}

namespace System.Reflection
{
    //
    // Summary:
    //     Specifies flags that control binding and the way in which the search for members
    //     and types is conducted by reflection.
    [Flags]
    internal enum BindingFlags
    {
        //
        // Summary:
        //     Specifies that no binding flags are defined.
        Default = 0,
        //
        // Summary:
        //     Specifies that the case of the member name should not be considered when binding.
        IgnoreCase = 1,
        //
        // Summary:
        //     Specifies that only members declared at the level of the supplied type's hierarchy
        //     should be considered. Inherited members are not considered.
        DeclaredOnly = 2,
        //
        // Summary:
        //     Specifies that instance members are to be included in the search.
        Instance = 4,
        //
        // Summary:
        //     Specifies that static members are to be included in the search.
        Static = 8,
        //
        // Summary:
        //     Specifies that public members are to be included in the search.
        Public = 16,
        //
        // Summary:
        //     Specifies that non-public members are to be included in the search.
        NonPublic = 32,
        //
        // Summary:
        //     Specifies that public and protected static members up the hierarchy should be
        //     returned. Private static members in inherited classes are not returned. Static
        //     members include fields, methods, events, and properties. Nested types are not
        //     returned.
        FlattenHierarchy = 64,
        //
        // Summary:
        //     Specifies that a method is to be invoked. This must not be a constructor or a
        //     type initializer. This flag is passed to an InvokeMember method to invoke a method.
        InvokeMethod = 256,
        //
        // Summary:
        //     Specifies that reflection should create an instance of the specified type. Calls
        //     the constructor that matches the given arguments. The supplied member name is
        //     ignored. If the type of lookup is not specified, (Instance | Public) will apply.
        //     It is not possible to call a type initializer. This flag is passed to an InvokeMember
        //     method to invoke a constructor.
        CreateInstance = 512,
        //
        // Summary:
        //     Specifies that the value of the specified field should be returned. This flag
        //     is passed to an InvokeMember method to get a field value.
        GetField = 1024,
        //
        // Summary:
        //     Specifies that the value of the specified field should be set. This flag is passed
        //     to an InvokeMember method to set a field value.
        SetField = 2048,
        //
        // Summary:
        //     Specifies that the value of the specified property should be returned. This flag
        //     is passed to an InvokeMember method to invoke a property getter.
        GetProperty = 4096,
        //
        // Summary:
        //     Specifies that the value of the specified property should be set. For COM properties,
        //     specifying this binding flag is equivalent to specifying PutDispProperty and
        //     PutRefDispProperty. This flag is passed to an InvokeMember method to invoke a
        //     property setter.
        SetProperty = 8192,
        //
        // Summary:
        //     Specifies that the PROPPUT member on a COM object should be invoked. PROPPUT
        //     specifies a property-setting function that uses a value. Use PutDispProperty
        //     if a property has both PROPPUT and PROPPUTREF and you need to distinguish which
        //     one is called.
        PutDispProperty = 16384,
        //
        // Summary:
        //     Specifies that the PROPPUTREF member on a COM object should be invoked. PROPPUTREF
        //     specifies a property-setting function that uses a reference instead of a value.
        //     Use PutRefDispProperty if a property has both PROPPUT and PROPPUTREF and you
        //     need to distinguish which one is called.
        PutRefDispProperty = 32768,
        //
        // Summary:
        //     Specifies that types of the supplied arguments must exactly match the types of
        //     the corresponding formal parameters. Reflection throws an exception if the caller
        //     supplies a non-null Binder object, since that implies that the caller is supplying
        //     BindToXXX implementations that will pick the appropriate method. The default
        //     binder ignores this flag, while custom binders can implement the semantics of
        //     this flag.
        ExactBinding = 65536,
        //
        // Summary:
        //     Not implemented.
        SuppressChangeType = 131072,
        //
        // Summary:
        //     Returns the set of members whose parameter count matches the number of supplied
        //     arguments. This binding flag is used for methods with parameters that have default
        //     values and methods with variable arguments (varargs). This flag should only be
        //     used with System.Type.InvokeMember(System.String,System.Reflection.BindingFlags,System.Reflection.Binder,System.Object,System.Object[],System.Reflection.ParameterModifier[],System.Globalization.CultureInfo,System.String[]).Parameters
        //     with default values are used only in calls where trailing arguments are omitted.
        //     They must be the last arguments.
        OptionalParamBinding = 262144,
        //
        // Summary:
        //     Used in COM interop to specify that the return value of the member can be ignored.
        IgnoreReturn = 16777216,
        DoNotWrapExceptions = 33554432
    }
}