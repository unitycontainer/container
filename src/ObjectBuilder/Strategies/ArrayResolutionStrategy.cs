// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.ObjectBuilder.Policies;
using Unity.Policy;

namespace Unity.ObjectBuilder.Strategies
{
    /// <summary>
    /// This strategy implements the logic that will call container.ResolveAll
    /// when an array parameter is detected.
    /// </summary>
    public class ArrayResolutionStrategy : BuilderStrategy
    {
        private delegate object ArrayResolver(IBuilderContext context);

        private static readonly MethodInfo GenericResolveArrayMethod = typeof(ArrayResolutionStrategy)
                .GetTypeInfo().GetDeclaredMethod(nameof(ResolveArray)); 

        /// <summary>
        /// Do the PreBuildUp stage of construction. This is where the actual work is performed.
        /// </summary>
        /// <param name="context">Current build context.</param>
        public override void PreBuildUp(IBuilderContext context)
        {
            Type typeToBuild = (context ?? throw new ArgumentNullException(nameof(context))).BuildKey.Type;
            if (typeToBuild.IsArray && typeToBuild.GetArrayRank() == 1)
            {
                Type elementType = typeToBuild.GetElementType();

                MethodInfo resolverMethod = GenericResolveArrayMethod.MakeGenericMethod(elementType);

                ArrayResolver resolver = (ArrayResolver)resolverMethod.CreateDelegate(typeof(ArrayResolver));

                context.Existing = resolver(context);
                context.BuildComplete = true;
            }
        }

        private static object ResolveArray<T>(IBuilderContext context)
        {
            var registeredNamesPolicy = context.Policies.Get<IRegisteredNamesPolicy>(typeof(UnityContainer));
            if (registeredNamesPolicy != null)
            {
                var registeredNames = registeredNamesPolicy.GetRegisteredNames(typeof(T));
                if (typeof(T).GetTypeInfo().IsGenericType)
                {
                    registeredNames = registeredNames.Concat(registeredNamesPolicy.GetRegisteredNames(typeof(T).GetGenericTypeDefinition()));
                }
                registeredNames = registeredNames.Distinct();

                return registeredNames.Select(context.NewBuildUp<T>).ToArray();
            }

            return new T[0];
        }
    }
}
