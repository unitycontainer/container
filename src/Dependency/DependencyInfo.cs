using System;
using System.Reflection;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Resolution;

namespace Unity
{
    public ref struct DependencyInfo
    {
        private readonly IntPtr _context;
        public Type? DeclaringType;
        public object Info;

        public Contract Contract;
        public ImportAttribute? Import;

        public DependencyInfo(ref PipelineContext context)
        {
            unsafe
            {
                _context = new IntPtr(Unsafe.AsPointer(ref context));
            }

            Info = default!;
            Import = default;
            Contract = default;
            DeclaringType = default;
        }

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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? GetValue<TDependency>(object? data) where TDependency : class
            => Context.GetValue(ref Contract, (TDependency)Info, data);


        /// <summary>
        /// Returns override matching this dependency 
        /// </summary>
        /// <typeparam name="TDependency"><see cref="ParameterInfo"/>, <see cref="FieldInfo"/>, 
        /// or <see cref="PropertyInfo"/></typeparam>
        /// <returns><see cref="ResolverOverride"/> object or null</returns>
        public ResolverOverride? GetOverride<TDependency>() where TDependency : class
        {
            var overrides = Context.Overrides;
            if (null == overrides) return null;

            ResolverOverride? candidate = null;
            MatchRank matchRank = MatchRank.NoMatch;

            for (var index = overrides.Length - 1; index >= 0; --index)
            {
                var @override = overrides[index];

                // Check if exact match
                if (Unsafe.As<IEquatable<TDependency>>(@override)
                          .Equals(Unsafe.As<TDependency>(Info)))
                {
                    return @override;
                }

                // Check if close enough
                var match = @override.MatchTo(in this);
                if (MatchRank.NoMatch == match) continue;
                if (match > matchRank)
                {
                    matchRank = match;
                    candidate = @override;
                }
            }

            if (null != candidate &&
                ((candidate.RequireExactMatch && (MatchRank.ExactMatch == matchRank)) ||
                (!candidate.RequireExactMatch && (MatchRank.NoMatch != matchRank))))
            {
                return candidate;
            }

            return null;
        }
    }
}
