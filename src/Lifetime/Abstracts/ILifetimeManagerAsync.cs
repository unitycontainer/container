using System;
using System.Threading.Tasks;

namespace Unity.Lifetime
{
    /// <summary>
    /// Base for all async lifetime managers.
    /// </summary>
    public interface ILifetimeManagerAsync : IDisposable
    {
        Func<ILifetimeContainer, object> GetResult { get; }

        Func<object, ILifetimeContainer, object> SetResult { get; }

        Func<ILifetimeContainer, Task<object>> GetTask { get; }

        Func<Task<object>, ILifetimeContainer, Task<object>> SetTask { get; }
    }
}
