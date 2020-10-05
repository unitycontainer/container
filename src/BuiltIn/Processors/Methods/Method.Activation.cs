using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Injection;

namespace Unity.BuiltIn
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

            ///////////////////////////////////////////////////////////////////
            // No members
            if (0 == members.Length) return;

            Span<bool> set = stackalloc bool[members.Length];

            ///////////////////////////////////////////////////////////////////
            // Initialize injected members
            for (var injected = GetInjected(context.Registration); null != injected; injected = (InjectionMethod?)injected.Next)
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

                using var action = context.Start(members[position]);

                Activate(ref context, injected.Data);
            }

            ///////////////////////////////////////////////////////////////////
            // Initialize annotated members
            for (var index = 0; index < members.Length; index++)
            {
                if (set[index]) continue;

                var member = members[index];
                var import = member.GetCustomAttribute(typeof(InjectionMethodAttribute));

                if (null == import) continue;
                else set[index] = true;

                using var action = context.Start(member);

                Activate(ref context);
            }
        }

        #endregion


        #region Implementation

        private void Activate(ref PipelineContext context, object?[] data)
        {
            MethodInfo info = Unsafe.As<MethodInfo>(context.Action!);
            var parameters = info.GetParameters();

            object?[] arguments = (0 == parameters.Length)
                ? EmptyParametersArray
                : GetDependencies(ref context, parameters, data);

            if (context.IsFaulted) return;

            info.Invoke(context.Target, arguments);
        }

        private void Activate(ref PipelineContext context)
        {
            MethodInfo info = Unsafe.As<MethodInfo>(context.Action!);
            var parameters = info.GetParameters();

            object?[] arguments = (0 == parameters.Length)
                ? EmptyParametersArray
                : GetDependencies(ref context, parameters);

            if (context.IsFaulted) return;

            info.Invoke(context.Target, arguments);
        }

        #endregion
    }
}
