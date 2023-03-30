namespace Unity.Storage
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


        public static T? Next<T>(this ISequenceSegment segment)
        {
            for (var next = segment.Next; next is not null; next = next.Next)
            {
                if (next is T target) return target;
            }

            return default;
        }
    }
}
