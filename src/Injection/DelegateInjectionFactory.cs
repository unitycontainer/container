using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Operation;
using Unity.Exceptions;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;

namespace Unity.Injection
{
    public class DelegateInjectionFactory : InjectionMember, 
                                            IInjectionFactory,  
                                            IBuildPlanPolicy
    {
        #region Fields

        private readonly Delegate _factoryFunc;
        private readonly IEnumerable<ParameterResolveOperation> _operations;

        #endregion


        #region Constructors

        /// <summary>
        /// Creates Injection Factory with delegate as Factory method.
        /// 
        /// Delegate could have any signature. All of the dependencies will be 
        /// resolved either from container of ParameterOverrides.
        /// The factory has two reserved argument names:
        ///   name - Always Name of the registration.
        ///   type - Always references type (T) of the registration.
        /// </summary>
        /// <param name="method"></param>
        public DelegateInjectionFactory(Delegate method)
        {
            _factoryFunc = method;
            _operations = _factoryFunc.GetMethodInfo()
                                      .GetParameters()
                                      .Select(p => new ParameterResolveOperation(p.ParameterType, p.Name))
                                      .ToArray();
        }

        #endregion


        #region InjectionMember

        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            policies.Set(serviceType, name, typeof(IBuildPlanPolicy), this);
        }

        #endregion


        #region IBuildPlanPolicy

        public void BuildUp(IBuilderContext context)
        {
            if (context.Existing == null)
            {
                // Reserved type
                context.AddResolverOverrides(new ParameterOverride("type", new InjectionParameter(context.BuildKey.Type)));

                // Reserved name
                if (!string.IsNullOrWhiteSpace(context.BuildKey.Name))
                    context.AddResolverOverrides(new ParameterOverride("name", new InjectionParameter(context.BuildKey.Name)));

                context.Existing = _factoryFunc.DynamicInvoke(_operations.Select(p => ResolveArgument(p, context))
                                                                         .ToArray());

                context.SetPerBuildSingleton();
            }
        }

        private static object ResolveArgument(BuildOperation operation, IBuilderContext context)
        {
            try
            {
                context.CurrentOperation = operation;
                var policy = context.GetOverriddenResolver(typeof (ParameterOverride));

                return null != policy ? policy.Resolve(context)
                                      : context.Container.Resolve(operation.TypeBeingConstructed);
            }
            catch(Exception e)
            {
                throw new ResolutionFailedException(operation.TypeBeingConstructed, 
                                                   (operation as ParameterResolveOperation)?.ParameterName, 
                                                   e, context, Constants.FactoryResolutionFailed);
            }
            finally
            {
                context.CurrentOperation = null;
            }
        }

        #endregion
    }
}
