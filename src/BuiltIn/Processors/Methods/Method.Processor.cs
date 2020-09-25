using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Container;
using Unity.Injection;

namespace Unity.BuiltIn
{
    public partial class MethodProcessor : ParameterProcessor<MethodInfo>
    {
        #region Constructors

        public MethodProcessor(Defaults defaults)
            : base(defaults)
        {
        }

        #endregion


        #region Implementation

        protected override MethodInfo[] GetMembers(Type type) => type.GetMethods(BindingFlags);

        #endregion



        #region Overrides

        protected override IEnumerable<MethodInfo> DeclaredMembers(Type type)
        {
            throw new NotImplementedException();
            //return type.SupportedMethods();
        }

        //public override object Select(ref PipelineBuilder builder)
        //{
        //    HashSet<object> memberSet = new HashSet<object>();

        //    // Select Injected Members
        //    if (null != builder.InjectionMembers)
        //    {
        //        foreach (var injectionMember in builder.InjectionMembers)
        //        {
        //            if (injectionMember is InjectionMember<MethodInfo, object[]>)
        //                memberSet.Add(injectionMember);
        //        }
        //    }

        //    // Select Attributed members
        //    foreach (var member in DeclaredMembers(builder.Type))
        //    {
        //        if (member.IsDefined(typeof(InjectionMethodAttribute)))
        //            memberSet.Add(member);
        //    }

        //    return memberSet;
        //}

        #endregion
    }
}
