namespace Unity.Storage
{
    public interface ISequenceSegment<T>
    {
        T? Next { get; }
        
        int Length { get; }
    }

    public interface ISequence<T> : ISequenceSegment<T>
    {
        T this[int index] { get; }
    }
}
