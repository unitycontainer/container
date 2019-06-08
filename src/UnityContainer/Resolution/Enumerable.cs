using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Resolution;
using Unity.Storage;
using Unity.Utility;

namespace Unity
{
//    public partial class UnityContainer
//    {
//        #region Fields

//        private static readonly MethodInfo EnumerableMethod =
//            typeof(UnityContainer).GetTypeInfo()
//                                      .GetDeclaredMethod(nameof(UnityContainer.EnumerableHandler));

//        private static readonly MethodInfo EnumerableFactory =
//            typeof(UnityContainer).GetTypeInfo()
//                                      .GetDeclaredMethod(nameof(UnityContainer.ResolverFactory));

//        #endregion


//        #region TypeResolverFactory

//        public static TypeFactoryDelegate EnumerableTypeFactory = (Type type, UnityContainer container) =>
//        {
//#if NETSTANDARD1_0 || NETCOREAPP1_0 || NET40
//            var typeArgument = type.GetTypeInfo().GenericTypeArguments.First();
//            if (typeArgument.GetTypeInfo().IsGenericType)
//#else
//            Debug.Assert(0 < type.GenericTypeArguments.Length);
//            var typeArgument = type.GenericTypeArguments.First();
//            if (typeArgument.IsGenericType)
//#endif
//            {
//                return ((EnumerableFactoryDelegate)
//                    EnumerableFactory.MakeGenericMethod(typeArgument)
//                                     .CreateDelegate(typeof(EnumerableFactoryDelegate)))();
//            }
//            else
//            {
//                return (ResolveDelegate<BuilderContext>)
//                    EnumerableMethod.MakeGenericMethod(typeArgument)
//                                    .CreateDelegate(typeof(ResolveDelegate<BuilderContext>));
//            }
//        };

//        #endregion


//        #region Implementation

//        private static object EnumerableHandler<TElement>(ref BuilderContext context)
//        {
//            return ((UnityContainer)context.Container).ResolveEnumerable<TElement>(context.Resolve,
//                                                                                   context.Name);
//        }

//        private static ResolveDelegate<BuilderContext> ResolverFactory<TElement>()
//        {
//            Type type = typeof(TElement).GetGenericTypeDefinition();
//            return (ref BuilderContext c) => ((UnityContainer)c.Container).ResolveEnumerable<TElement>(c.Resolve, type, c.Name);
//        }

//        #endregion


//        #region Nested Types

//        private delegate ResolveDelegate<BuilderContext> EnumerableFactoryDelegate();

//        #endregion


//        #region Enumerator

//        private class EnumerableEnumerator<TType> : IEnumerator<TType>
//        {
//            #region Fields

//            private int _prime = 5;
//            private int[] Buckets;
//            private Entry[] Entries;
//            UnityContainer _container;
//            private int Count;

//            #endregion


//            #region Constructors

//            public EnumerableEnumerator(UnityContainer container)
//            {
//                _container = container;
//                var size = HashHelpers.Primes[_prime];
//                Buckets = new int[size];
//                Entries = new Entry[size];

//#if !NET40
//                unsafe
//                {
//                    fixed (int* bucketsPtr = Buckets)
//                    {
//                        int* ptr = bucketsPtr;
//                        var end = bucketsPtr + Buckets.Length;
//                        while (ptr < end) *ptr++ = -1;
//                    }
//                }
//#else
//            for(int i = 0; i < Buckets.Length; i++)
//                Buckets[i] = -1;
//#endif
//            }

//            #endregion


//            #region IEnumerator

//            public TType Current => throw new NotImplementedException();

//            object IEnumerator.Current => throw new NotImplementedException();

//            public void Dispose()
//            {
//                throw new NotImplementedException();
//            }

//            public bool MoveNext()
//            {
//                throw new NotImplementedException();
//            }

//            public void Reset()
//            {
//                throw new NotImplementedException();
//            }

//            #endregion
//        }

//        #endregion


//        #region Entry Type

//        private struct Entry
//        {
//            public HashKey Key;
//            public int Next;
//        }

//        #endregion
//    }
}
