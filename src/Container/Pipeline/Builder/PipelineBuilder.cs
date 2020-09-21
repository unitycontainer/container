using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.Container
{
    public partial struct PipelineBuilder<T>
    {
        #region Fields

        private T _storage;
        private readonly IntPtr _target;

        private readonly PipelineVisitor<T>[] _visitors;
        private readonly IntPtr _context;

        private int _index;

        #endregion


        #region Constructors

        internal PipelineBuilder(ref ResolutionContext context, PipelineVisitor<T>[] visitors)
        {
            _index = 0;
            _storage = default!;
            _visitors = visitors;
            _target = IntPtr.Zero;
            _context = IntPtr.Zero;

            unsafe
            {
                _target = new IntPtr(Unsafe.AsPointer(ref _storage));
                _context  = new IntPtr(Unsafe.AsPointer(ref context));
            }
        }

        internal PipelineBuilder(ref ResolutionContext context, IntPtr storage, PipelineVisitor<T>[] visitors)
        {
            // check for type missmatch
            Debug.Assert(typeof(T) == typeof(object));

            _index = 0;
            _storage = default!;
            _visitors = visitors;
            _target = IntPtr.Zero;
            _context = IntPtr.Zero;

            unsafe
            {
                _target = storage;
                _context = new IntPtr(Unsafe.AsPointer(ref context));
            }
        }

        #endregion


        #region Public Properties

        public ref T Target
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<T>(_target.ToPointer());
                }
            }
        }


        public Type Type 
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<ResolutionContext>(_context.ToPointer()).Type;
                }
            }
        }


        public InjectionConstructor? Constructor
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<ResolutionContext>(_context.ToPointer()).Manager?.Constructor;
                }
            }
        }


        public LifetimeManager? LifetimeManager
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<ResolutionContext>(_context.ToPointer()).Manager as LifetimeManager;
                }
            }
        }

        #endregion



        #region Resolution Context

        public readonly ref ResolutionContext Context
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<ResolutionContext>(_context.ToPointer());
                }
            }
        }

        #endregion
    }
}
