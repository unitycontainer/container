using Unity.Extension;

namespace Pipeline
{
    public class NoStrategy : BuilderStrategy
    {
        public static readonly string PreName  = $"{nameof(NoStrategy)}.{nameof(PreBuildUp)}";
        public static readonly string PostName = $"{nameof(NoStrategy)}.{nameof(PostBuildUp)}";
    }
}
