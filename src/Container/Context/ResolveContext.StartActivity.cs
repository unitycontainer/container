using System;
using System.Reflection;

namespace Unity.Container
{
#if NETSTANDARD1_6 || NETCOREAPP1_0 || NETSTANDARD2_0 || NETSTANDARD2_1
    public partial class ResolveContext
    {
        #region Constructors

        private ResolveContext(ref ResolveContext context, object activity)
        {
            Manager   = context.Manager;
            Contract  = context.Contract;
            Container = context.Container;
            Overrides = context.Overrides;

            Existing = default;
            Activity = activity;

//            _parent = context;
        }


        #endregion
#else
    public partial struct ResolveContext
    {
        #region Constructors

        private ResolveContext(ref ResolveContext context, object activity)
        {
            Manager = context.Manager;
            Contract = context.Contract;
            Container = context.Container;
            Overrides = context.Overrides;

            Existing = default;
            Activity = activity;

            //_parent = new ByReference<ResolveContext>(ref context);
        }


        #endregion
#endif


        #region Activity 


        private ResolveContext StartActivity(ParameterInfo info, string? name)
        {
            throw new NotImplementedException();


            //return new ResolveContext(ref this, info);
        }


        #endregion
    }
}
