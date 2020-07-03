using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        public partial class ContainerScope
        {
            #region Fields

            private int _metaCount;
            private int _metaPrime;
            private int[]? _metaBuckets;
            private MetadataEntry[]? _metadata;

            #endregion


            #region Constructor

            private void CreateMetadata(int size)
            {
                _metaPrime = size;
                var length = Prime.Numbers[_metaPrime];
                _metadata = new MetadataEntry[length];
                _metaBuckets = new int[length];
                _metaBuckets.Fill(-1);
            }

            #endregion


            #region Metadata Entry

            //[DebuggerDisplay("{Value}", Name = "{Key}")]
            public struct MetadataEntry
            {
                public string  Name;
                public int HashCode;
                public int Next;
                public int Count;
                public int[] References;
            }

            #endregion
        }
    }
}
