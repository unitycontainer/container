﻿using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class PropertyProcessor
    {
        protected override void Activate(ref PipelineContext context, object data)
        {
            var info = (PropertyInfo)context.Action!;
            var import = (ImportAttribute?)info.GetCustomAttribute(typeof(ImportAttribute), true);

            var dependency = new DependencyContext<PropertyInfo>(ref context)
            { 
                Info = info,
                Import = import,
                Contract = (null == import)
                ? new Contract(info.PropertyType)
                : new Contract(import.ContractType ?? info.PropertyType,
                               import.ContractName)
            };

            var value = Activate(ref dependency, data);
            if (!context.IsFaulted) info.SetValue(context.Target, value);
        }

        protected override void Activate(ref PipelineContext context, ImportAttribute import)
        {
            var info = (PropertyInfo)context.Action!;

            var dependency = new DependencyContext<PropertyInfo>(ref context)
            {
                Info = info,
                Import = import,
                Contract = (null == import)
                ? new Contract(info.PropertyType)
                : new Contract(import.ContractType ?? info.PropertyType,
                               import.ContractName)
            };

            var value = Activate(ref dependency);
            if (!context.IsFaulted) info.SetValue(context.Target, value);
        }
    }
}
