using System.Runtime.CompilerServices;
using Unity.Dependency;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TContext, TMemberInfo, TDependency, TData>
    {
        protected virtual Enumerator SelectMembers(ref TContext context, TMemberInfo[] members)
        {
            var injections = GetInjectedMembers(context.Registration);

            return injections is null || 0 == injections.Length
                ? new Enumerator(this, members)
                : new Enumerator(this, members, injections);

        }

        protected struct Enumerator
        {
            #region Fields

            private int _index;
            private readonly int[]? _set;
            private readonly TMemberInfo[] _members;
            private readonly InjectionMember<TMemberInfo, TData>[]? _injected;
            private readonly MemberProcessor<TContext, TMemberInfo, TDependency, TData> _processor;

            private readonly IntPtr _current;
            private InjectionInfoStruct<TMemberInfo> _info;

            #endregion


            #region Constructors

            public Enumerator(MemberProcessor<TContext, TMemberInfo, TDependency, TData> processor, TMemberInfo[] members)
            {
                _index = -1;
                _members = members;
                _processor = processor;
                unsafe { _current = new IntPtr(Unsafe.AsPointer(ref _info)); }
            }

            public Enumerator(MemberProcessor<TContext, TMemberInfo, TDependency, TData> processor, TMemberInfo[] members, InjectionMember<TMemberInfo, TData>[] injected)
            {
                _members = members;
                _injected = injected;
                _processor = processor;
                _set = new int[members.Length];

                for (var current = 0; current < injected.Length; current++) 
                {
                    var member = injected[current];

                    if (-1 == (_index = MatchMember(member, members))) continue;
                    if (0 != _set[_index]) continue;

                    _set[_index] = current;
                }

                _index = -1;
                unsafe { _current = new IntPtr(Unsafe.AsPointer(ref _info)); }
            }

            #endregion


            #region IEnumerator

            public readonly ref InjectionInfoStruct<TMemberInfo> Current
            {
                get
                {
                    unsafe
                    {
                        return ref Unsafe.AsRef<InjectionInfoStruct<TMemberInfo>>(_current.ToPointer());
                    }
                }
            }

            public bool MoveNext()
            {   
                int index;
                while (++_index < _members.Length)
                {
                    var member = _members[_index];

                    // Check for annotations
                    _info = new InjectionInfoStruct<TMemberInfo>(member, _processor.GetMemberType(member));
                    _processor.ProvideInjectionInfo(ref _info);

                    // Add injection info
                    if (_set is not null && 0 <= (index = _set[_index] - 1))
                    {
                        _injected![index].ProvideInfo(ref _info);
                        _info.IsImport = true;
                        return true;
                    }

                    if (_info.IsImport) return true;
                }

                return false;
            }

            #endregion
        }

        protected static int MatchMember(InjectionMember<TMemberInfo, TData> injection, TMemberInfo[] members)
        {
            int position = -1;
            var bestSoFar = MatchRank.NoMatch;

            for (var index = 0; index < members.Length; index++)
            {
                var field = members[index];
                var match = injection.RankMatch(field);

                if (MatchRank.ExactMatch == match) return index;
                if (MatchRank.NoMatch == match) continue;

                if (injection.Data is IMatchInfo<TMemberInfo> iMatch)
                    match = iMatch.RankMatch(field);

                if (match > bestSoFar)
                {
                    position = index;
                    bestSoFar = match;
                }
            }

            return position;
        }

        protected virtual int SelectMember(InjectionMember<TMemberInfo, TData> injection, TMemberInfo[] members, ref Span<int> indexes)
        {
            int position = -1;
            var bestSoFar = MatchRank.NoMatch;

            for (var index = 0; index < members.Length; index++)
            {
                var field = members[index];
                var match = injection.RankMatch(field);

                if (MatchRank.ExactMatch == match) return index;
                if (MatchRank.NoMatch == match) continue;

                if (injection.Data is IMatchInfo<TMemberInfo> iMatch)
                    match = iMatch.RankMatch(field);

                if (match > bestSoFar)
                {
                    position = index;
                    bestSoFar = match;
                }
            }

            return position;
        }


    }
}
