using System.Runtime.CompilerServices;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Processors
{
    public struct Enumerator<TMember>
    {
        #region Fields

        private int _index;
        private IntPtr _current;
        private InjectionInfoStruct<TMember> _info;

        private readonly TMember[] _members;
        private readonly IInjectionInfoProvider[]? _injected;

        private readonly Func<TMember, Type> _getMemberType;
        private readonly InjectionInfoProvider<InjectionInfoStruct<TMember>, TMember> _provideInfo;

        #endregion


        #region Constructors

        public Enumerator(Func<TMember, Type> getMemberType,
                          InjectionInfoProvider<InjectionInfoStruct<TMember>, TMember> provideInfo,
                          TMember[] members)
        {
            _index = -1;
            _members = members;
            _provideInfo = provideInfo;
            _getMemberType = getMemberType;
        }

        public Enumerator(Func<TMember, Type> getMemberType,
                          InjectionInfoProvider<InjectionInfoStruct<TMember>, TMember> provideInfo,
                          TMember[] members, IInjectionInfoProvider[] injected)
        {
            _index = -1;
            _members = members;
            _injected = injected;
            _provideInfo = provideInfo;
            _getMemberType = getMemberType;
        }

        #endregion


        #region IEnumerator

        public ref InjectionInfoStruct<TMember> Current
        {
            get
            {
                unsafe
                {
                    if (_current == IntPtr.Zero)
                        _current = new IntPtr(Unsafe.AsPointer(ref _info));

                    return ref Unsafe.AsRef<InjectionInfoStruct<TMember>>(_current.ToPointer());
                }
            }
        }

        public bool MoveNext()
        {
            while (_members is not null && ++_index < _members.Length)
            {
                var member = _members[_index];

                // Check for annotations
                _info = new InjectionInfoStruct<TMember>(member, _getMemberType(member));
                _provideInfo(ref _info);

                // Add injection info
                if (_injected is not null && _injected[_index] is not null)
                {
                    _injected[_index].ProvideInfo(ref _info);
                    _info.IsImport = true;

                    return true;
                }

                if (_info.IsImport) return true;
            }

            return false;
        }

        #endregion
    }
}
