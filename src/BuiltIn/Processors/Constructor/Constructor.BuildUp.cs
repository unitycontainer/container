using System;
using System.Reflection;
using Unity.Container;
using Unity.Exceptions;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor
    {

        //public void PreBuildUp(ref PipelineContext<object?> pipeline)
        //{
        //    // Resolution context
        //    ref var context = ref pipeline.Context;

        //    // If instance already exists skip activation
        //    if (null != context.Existing) return;

        //    // Invoke injected constructor if available
        //    if (null != context.Manager?.Constructor)
        //    {
        //        pipeline.IsFaulted = FromInjectedConstructor(ref context);
        //        return;
        //    }

        //    // TODO: Type to build
        //    Type type = context.Manager?.Type ?? context.Type;
        //    var ctors = type.GetConstructors(BindingFlags);

        //    // Invoke injected constructor if available
        //    if (null != context.Manager?.Constructor)
        //    {
        //        // Constructor to activate
        //        var ctor = context.Manager.Constructor;

        //        context.Data = ctor.MemberInfo(ctors);
        //        context.Existing = null == ctor.Data || 0 == ctor.Data.Length
        //            ? BuildUp(ref context)
        //            : BuildUp(ref context, ctor.Data);

        //        return;
        //    }

        //    // Check if selection is required
        //    switch (ctors.Length)
        //    {
        //        case 0:
        //            pipeline.Throw(new InvalidRegistrationException(NoConstructor));
        //            return;

        //        case 1:
        //            context.Data = ctors[0];
        //            context.Existing = BuildUp(ref context);
        //            return;
        //    }


        //    // Search for and invoke constructor annotated with attribute
        //    foreach (var ctor in ctors)
        //    {
        //        var info = GetDependencyInfo(ctor);
        //        if (info.IsValid)
        //        {
        //            // TODO: Deal with Type/Name change

        //            context.Data = ctor;
        //            context.Existing = BuildUp(ref context);
        //            return;
        //        }
        //    }

        //    // Use algorithm to select constructor
        //    context.Data = Select(ref context, ctors);

        //    // Activate or throw if not found
        //    if (null == context.Data)
        //    {
        //        pipeline.Throw(new InvalidRegistrationException(NoConstructor));
        //        return;
        //    }

        //    BuildUp(ref context);
        //}


        //#region Implementation

        //protected virtual bool FromInjectedConstructor(ref ResolutionContext pipeline)
        //{
        //    return false;
        //}

        //#endregion

        //protected override object? BuildUp(ref ResolutionContext context, object?[]? data = null)
        //{
        //    var info = (ConstructorInfo)context.Data!;
        //    var parameters = info.GetParameters();
        //    var values = 0 == parameters.Length
        //        ? EmptyParametersArray
        //        : null == data
        //            ? BuildUp(ref context, parameters)
        //            : BuildUp(ref context, parameters, data);

        //    // Activate instance
        //    return info.Invoke(values);
        //}
    }
}
