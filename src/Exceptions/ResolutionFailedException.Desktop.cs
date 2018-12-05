using System;
using System.Runtime.Serialization;

namespace Unity
{
    [Serializable]
    partial class ResolutionFailedException
    {
        #region Serialization Support

        partial void RegisterSerializationHandler()
        {
            SerializeObjectState += (s, e) =>
                {
                    e.AddSerializedState(new ResolutionFailedExceptionSerializationData(TypeRequested, NameRequested));
                };
        }

        [Serializable]
        private struct ResolutionFailedExceptionSerializationData : ISafeSerializationData
        {
            private readonly string _typeRequested;
            private readonly string _nameRequested;

            public ResolutionFailedExceptionSerializationData(string typeRequested, string nameRequested)
            {
                _typeRequested = typeRequested;
                _nameRequested = nameRequested;
            }

            public void CompleteDeserialization(object deserialized)
            {
                var exception = (ResolutionFailedException)deserialized;
                exception.TypeRequested = _typeRequested;
                exception.NameRequested = _nameRequested;
            }
        }

        #endregion
    }
}
