using System;

namespace Unity
{
    public static partial class Injection
    {
        public static InjectionMember Factory(Func<IUnityContainer, object> factoryFunc) => new InjectionFactory(factoryFunc);

        public static InjectionMember Factory(Func<IUnityContainer, Type, string, object> factoryFunc) => new InjectionFactory(factoryFunc);
    }
}
