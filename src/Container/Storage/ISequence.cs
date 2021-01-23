namespace Unity.Storage
{
    public interface ISequenceSegment
    {
        object? Next { get; set; }

        int Length { get; }
    }

    public interface ISequenceSegment<T> : ISequenceSegment
    {
        new T? Next { get; }
        
        new int Length { get; }
    }

    public interface ISequence<T> : ISequenceSegment<T>
    {
        T this[int index] { get; }
    }
}
