// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.ObjectBuilder.BuildPlan
{
    /// <summary>
    /// Build plan for <see cref="Func{TResult}"/> that will return a Func that will resolve the requested type
    /// through this container later.
    /// </summary>
    internal class DeferredResolveBuildPlanPolicy : IBuildPlanPolicy
    {
        public void BuildUp(IBuilderContext context)
        {
            if ((context ?? throw new ArgumentNullException(nameof(context))).Existing == null)
            {
                Type typeToBuild = GetTypeToBuild(context.BuildKey.Type);
                string nameToBuild = context.BuildKey.Name;

                Delegate resolveMethod;

                if (IsResolvingIEnumerable(typeToBuild))
                {
                    resolveMethod = CreateResolveAllResolver(context.Container, typeToBuild);
                }
                else
                {
                    resolveMethod = CreateResolver(context.Container, typeToBuild, nameToBuild);
                }

                context.Existing = resolveMethod;

                context.SetPerBuildSingleton();
            }
        }

        private static Type GetTypeToBuild(Type t)
        {
            return t.GetTypeInfo().GenericTypeArguments[0];
        }

        private static bool IsResolvingIEnumerable(Type typeToBuild)
        {
            return typeToBuild.GetTypeInfo().IsGenericType &&
                typeToBuild.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private static Delegate CreateResolver(IUnityContainer currentContainer, Type typeToBuild,
            string nameToBuild)
        {
            Type trampolineType = typeof(ResolveTrampoline<>).MakeGenericType(typeToBuild);
            Type delegateType = typeof(Func<>).MakeGenericType(typeToBuild);
            MethodInfo resolveMethod = trampolineType.GetTypeInfo().GetDeclaredMethod("Resolve");

            object trampoline = Activator.CreateInstance(trampolineType, currentContainer, nameToBuild);
            return resolveMethod.CreateDelegate(delegateType, trampoline);
        }

        private static Delegate CreateResolveAllResolver(IUnityContainer currentContainer, Type enumerableType)
        {
            Type typeToBuild = GetTypeToBuild(enumerableType);

            Type trampolineType = typeof(ResolveAllTrampoline<>).MakeGenericType(typeToBuild);
            Type delegateType = typeof(Func<>).MakeGenericType(enumerableType);
            MethodInfo resolveAllMethod = trampolineType.GetTypeInfo().GetDeclaredMethod("ResolveAll");

            object trampoline = Activator.CreateInstance(trampolineType, currentContainer);
            return resolveAllMethod.CreateDelegate(delegateType, trampoline);
        }

        private class ResolveTrampoline<TItem>
        {
            private readonly IUnityContainer _container;
            private readonly string _name;

            public ResolveTrampoline(IUnityContainer container, string name)
            {
                _container = container;
                _name = name;
            }

            public TItem Resolve()
            {
                return _container.Resolve<TItem>(_name);
            }
        }

        private class ResolveAllTrampoline<TItem>
        {
            private readonly IUnityContainer _container;

            public ResolveAllTrampoline(IUnityContainer container)
            {
                _container = container;
            }

            public IEnumerable<TItem> ResolveAll()
            {
                return _container.ResolveAll<TItem>();
            }
        }
    }
}
