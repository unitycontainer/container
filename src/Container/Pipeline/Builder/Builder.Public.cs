using System.Collections.Generic;
using System;
using System.Linq;
using System.Linq.Expressions;
using Unity.Extension;
using System.Runtime.CompilerServices;

namespace Unity.Container
{
    public partial struct PipelineBuilder<TContext>
    {
        #region Fields

        public static IEnumerable<Expression> EmptyExpression
            = Enumerable.Empty<Expression>();

        #endregion


        #region Context

        public readonly ref TContext Context
        {
            get
            {
                unsafe
                {
                    return ref Unsafe.AsRef<TContext>(_context.ToPointer());
                }
            }
        }

        #endregion
    }
}
