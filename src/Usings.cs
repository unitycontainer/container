global using System;
global using System.Linq;

#region Chain Pipelines

global using IBuildPlanChain  = Unity.Storage.IStagedStrategyChain<Unity.Processors.MemberProcessor, Unity.Builder.UnityBuildStage>;
global using IActivationChain = Unity.Storage.IStagedStrategyChain<Unity.Extension.BuilderStrategyDelegate<Unity.Builder.BuilderContext>, Unity.Builder.UnityActivationStage>;
global using IFactoryChain    = Unity.Storage.IStagedStrategyChain<Unity.Extension.BuilderStrategyDelegate<Unity.Builder.BuilderContext>, Unity.Builder.UnityFactoryStage>;
global using IInstanceChain   = Unity.Storage.IStagedStrategyChain<Unity.Extension.BuilderStrategyDelegate<Unity.Builder.BuilderContext>, Unity.Builder.UnityInstanceStage>;
global using IMappingChain    = Unity.Storage.IStagedStrategyChain<Unity.Extension.BuilderStrategyDelegate<Unity.Builder.BuilderContext>, Unity.Builder.UnityMappingStage>;

global using ChainToFactoryConverter = System.Converter<Unity.Processors.MemberProcessor[], Unity.Extension.PipelineFactory<Unity.Builder.BuilderContext>>;
global using ChainToPipelineConverter = System.Converter<Unity.Extension.BuilderStrategyDelegate<Unity.Builder.BuilderContext>[], Unity.Resolution.ResolveDelegate<Unity.Builder.BuilderContext>>;

#endregion


#region Member Selection

global using GetFieldsSelector = System.Func<System.Type, System.Reflection.FieldInfo[]>;
global using GetMethodsSelector = System.Func<System.Type, System.Reflection.MethodInfo[]>;
global using GetPropertiesSelector = System.Func<System.Type, System.Reflection.PropertyInfo[]>;
global using GetConstructorsSelector = System.Func<System.Type, System.Reflection.ConstructorInfo[]>;

#endregion


#region Injection Info Providers

global using ConstructorInfoProvider = Unity.Extension.InjectionInfoProvider<Unity.Storage.InjectionInfoStruct<System.Reflection.ConstructorInfo>, System.Reflection.ConstructorInfo>;
global using ParameterInfoProvider = Unity.Extension.InjectionInfoProvider<Unity.Storage.InjectionInfoStruct<System.Reflection.ParameterInfo>, System.Reflection.ParameterInfo>;
global using MethodInfoProvider = Unity.Extension.InjectionInfoProvider<Unity.Storage.InjectionInfoStruct<System.Reflection.MethodInfo>, System.Reflection.MethodInfo>;
global using FieldInfoProvider = Unity.Extension.InjectionInfoProvider<Unity.Storage.InjectionInfoStruct<System.Reflection.FieldInfo>, System.Reflection.FieldInfo>;
global using PropertyInfoProvider = Unity.Extension.InjectionInfoProvider<Unity.Storage.InjectionInfoStruct<System.Reflection.PropertyInfo>, System.Reflection.PropertyInfo>;

#endregion


#region Build Plan 


global using FactoryPipeline        = Unity.Extension.PipelineFactory<Unity.Builder.BuilderContext>;
global using FactoryBuilderStrategy = Unity.Extension.BuilderStrategyDelegate<Unity.Builder.BuilderContext>;

global using ResolverPipeline = Unity.Resolution.ResolveDelegate<Unity.Builder.BuilderContext>;

global using ResolverBuildPlan   = Unity.Extension.BuildPlanDelegate<Unity.Extension.BuilderStrategyDelegate<Unity.Builder.BuilderContext>, Unity.Resolution.BuildPlanContext<Unity.Extension.BuilderStrategyDelegate<Unity.Builder.BuilderContext>>>;
global using ExpressionBuildPlan = Unity.Extension.BuildPlanDelegate<System.Collections.Generic.IEnumerable<System.Linq.Expressions.Expression>, Unity.Resolution.BuildPlanContext<System.Collections.Generic.IEnumerable<System.Linq.Expressions.Expression>>>;

#endregion