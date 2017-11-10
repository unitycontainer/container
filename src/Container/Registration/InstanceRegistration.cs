using System;
using System.Reflection;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;

namespace Unity.Container.Registration
{
    public class InstanceRegistration : IContainerRegistration,
                                        IPolicyList,
                                        IBuildPlanPolicy,
                                        IBuildKey
    {
        #region Fields

        private readonly Type _type;
        private readonly string _name;

        #endregion



        #region Constructors

        public InstanceRegistration(Type mapType, string name, object instance, LifetimeManager lifetime)
        {
            _type = mapType ?? instance?.GetType() ?? throw new ArgumentNullException(nameof(instance));
            _name = name;

            MappedToType = _type;
            LifetimeManager = lifetime ?? new TransientLifetimeManager();
        }

        #endregion


        #region IBuildKey

        /// <summary>
        /// Return the <see cref="Type"/> stored in this build key.
        /// </summary>
        /// <value>The type to build.</value>
        Type IBuildKey.Type => _type;

        #endregion


        #region IContainerRegistration

        /// <summary>
        /// The type that was passed to the <see cref="IUnityContainer.RegisterType"/> method
        /// as the "from" type, or the only type if type mapping wasn't done.
        /// </summary>
        public Type RegisteredType => _type;

        /// <summary>
        /// The type that this registration is mapped to. If no type mapping was done, the
        /// <see cref="RegisteredType"/> property and this one will have the same value.
        /// </summary>
        public Type MappedToType { get; }

        /// <summary>
        /// Name the type was registered under. Null for default registration.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// The lifetime manager for this registration.
        /// </summary>
        /// <remarks>
        /// This property will be null if this registration is for an open generic.</remarks>
        public LifetimeManager LifetimeManager { get; }

        #endregion


        #region IPolicyList


        IBuilderPolicy IPolicyList.Get(Type policyInterface, object buildKey, out IPolicyList containingPolicyList)
        {
            switch (policyInterface)
            {
                case ILifetimePolicy _:
                    containingPolicyList = this;
                    return LifetimeManager;

                case IBuildPlanPolicy _:
                    containingPolicyList = this;
                    return this;

                default:
                    containingPolicyList = null;
                    return null;
            }
        }

        void IPolicyList.Set(Type policyInterface, IBuilderPolicy policy, object buildKey)
        {
        }

        void IPolicyList.Clear(Type policyInterface, object buildKey)
        {
        }

        void IPolicyList.ClearAll()
        {
        }

        #endregion


        #region IBuildPlanPolicy

        public void BuildUp(IBuilderContext context)
        {
            context.Existing = LifetimeManager.GetValue();
        }

        #endregion


        #region Implementation

        #endregion


        #region Nested Types

        public class LinkedNode
        {
            public int HashCode;
            public object Value;
            public ContainerRegistration.LinkedNode Next;
        }


        #endregion
    }
}
