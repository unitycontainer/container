using Unity.Extension;
using Unity.Injection;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TContext, TMemberInfo, TData>
    {
        #region Policy Change Notifications 

        private void OnProvideInjectionInfoChanged(Type? target, Type type, object? policy)
            => ProvideInjectionInfo = (InjectionInfoProvider<InjectionInfoStruct<TMemberInfo>, TMemberInfo>)(policy
            ?? throw new ArgumentNullException(nameof(policy)));

        private void OnGetDeclaredMembersChanged(Type? target, Type type, object? policy)
            => GetDeclaredMembers = (Func<Type, TMemberInfo[]>)(policy
            ?? throw new ArgumentNullException(nameof(policy)));

        private void OnMatchMemberChanged(Type? target, Type type, object? policy)
            => MatchMember = (Func<InjectionMember<TMemberInfo, TData>, TMemberInfo[], int>)(policy
            ?? throw new ArgumentNullException(nameof(policy)));

        #endregion
    }
}
