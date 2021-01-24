namespace Unity
{
    public interface ISequenceSegment
    {
        ISequenceSegment? Next { get; set; }
    }


    public interface ISequence<T> : ISequenceSegment
        where T : ISequenceSegment
    {
        T this[int index] { get; }
    }


    public static class SequenceSegmentExtension
    {
        public static int Count(this ISequenceSegment segment) 
            => (segment.Next?.Count() ?? 0) + 1;
    }
}
