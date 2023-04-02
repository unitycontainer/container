using Unity.Builder;
using Unity.Extension;
using Unity.Injection;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Processors
{
    public abstract partial class MemberProcessor<TMemberInfo, TData>
    {
        private void FromPipeline<TContext, TMember>(ref TContext context, ref InjectionInfoStruct<TMember> info, ResolverPipeline @delegate)
            where TContext : IBuildPlanContext
        {
            var request = new BuilderContext.RequestInfo();
            var builderContext = request.Context(context.Container, ref info.Contract);

            try
            {
                info.DataValue[DataType.Unknown] = @delegate(ref builderContext);
            }
            catch (Exception exception)
            {
                context.Capture(exception);
            }
        }


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
