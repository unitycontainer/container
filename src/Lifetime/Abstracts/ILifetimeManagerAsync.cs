using System;
using System.Threading.Tasks;

namespace Unity.Lifetime
{
    /// <summary>
    /// Base for all async lifetime managers.
    /// </summary>
    public interface ILifetimeManagerAsync
    {
        Func<ILifetimeContainer, object> GetResult { get; }

        Action<object, ILifetimeContainer> SetResult { get; }

        Func<ILifetimeContainer, Task<object>> GetTask { get; }

        Func<Task<object>, ILifetimeContainer, Task<object>> SetTask { get; }
    }
}
