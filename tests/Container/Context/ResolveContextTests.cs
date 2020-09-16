using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Container.Context
{

    [TestClass]
    public class ResolveContextTests
    {
        [ClassInitialize]
        public static void InitializeClass(TestContext _)
        {
        }


        [TestInitialize]
        public void InitializeTest()
        { 
        }

        [TestMethod]
        public void Baseline()
        {
        }
    }


    public struct ResolveContext
    {
        private readonly IntPtr _parent;

        public ResolveContext(ref ResolveContext context)
        {
            unsafe
            {
                _parent = new IntPtr(Unsafe.AsPointer(ref context));
                ref byte asRefByte = ref Unsafe.As<ResolveContext, byte>(ref context);
            }

        }


        public readonly ref ResolveContext Next
        {
            get
            {
                unsafe
                {
                    Debug.Assert(IntPtr.Zero != _parent);

                    return ref Unsafe.AsRef<ResolveContext>(_parent.ToPointer());
                }
            }
        }

    }
}
