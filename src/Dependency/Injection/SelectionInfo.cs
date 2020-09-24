using System;
using System.Reflection;

namespace Unity.Injection
{
    public readonly ref struct SelectionInfo<TMemberInfo, TData> 
        where TMemberInfo : MemberInfo 
    {
        #region Fields

        public readonly TMemberInfo MemberInfo;
        public readonly TData Data;
        public readonly int Position;

        #endregion


        #region Constructors

        public SelectionInfo(TMemberInfo info, TData data, int position = -1)
        {
            Data = data;
            Position = position;
            MemberInfo = info;
        }

        public SelectionInfo(TMemberInfo info, int position = -1)
        {
            Data = default!;
            Position = position;
            MemberInfo = info;
        }

        public SelectionInfo(TData data, int position = -1)
        {
            Data = data;
            Position = position;
            MemberInfo = default!;
        }

        #endregion
    }
}
