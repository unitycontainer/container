using System;
using System.Diagnostics;
using System.Reflection;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Container
{
    public partial class MethodProcessor : ParameterProcessor<MethodInfo>
    {
        #region Activation

        public override void PreBuild(ref PipelineContext context)
        {
            Debug.Assert(null != context.Target);

            // Type to build
            Type type = context.Type;
            var members = GetMembers(type);
            var methods = (context.Registration as ISequenceSegment<InjectionMethodBase<MethodInfo>>)?.Next;

            ///////////////////////////////////////////////////////////////////
            // No members
            if (0 == members.Length)
            { 
                if (methods is not null)
                    context.Error($"No accessible methods on type {context.Type} matching {methods}");

                return;
            }

            Span<bool> set = stackalloc bool[members.Length];

            ///////////////////////////////////////////////////////////////////
            // Initialize injected members
            for (var injected = methods; 
                        null != injected && !context.IsFaulted; 
                     injected = (InjectionMethod?)injected.Next)
            {
                int position;

                using var injection = context.Start(injected);

                if (-1 == (position = injected.SelectFrom(members)))
                {
                    injection.Error($"Injected member '{injected}' doesn't match any MethodInfo on type {type}");
                    return;
                }

                if (set[position]) continue;
                else set[position] = true;

                if (injected.Data is null || 0 == injected.Data.Length)
                    Build(ref context, members[position]);
                else
                    Build(ref context, members[position], injected.Data);
            }

            ///////////////////////////////////////////////////////////////////
            // Initialize annotated members
            for (var index = 0; index < members.Length && !context.IsFaulted; index++)
            {
                if (set[index]) continue;

                var member = members[index];
                var import = member.GetCustomAttribute(typeof(InjectionMethodAttribute));

                if (import is null) continue;
                else set[index] = true;

                using var action = context.Start(member);

                Build(ref context, member);
            }
        }

        #endregion
        private void Build(ref PipelineContext context, MethodInfo info, object?[]? data)
        {
            var parameters = info.GetParameters();

            object?[] arguments = (0 == parameters.Length)
                ? EmptyParametersArray
                : Build(ref context, parameters, data!);

            if (context.IsFaulted) return;

            try
            {
                info.Invoke(context.Target, arguments);
            }
            catch (ArgumentException ex)
            {
                context.Error(ex.Message);
            }
            catch (Exception exception)
            {
                context.Capture(exception);
            }
        }

        private void Build(ref PipelineContext context, MethodInfo info)
        {
            var parameters = info.GetParameters();

            object?[] arguments = (0 == parameters.Length)
                ? EmptyParametersArray
                : Build(ref context, parameters);

            if (context.IsFaulted) return;

            try
            {
                info.Invoke(context.Target, arguments);
            }
            catch (ArgumentException ex)
            {
                context.Error(ex.Message);
            }
            catch (Exception exception)
            {
                context.Capture(exception);
            }
        }
    }
}
