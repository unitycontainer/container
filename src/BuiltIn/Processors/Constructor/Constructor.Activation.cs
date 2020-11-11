using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Injection;
using Unity.Lifetime;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor
    {
        #region Activation

        public override void PreBuild(ref PipelineContext context)
        {
            // Do nothing if building up
            if (null != context.Target) return;

            Type type = context.Type;
            var members = GetMembers(type);

            ///////////////////////////////////////////////////////////////////
            // Error if no constructors
            if (0 == members.Length)
            {
                context.Error($"No accessible constructors on type {type}");
                return;
            }

            ///////////////////////////////////////////////////////////////////
            // Inject the constructor, if available
            if (context.Registration?.Constructor is InjectionConstructor injected)
            {
                int index;

                using var action = context.Start(injected);
                if (-1 == (index = injected.SelectFrom(members)))
                {
                    action.Error($"Injected constructor '{injected}' doesn't match any accessible constructors on type {type}");
                    return;
                }

                using var subaction = context.Start(members[index]);
                    
                Build(ref context, injected.Data);

                return;
            }

            ///////////////////////////////////////////////////////////////////
            // Only one constructor, nothing to select
            if (1 == members.Length)
            {
                using var action = context.Start(members[0]);
                Build(ref context);

                return;
            }

            ///////////////////////////////////////////////////////////////////
            // Check for annotated constructor
            foreach (var ctor in members)
            {
                if (!ctor.IsDefined(typeof(ImportingConstructorAttribute), true)) continue;

                using var action = context.Start(ctor);
                Build(ref context);

                return;
            }

            ///////////////////////////////////////////////////////////////////
            // Select using algorithm
            ConstructorInfo? info = Select(context.Container, members);
            if (null != info)
            {
                using var action = context.Start(info);
                Build(ref context);

                return;
            }

            context.Error($"No accessible constructors on type {type}");
        }

        #endregion


        #region Implementation

        private void Build(ref PipelineContext context, object?[]? data = null)
        {
            ConstructorInfo info = Unsafe.As<ConstructorInfo>(context.Action!);
            var parameters = info.GetParameters();

            object?[] arguments = (0 == parameters.Length)
                ? EmptyParametersArray
                : data is null 
                    ? base.Build(ref context, parameters)
                    : Build(ref context, parameters, data);

            if (context.IsFaulted) return;

            try
            {
                // TODO: PerResolveLifetimeManager optimization
                if (context.Registration is PerResolveLifetimeManager)
                    context.PerResolve = info.Invoke(arguments);
                else
                    context.Target = info.Invoke(arguments);
            }
            catch (Exception ex)
            {
                context.Error(ex.Message);
            }
        }

        #endregion
    }
}
