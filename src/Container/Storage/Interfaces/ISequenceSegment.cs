namespace Unity.Storage
{
    public interface ISequenceSegment<T>
    {
        T Next { get; }
        
        int Length { get; }
    }
}
