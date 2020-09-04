using System;
using System.Runtime.CompilerServices;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    /// <summary>
    /// Extension class that adds a set of convenience overloads for Register method.
    /// </summary>
    public static partial class UnityNamespaceExtensions
    {
        public static ref readonly RegistrationDescriptor WithInjectionMembers(in this RegistrationDescriptor descriptor, params InjectionMember[] members)
        {
            descriptor.Manager.Add(members);
            return ref descriptor;
        }


        #region RegisterType overloads

        ///// <summary>
        ///// Create a type registration with default lifetime manager
        ///// </summary>
        ///// <param name="type"><see cref="Type"/> to create when resolved</param>
        ///// <param name="registerAs">Interfaces/aliases the component should be registered under</param>
        ///// <returns>The <see cref="RegistrationDescriptor"/> with registration data</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static RegistrationDescriptor RegisterType(this Type type, params Type[] registerAs)
        //    => new RegistrationDescriptor(type, null, (ITypeLifetimeManager)LifetimeManager._typeManager.Clone(), registerAs);

        ///// <summary>
        ///// Create a type registration with specific lifetime manager
        ///// </summary>
        ///// <param name="type"><see cref="Type"/> to create when resolved</param>
        ///// <param name="lifetime">The <see cref="LifetimeManager"/> that controls the lifetime of the resolved instance</param>
        ///// <param name="registerAs">Interfaces/aliases the component should be registered under</param>
        ///// <returns>The <see cref="RegistrationDescriptor"/> with registration data</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static RegistrationDescriptor RegisterType(this Type type, ITypeLifetimeManager lifetime, params Type[] registerAs)
        //    => new RegistrationDescriptor(type, null, lifetime, registerAs);

        ///// <summary>
        ///// Create a type registration with named contract and default lifetime manager
        ///// </summary>
        ///// <param name="type"><see cref="Type"/> to create when resolved</param>
        ///// <param name="name">Name of the contract</param>
        ///// <param name="registerAs">Interfaces/aliases the component should be registered under</param>
        ///// <returns>The <see cref="RegistrationDescriptor"/> with registration data</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static RegistrationDescriptor RegisterType(this Type type, string name, params Type[] registerAs)
        //    => new RegistrationDescriptor(type, name, (ITypeLifetimeManager)LifetimeManager._typeManager.Clone(), registerAs);

        ///// <summary>
        ///// Create a type registration with named contract and specific lifetime manager
        ///// </summary>
        ///// <param name="type"><see cref="Type"/> to create when resolved</param>
        ///// <param name="name">Name of the contract</param>
        ///// <param name="lifetime">The <see cref="LifetimeManager"/> that controls the lifetime of the resolved instance</param>
        ///// <param name="registerAs">Interfaces/aliases the component should be registered under</param>
        ///// <returns>The <see cref="RegistrationDescriptor"/> with registration data</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static RegistrationDescriptor RegisterType(this Type type, string name, ITypeLifetimeManager lifetime, params Type[] registerAs)
        //    => new RegistrationDescriptor(type, name, lifetime, registerAs);

        #endregion

        
        // TODO: conflict

        #region RegisterInstance overloads

        ///// <summary>
        ///// Create an instance registration with default lifetime manager
        ///// </summary>
        ///// <param name="instance">An instance to register</param>
        ///// <param name="registerAs">Interfaces/types the instance should be registered under</param>
        ///// <returns>The <see cref="RegistrationDescriptor"/> with registration data</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static RegistrationDescriptor RegisterInstance<TInstance>(this TInstance instance, params Type[] registerAs) 
        //    => new RegistrationDescriptor(instance, null, (IInstanceLifetimeManager)LifetimeManager._instanceManager.Clone(), registerAs);

        ///// <summary>
        ///// Create an instance registration with specific lifetime manager
        ///// </summary>
        ///// <param name="instance">An instance to register</param>
        ///// <param name="lifetime">The <see cref="LifetimeManager"/> that controls the lifetime of the instance</param>
        ///// <param name="registerAs">Interfaces/types the instance should be registered under</param>
        ///// <returns>The <see cref="RegistrationDescriptor"/> with registration data</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static RegistrationDescriptor RegisterInstance<TInstance>(this TInstance instance, IInstanceLifetimeManager lifetime, params Type[] registerAs)
        //    => new RegistrationDescriptor(instance, null, lifetime, registerAs);


        ///// <summary>
        ///// Create an instance registration with named contract and default lifetime manager
        ///// </summary>
        ///// <param name="type"><see cref="Type"/> to create when resolved</param>
        ///// <param name="name">Name of the contract</param>
        ///// <param name="registerAs">Interfaces/aliases the instance should be registered under</param>
        ///// <returns>The <see cref="RegistrationDescriptor"/> with registration data</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static RegistrationDescriptor RegisterInstance<TInstance>(this TInstance instance, string name, params Type[] registerAs)
        //    => new RegistrationDescriptor(instance, name, (IInstanceLifetimeManager)LifetimeManager._instanceManager.Clone(), registerAs);

        ///// <summary>
        ///// Create an instance registration with named contract and specific lifetime manager
        ///// </summary>
        ///// <param name="type"><see cref="Type"/> to create when resolved</param>
        ///// <param name="name">Name of the contract</param>
        ///// <param name="lifetime">The <see cref="LifetimeManager"/> that controls the lifetime of the instance</param>
        ///// <param name="registerAs">Interfaces/aliases the instance should be registered under</param>
        ///// <returns>The <see cref="RegistrationDescriptor"/> with registration data</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static RegistrationDescriptor RegisterInstance<TInstance>(this TInstance instance, string name, IInstanceLifetimeManager lifetime, params Type[] registerAs)
        //    => new RegistrationDescriptor(instance, name, lifetime, registerAs);

        #endregion


        #region RegisterFactory overloads

        ///// <summary>
        ///// Create a factory registration with specific lifetime manager
        ///// </summary>
        ///// <param name="factory">Factory to register</param>
        ///// <param name="registerAs">Interfaces/aliases the factory should be registered under</param>
        ///// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static RegistrationDescriptor RegisterFactory(this ResolveDelegate<IResolveContext> factory, params Type[] registerAs)
        //    => new RegistrationDescriptor(factory, null, (IFactoryLifetimeManager)LifetimeManager._factoryManager.Clone(), registerAs);

        ///// <summary>
        ///// Create a factory registration with default lifetime manager
        ///// </summary>
        ///// <param name="factory">Factory to register</param>
        ///// <param name="lifetime">The <see cref="LifetimeManager"/> that controls the lifetime of the resolved instance</param>
        ///// <param name="registerAs">Interfaces/aliases the factory should be registered under</param>
        ///// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static RegistrationDescriptor RegisterFactory(this ResolveDelegate<IResolveContext> factory, IFactoryLifetimeManager lifetime, params Type[] registerAs)
        //    => new RegistrationDescriptor(factory, null, lifetime, registerAs);

        ///// <summary>
        ///// Create a factory registration with named contract and default lifetime manager
        ///// </summary>
        ///// <param name="factory">Factory to register</param>
        ///// <param name="name">Name of the contract</param>
        ///// <param name="registerAs">Interfaces/aliases the component should be registered under</param>
        ///// <returns>The <see cref="RegistrationDescriptor"/> with registration data</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static RegistrationDescriptor RegisterFactory(this ResolveDelegate<IResolveContext> factory, string name, params Type[] registerAs)
        //    => new RegistrationDescriptor(factory, name, (IFactoryLifetimeManager)LifetimeManager._factoryManager.Clone(), registerAs);

        ///// <summary>
        ///// Create a factory registration with named contract and default lifetime manager
        ///// </summary>
        ///// <param name="factory">Factory to register</param>
        ///// <param name="name">Name of the contract</param>
        ///// <param name="lifetime">The <see cref="LifetimeManager"/> that controls the lifetime of the resolved instance</param>
        ///// <param name="registerAs">Interfaces/aliases the component should be registered under</param>
        ///// <returns>The <see cref="RegistrationDescriptor"/> with registration data</returns>
        //public static RegistrationDescriptor RegisterFactory(this ResolveDelegate<IResolveContext> factory, string name, IFactoryLifetimeManager lifetime, params Type[] registerAs)
        //    => new RegistrationDescriptor(factory, name, lifetime, registerAs);

        #endregion
    }
}
