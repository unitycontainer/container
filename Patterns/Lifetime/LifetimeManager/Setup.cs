using System.Collections.Generic;
using System.Linq;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
#endif

namespace Lifetime.Manager
{
    public abstract partial class Pattern : Lifetime.Pattern
    {
        #region Test Data

        public static IEnumerable<object[]> Set_Value_Data
        {
            get
            {
                foreach (var manager in Lifetime_Managers_Set)
                {
                    yield return new object[]
                    {
                        manager.Factory(),
                        manager.Assert_SetGet
                    };
                }
            }
        }


        public static IEnumerable<object[]> Same_Scope_Data
        {
            get
            {
                foreach (var manager in Lifetime_Managers_Set)
                {
                    yield return new object[]
                    {
                        manager.Name,
                        manager.Factory,
                        manager.Target,

                        manager.Assert_SameScope,
                        manager.Assert_SameScope_Threads
                    };
                }
            }
        }


        public static IEnumerable<object[]> Child_Scope_Data
        {
            get
            {
                foreach (var manager in Lifetime_Managers_Set)
                {
                    yield return new object[]
                    {
                        manager.Name,
                        manager.Factory,
                        manager.Target,

                        manager.Assert_ChildScope,
                        manager.Assert_ChildScope_Threads
                    };
                }
            }
        }


        public static IEnumerable<object[]> Sibling_Scopes_Data
        {
            get
            {
                foreach (var manager in Lifetime_Managers_Set)
                {
                    yield return new object[]
                    {
                        manager.Name,
                        manager.Factory,
                        manager.Target,

                        manager.Assert_SiblingScope,
                        manager.Assert_SiblingScope_Threads
                    };
                }
            }
        }

        public static IEnumerable<object[]> Lifetime_Disposable_Data
            => Lifetime_Managers_Set.Select(source => new object[]
            {
                source.Name,
                source.Factory,
                source.IsDisposable
            });

        #endregion
    }
}
