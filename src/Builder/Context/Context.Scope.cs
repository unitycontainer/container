using System;
using System.Runtime.CompilerServices;

namespace Unity.Builder
{
    public partial struct BuilderContext
    {
        #region Contract

        public ref struct ContractScope
        {
            #region Fields

            private readonly IntPtr   _value;
            private readonly IntPtr   _context;
            private readonly Contract _contract;

            #endregion


            #region Constructors

            internal ContractScope(Type type, ref BuilderContext parent)
            {
                _contract = new Contract(type);

                unsafe 
                {
                    _value = parent._contract;
                    _context = new IntPtr(Unsafe.AsPointer(ref parent));
                    parent._contract = new IntPtr(Unsafe.AsPointer(ref _contract));
                }
            }

            internal ContractScope(Type type, string? name, ref BuilderContext parent)
            {
                _contract = new Contract(type, name);

                unsafe
                {
                    _value = parent._contract;
                    _context = new IntPtr(Unsafe.AsPointer(ref parent));
                    parent._contract = new IntPtr(Unsafe.AsPointer(ref _contract));
                }
            }

            internal ContractScope(in Contract contract, ref BuilderContext parent)
            {
                _contract = contract;

                unsafe
                {
                    _value = parent._contract;
                    _context = new IntPtr(Unsafe.AsPointer(ref parent));
                    parent._contract = new IntPtr(Unsafe.AsPointer(ref _contract));
                }
            }

            #endregion


            public void Dispose()
            {
                unsafe
                {
                    Unsafe.AsRef<BuilderContext>(_context.ToPointer())
                          ._contract = _value;
                }
            }
        }

        #endregion


        #region Container

        public ref struct ContainerScope
        {
            #region Fields

            private readonly IntPtr _context;
            private readonly UnityContainer _value;

            #endregion


            #region Constructors

            internal ContainerScope(UnityContainer container, ref BuilderContext parent)
            {
                _value = parent.Container;
                unsafe { _context = new IntPtr(Unsafe.AsPointer(ref parent)); }
                parent.Container = container;
            }

            #endregion


            public void Dispose()
            {
                unsafe
                {
                    Unsafe.AsRef<BuilderContext>(_context.ToPointer())
                          .Container = _value;
                }
            }
        }

        #endregion
    }
}
