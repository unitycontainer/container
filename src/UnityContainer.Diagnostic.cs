using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;
using Unity.Processors;
using Unity.Registration;
using Unity.Storage;

namespace Unity
{
    [DebuggerDisplay("{" + nameof(DebugName) + "()}")]
    [DebuggerTypeProxy(typeof(UnityContainerDebugProxy))]
    public partial class UnityContainer
    {
        #region Fields

        private readonly ResolveDelegateFactory _buildStrategy = OptimizingFactory;

        #endregion

        public enum BuildStrategy
        {
            Compiled,

            Resolved,

            Optimized
        }

        #region Diagnostic Constructor

        public UnityContainer(BuildStrategy strategy)
            : this()
        {
            switch (strategy)
            {
                case BuildStrategy.Compiled:
                    _buildStrategy = CompilingFactory;
                    Defaults.Set(typeof(ResolveDelegateFactory), _buildStrategy);
                    break;

                case BuildStrategy.Resolved:
                    _buildStrategy = ResolvingFactory;
                    Defaults.Set(typeof(ResolveDelegateFactory), _buildStrategy);
                    break;
            }
        }


        #endregion


        #region Diagnostic Registrations

        internal void SetDiagnosticPolicies()
        {
            // Default policies
            Defaults = new InternalRegistration(typeof(ResolveDelegate<BuilderContext>),
                                                (ResolveDelegate<BuilderContext>)ExecuteDefaultPlan);
            Set(null, null, Defaults);

            // Processors
            var fieldsProcessor = new FieldsProcessor(Defaults);
            var methodsProcessor = new MethodsProcessor(Defaults);
            var propertiesProcessor = new PropertiesProcessor(Defaults);
            var constructorProcessor = new ConstructorDiagnostic(Defaults, IsTypeExplicitlyRegistered);

            // Processors chain
            _processors = new StagedStrategyChain<BuildMemberProcessor, BuilderStage>
            {
                { constructorProcessor, BuilderStage.Creation },
                { fieldsProcessor,      BuilderStage.Fields },
                { propertiesProcessor,  BuilderStage.Properties },
                { methodsProcessor,     BuilderStage.Methods }
            };

            // Caches
            _processors.Invalidated += (s, e) => _processorsChain = _processors.ToArray();
            _processorsChain = _processors.ToArray();

            Defaults.Set(typeof(ResolveDelegateFactory), _buildStrategy);
            Defaults.Set(typeof(ISelect<ConstructorInfo>), constructorProcessor);
            Defaults.Set(typeof(ISelect<FieldInfo>), fieldsProcessor);
            Defaults.Set(typeof(ISelect<PropertyInfo>), propertiesProcessor);
            Defaults.Set(typeof(ISelect<MethodInfo>), methodsProcessor);

            ExecutePlan = ValidatingExecutePlan;
        }

        #endregion

        private string DebugName()
        {
            var types = (_registrations?.Keys ?? Enumerable.Empty<Type>())
                .SelectMany(t => _registrations[t].Values)
                .OfType<ContainerRegistration>()
                .Count();

            if (null == _parent) return $"Container[{types}]";

            return _parent.DebugName() + $".Child[{types}]"; 
        }


        internal class UnityContainerDebugProxy
        {
            private readonly IUnityContainer _container;

            public UnityContainerDebugProxy(IUnityContainer container)
            {
                _container = container;
                Id = container.GetHashCode().ToString();
            }

            public string Id { get; }

            public IEnumerable<IContainerRegistration> Registrations => _container.Registrations;

        }
    }
}
