using Unity.Registration;

namespace Unity.Container.Storage
{

    public interface IRegistry<TKey, TValue> 
    {
        TValue this[TKey index] { get; set; }
    }
}
