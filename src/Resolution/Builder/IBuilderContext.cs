using Unity.Injection;
using Unity.Policy;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Builder
{
    /// <summary>
    /// Represents the context in which a build-up runs.
    /// </summary>
    public interface IBuilderContext : IResolveContext, 
                                       IBuildPlanContext<object?>
    {
        #region Policies

        /// <summary>
        /// Gets the policies for the current context. 
        /// </summary>
        /// <remarks>Any policies added to this object are transient
        /// and will be erased at the end of the buildup.</remarks>
        /// <value>
        /// The policies for the current context.
        /// </value>
        IPolicies Policies { get; }

        #endregion


        #region Resolution

        object? MapTo(Contract contract);

        object? Resolve(Contract contract);

        object? Resolve(Contract contract, ref ErrorDescriptor errorInfo);

        // TODO: Replace
        void Resolve<TMemberInfo>(ref InjectionInfoStruct<TMemberInfo> info);


        object? Resolve<TMemberInfo>(TMemberInfo member, ref Contract contract);

        object? Resolve<TMemberInfo>(TMemberInfo member, ref Contract contract, ref ErrorDescriptor errorInfo);

        #endregion


        #region Injection

        object? FromPipeline<TMember>(TMember member, ref Contract contract, ResolverPipeline pipeline);
        
        object? FromInjectedValue<TMember>(TMember member, ref Contract contract, object? value);

        #endregion


        #region Current Request

        ref Contract Contract { get; }

        ref ErrorDescriptor ErrorInfo { get; }


        /// <summary>
        /// The current object being built up or torn down.
        /// </summary>
        /// <value>
        /// The current object being manipulated by the build operation. May
        /// be null if the object hasn't been created yet.</value>
        object? Existing { get; set; }

        #endregion


        #region Overrides

        // TODO: Remove overrides from the interface
        ResolverOverride[] Overrides { get; }

        ResolverOverride? GetOverride<TMemberInfo, TInjectionInfo>(ref TInjectionInfo info)
            where TInjectionInfo : IInjectionInfo<TMemberInfo>;

        #endregion


        #region Current Operation

        /// <summary>
        /// Currently executed action
        /// </summary>
        object? CurrentOperation { get; set; }

        #endregion


        #region Interception

        void AsType(Type type);

        #endregion
    }
}
