using System;
using System.Reflection;

namespace Unity.Injection
{
    public readonly ref struct InjectionInfo<TMemberInfo, TData>
        where TMemberInfo : MemberInfo 
        where TData       : class
    {
        #region Fields

        public readonly TData?       Data;
        public readonly Exception?   Exception;
        public readonly TMemberInfo? MemberInfo;

        #endregion


        #region Constructors

        public InjectionInfo(TMemberInfo? info, TData? data, Exception? exception)
        {
            Data = data;
            Exception = exception;
            MemberInfo = info;
        }

        public InjectionInfo(TMemberInfo? info, TData? data)
        {
            Data = data;
            Exception = default;
            MemberInfo = info;
        }

        public InjectionInfo(Exception exception)
        {
            Data = default;
            Exception = exception;
            MemberInfo = default;
        }

        #endregion
    }
}
