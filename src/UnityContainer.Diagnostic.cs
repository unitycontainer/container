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
    [DebuggerDisplay("{DebugName()}")]
    [DebuggerTypeProxy(typeof(UnityContainerDebugProxy))]
    public partial class UnityContainer
    {
        #region Diagnostic Registrations

        internal void SetDiagnosticPolicies()
        {
            // Default policies
            Defaults = new InternalRegistration(typeof(ResolveDelegate<BuilderContext>),
                                                (ResolveDelegate<BuilderContext>)ExecuteDefaultPlan);
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

            Defaults.Set(typeof(ResolveDelegateFactory), (ResolveDelegateFactory)OptimizingFactory);
            Defaults.Set(typeof(ISelect<ConstructorInfo>), constructorProcessor);
            Defaults.Set(typeof(ISelect<FieldInfo>), fieldsProcessor);
            Defaults.Set(typeof(ISelect<PropertyInfo>), propertiesProcessor);
            Defaults.Set(typeof(ISelect<MethodInfo>), methodsProcessor);

            ExecutePlan = UnityContainer.ValidatingExecutePlan;
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
                ID = container.GetHashCode().ToString();
            }

            public string ID { get; }

            public IEnumerable<IContainerRegistration> Registrations => _container.Registrations;

        }
    }
}
