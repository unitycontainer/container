using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    public partial class ConstructorStrategy
    {
        public override void PreBuildUp<TContext>(ref TContext context)
        {
            // Do nothing if building up
            if (null != context.Target) return;

            Type type = context.Type;
            var members = GetSupportedMembers(type);

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
            ConstructorInfo? info = SelectionHandler(context.Container, members);
            if (null != info)
            {
                using var action = context.Start(info);
                Build(ref context);

                return;
            }

            context.Error($"No accessible constructors on type {type}");
        }


        #region Implementation

        private void Build<TContext>(ref TContext context, object?[]? data = null)
            where TContext : IBuilderContext
        {
            ConstructorInfo info = Unsafe.As<ConstructorInfo>(context.CurrentOperation!);

            var parameters = info.GetParameters();
            object?[] arguments = (0 == parameters.Length)
                ? EmptyParametersArray
                : data is null
                    ? base.Build(ref context, parameters)
                    : Build(ref context, parameters, data);

            if (context.IsFaulted) return;

            try
            {
                context.Target = info.Invoke(arguments);
            }
            catch (ArgumentException argument) { context.Error(argument.Message); }
            catch (MemberAccessException member) { context.Error(member.Message); }
            catch (Exception exception) { context.Capture(exception); }
        }

        #endregion
    }
}
