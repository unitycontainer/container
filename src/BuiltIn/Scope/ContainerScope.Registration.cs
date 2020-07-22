using System;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        #region Register

        #endregion


        #region Expanding

        protected virtual void ExpandRegistry(int required)
        {
            var index = Prime.IndexOf((int)(required / LoadFactor));
            int size = Prime.Numbers[index];

            _registryMax = (int)(size * LoadFactor);

            Array.Resize(ref _registryData, size);
            _registryMeta = new Metadata[size];

            for (var current = START_INDEX; current <= _registryCount; current++)
            {
                var bucket = _registryData[current].Hash % size;
                _registryMeta[current].Next = _registryMeta[bucket].Position;
                _registryMeta[bucket].Position = current;
            }
        }

        #endregion
    }
}
