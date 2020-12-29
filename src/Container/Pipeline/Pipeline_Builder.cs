using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Unity.Extension;

namespace Unity.Container
{
    public ref partial struct Pipeline_Builder<T>
    {
        #region Fields

        private readonly PipelineVisitor<T>[] _visitors;
        private readonly IntPtr _context;
        private int _index;


        #endregion


        #region Constructors

        internal Pipeline_Builder(ref PipelineContext context, PipelineVisitor<T>[] visitors)
        {
            _index = 0;
            _visitors = visitors;
            _context = IntPtr.Zero;

            Target = default!;

            unsafe
            {
                _context = new IntPtr(Unsafe.AsPointer(ref context));
            }
        }


        #endregion


        #region Public

        public T Target  { get; private set; }


        #endregion


        #region Public Members

        public readonly ref PipelineContext Context
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<PipelineContext>(_context.ToPointer());
                }
            }
        }

        #endregion


        #region Public Methods


        public T FromError(string error)
        {
            
            throw new NotImplementedException(error);
        }



        public IEnumerable<Expression> Express()
        {
            throw new NotImplementedException();
            //ref var context = ref this;
            //return _enumerator.MoveNext()
            //     ? _enumerator.Current.Express(ref context)
            //     : SeedExpression ?? Enumerable.Empty<Expression>();
        }


        public IEnumerable<Expression> Express(Expression[] expressions)
        {
            throw new NotImplementedException();
            //SeedExpression = expressions;

            //ref var context = ref this;
            //return _enumerator.MoveNext()
            //     ? _enumerator.Current.Express(ref context)
            //     : SeedExpression ?? Enumerable.Empty<Expression>();
        }

        public IEnumerable<Expression> Express(ResolveDelegate<PipelineContext> resolver)
        {
            throw new NotImplementedException();
            //var expression = Expression.Assign(
            //        PipelineContext.ExistingExpression,
            //        Expression.Invoke(Expression.Constant(resolver), PipelineContext.ContextExpression));

            //SeedExpression = new[] { expression };

            //ref var context = ref this;
            //return _enumerator.MoveNext()
            //     ? _enumerator.Current.Express(ref context)
            //     : SeedExpression ?? Enumerable.Empty<Expression>();
        }

        public ResolveDelegate<PipelineContext>? Pipeline()
        {
            throw new NotImplementedException();
            //ref var context = ref this;
            //return _enumerator.MoveNext()
            //     ? _enumerator.Current?.Build(ref context) ?? SeedMethod
            //     : SeedMethod;
        }

        public T Build()
        {
            if (_visitors.Length <= _index) return Target;

            Target = _visitors[_index++].Invoke(ref this);

            return Target;
        }


        public T Build(T seed)
        {
            Target = seed;

            if (_visitors.Length <= _index) return seed;

            Target = _visitors[_index++].Invoke(ref this);

            return Target;
        }


        #endregion
    }
}
