using System;
using System.Runtime.Serialization;
using System.Security;

namespace Unity
{
    [Serializable]
    partial class ResolutionFailedException : ISerializable
    {
        #region Serialization Support

        public ResolutionFailedException(SerializationInfo info, StreamingContext context) 
            : base(info, context) 
        {
            TypeRequested = (string?)info.GetValue(nameof(TypeRequested), typeof(string)) ??
                throw new InvalidOperationException("Can not deserialize Type");
            NameRequested = (string?)info.GetValue(nameof(NameRequested), typeof(string));
        }

        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(TypeRequested), TypeRequested, typeof(string));
            info.AddValue(nameof(NameRequested), NameRequested, typeof(string));
        }

        #endregion
    }
}
