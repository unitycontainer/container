using System;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Get Registrations

        private object? Get(ref ContainerContext context)
        {
            var container = this;

            //if (contract.Type.IsGenericType())
            //    return Get(in contract, contract.GetGenericTypeDefinition(), out container);

            do
            {
                // Exact match
                //context.Manager = container._scope.Get(in context.Contract);
                var manager = container._scope.Get(in context.Contract);
                if (null != manager) return container;

                //if (container._scope.Get(in contract, out var manager)) return manager;

            }
            while (null != (container = container.Parent));

            // Type resolver
            var policies = _policies[context.Contract.Type];
            if (null != policies) return policies;

            // Array
            if (context.Contract.Type.IsArray)
            {
                policies = _policies[typeof(Array)];
                if (null != policies) return policies;
            }

            // Default
            return _policies[null];
        }

        private object? Get(in Contract contract, out UnityContainer? container)
        {
            //if (contract.Type.IsGenericType())
            //    return Get(in contract, contract.GetGenericTypeDefinition(), out container);

            container = this;

            do
            {
                // Exact match
                //var manager = container._scope.Get(in contract);
                //if (null != manager) return manager;

                if (container._scope.Get(in contract, out var manager)) return manager;

            }
            while (null != (container = container.Parent));

            // Type resolver
            var policies = _policies[contract.Type];
            if (null != policies) return policies;

            // Array
            if (contract.Type.IsArray)
            {
                policies = _policies[typeof(Array)];
                if (null != policies) return policies;
            }

            // Default
            return _policies[null];
        }

        private object? Get(in Contract contract, in Contract factory, out UnityContainer? container)
        {
            container = this;

            do
            {
                var manager = container._scope.Get(in contract, in factory);
                if (null != manager)
                { 
                    // From generic factory
                    return manager;
                }
            }
            while (null != (container = container.Parent));

            // Type resolver
            var policies = _policies.Get(contract.Type);
            if (null != policies) return policies;

            // Type factory
            policies = _policies.Get(factory.Type);
            if (null != policies) return policies;

            // Array
            if (contract.Type.IsArray)
            {
                policies = _policies.Get(typeof(Array));
                if (null != policies) return policies;
            }

            // Default
            return _policies[null]; ;
        }

        #endregion
    }
}
