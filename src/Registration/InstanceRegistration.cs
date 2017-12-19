using System;
using System.Globalization;
using System.Reflection;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Storage;

namespace Unity.Registration
{
    /// <summary>
    /// This class holds instance registration
    /// </summary>
    public class InstanceRegistration : INamedType, 
                                        IContainerRegistration, 
                                        IBuildPlanCreatorPolicy, 
                                        IBuildPlanPolicy, 
                                        IPolicyStore,
                                        IDisposable
    {
        #region Fields

        private readonly int _hash;

        #endregion



        #region Constructors

        /// <summary>
        /// Instance registration with the container.
        /// </summary>
        /// <remarks> <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        /// of the container creating the instance the first time it is requested, the user
        /// creates the instance ahead of type and adds that instance to the container.
        /// </para></remarks>
        /// <param name="registrationType">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="instance">Object to be returned.</param>
        /// <param name="registrationName">Name for registration.</param>
        /// <param name="lifetimeManager">
        /// <para>If null or <see cref="ContainerControlledLifetimeManager"/>, the container will take over the lifetime of the instance,
        /// calling Dispose on it (if it's <see cref="IDisposable"/>) when the container is Disposed.</para>
        /// <para>
        ///  If <see cref="ExternallyControlledLifetimeManager"/>, container will not maintain a strong reference to <paramref name="instance"/>. 
        /// User is responsible for disposing instance, and for keeping the instance typeFrom being garbage collected.</para></param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public InstanceRegistration(Type registrationType, string registrationName, object instance, LifetimeManager lifetimeManager)
        {
            // Validate input
            if (null != registrationType) InstanceIsAssignable(registrationType, instance, nameof(instance));

            Name = string.IsNullOrEmpty(registrationName) ? null : registrationName;
            Type = registrationType ?? (instance ?? throw new ArgumentNullException(nameof(instance))).GetType();
            _hash = Type.GetHashCode() + Name?.GetHashCode() ?? 0;

            var lifetime = lifetimeManager ?? new ContainerControlledLifetimeManager();
            if (lifetime.InUse) throw new InvalidOperationException(Constants.LifetimeManagerInUse);

            lifetime.SetValue(instance);
            LifetimeManager = lifetime;

            MappedToType = registrationType ?? instance.GetType();
        }

        #endregion


        #region IPolicyStore

        public IBuilderPolicy Get(Type policyInterface)
        {
            if (typeof(ILifetimePolicy) == policyInterface)
                return LifetimeManager;

            if (typeof(IBuildPlanPolicy) == policyInterface ||
                typeof(IBuildPlanCreatorPolicy) == policyInterface)
                return this;

            return null;
        }

        public void Set(Type policyInterface, IBuilderPolicy policy)
        {
        }

        public void Clear(Type policyInterface)
        {
        }

        public void ClearAll()
        {
        }

        #endregion


        #region INamedType

        public string Name { get; }

        public Type Type { get; }

        #endregion


        #region IContainerRegistration

        public Type RegisteredType => Type;

        public Type MappedToType { get; }

        public LifetimeManager LifetimeManager { get; }

        #endregion


        #region IBuildPlanCreatorPolicy

        public IBuildPlanPolicy CreatePlan(IBuilderContext context, INamedType buildKey)
        {
            return this;
        }

        #endregion


        #region IBuildPlanPolicy

        public void BuildUp(IBuilderContext context)
        {
            context.Existing = LifetimeManager.GetValue();
            context.BuildComplete = true;
        }

        #endregion


        #region Implementation

        private static void InstanceIsAssignable(Type assignmentTargetType, object assignmentInstance, string argumentName)
        {
            if (!(assignmentTargetType ?? throw new ArgumentNullException(nameof(assignmentTargetType)))
                .GetTypeInfo().IsAssignableFrom((assignmentInstance ?? throw new ArgumentNullException(nameof(assignmentInstance))).GetType().GetTypeInfo()))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Constants.TypesAreNotAssignable,
                        assignmentTargetType, GetTypeName(assignmentInstance)),
                    argumentName);
            }
        }

        private static string GetTypeName(object assignmentInstance)
        {
            string assignmentInstanceType;
            try
            {
                assignmentInstanceType = assignmentInstance.GetType().FullName;
            }
            catch (Exception)
            {
                assignmentInstanceType = Constants.UnknownType;
            }

            return assignmentInstanceType;
        }

        #endregion


        #region Object

        public override bool Equals(object obj) => obj is IContainerRegistration registration && 
                                                RegisteredType == registration.RegisteredType && 
                                                                    Name == registration.Name;
        public override int GetHashCode()
        {
            return _hash;
        }

        #endregion


        #region IDisposable

        public void Dispose()
        {
            if (LifetimeManager is IDisposable disposable)
                disposable.Dispose();
        }

        #endregion
    }
}
