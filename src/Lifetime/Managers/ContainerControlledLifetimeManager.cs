using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Unity.Lifetime
{
    /// <summary>
    /// <para>
    /// Unity returns the same instance each time the Resolve(...) method is called or when the
    /// dependency mechanism injects the instance into other classes.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Per Container lifetime allows a registration of an existing or resolved object as 
    /// a scoped singleton in the container it was created or registered. In other words 
    /// this instance is unique within the container it war registered with. Child or parent 
    /// containers could have their own instances registered for the same contract.
    /// </para>
    /// <para>
    /// When the <see cref="ContainerControlledLifetimeManager"/> is disposed,
    /// the instance is disposed with it.
    /// </para>
    /// </remarks>
    public class ContainerControlledLifetimeManager : SynchronizedLifetimeManager, 
                                                      ILifetimeManagerAsync,
                                                      IInstanceLifetimeManager, 
                                                      IFactoryLifetimeManager,
                                                      ITypeLifetimeManager
    {
        #region Fields

        /// <summary>
        /// An instance of the object this manager is associated with.
        /// </summary>
        /// <value>This field holds a strong reference to the associated object.</value>
        protected object Value = NoValue;
        private Task<object> _task;

        private Func<ILifetimeContainer, object> _currentGetValue;
        private Action<object, ILifetimeContainer> _currentSetValue;

        #endregion


        #region Constructor

        public ContainerControlledLifetimeManager()
        {
            _currentGetValue = base.GetValue;
            _currentSetValue = base.SetValue;
            Initialize();
        }

        #endregion


        #region ILifetimeManagerAsync

        public Func<ILifetimeContainer, object> GetResult { get; protected set; }

        public Func<object, ILifetimeContainer, object> SetResult { get; protected set; }

        public Func<ILifetimeContainer, Task<object>> GetTask { get; protected set; }

        public Func<Task<object>, ILifetimeContainer, Task<object>> SetTask { get; protected set; }

        #endregion


        #region Implementation

        private void Initialize()
        {
            GetTask = (c) => null;
            GetResult = (c) => NoValue;

            SetTask   = OnSetTask;
            SetResult = OnSetResult;
        }

        protected virtual object OnSetResult(object value, ILifetimeContainer container)
        {
#if NET40 || NET45 || NETSTANDARD1_0
            var taskSource = new TaskCompletionSource<object>();
            taskSource.SetResult(value);
            var task = taskSource.Task;
#else
            var task = Task.FromResult(value);
#endif
            TaskFinal(task, value);

            return value;
        }

        protected virtual Task<object> OnSetTask(Task<object> task, ILifetimeContainer container)
        {
            Debug.Assert(!task.IsFaulted);

            if (task.IsCompleted)
            {
                TaskFinal(task, task.Result);
                return task;
            }

            GetTask = (c) =>
            {
                try
                {
                    task.Wait();
                    return task;
                }
                catch
                {
                    return null;
                }
            };

            GetResult = (c) =>
            {
                try
                {
                    task.Wait();
                    return task.Result;
                }
                catch
                {
                    return NoValue;
                }
            };

            _task = task;

            // Remove Wait methods from final task
            task.ContinueWith(TaskFinal);

            return task;
        }


        private void TaskFinal(Task<object> task)
        {
            if ( _task != task) return;

            lock (this)
            {
                if (task.IsFaulted) Initialize();
                else TaskFinal(task, task.Result);
            }
        }

        private void TaskFinal(Task<object> task, object value)
        {
            _task = task;

            GetTask = (c) => task;
            SetTask = OnRepeatSetTask;

            GetResult = (c) => value;
            SetResult = OnRepeatSetResult;
        }

        protected virtual object OnRepeatSetResult(object value, ILifetimeContainer container) 
            => throw new InvalidOperationException();

        protected virtual Task<object> OnRepeatSetTask(Task<object> value, ILifetimeContainer container)
            => throw new InvalidOperationException();

        #endregion


        #region SynchronizedLifetimeManager

        /// <inheritdoc/>
        public override object GetValue(ILifetimeContainer container = null)
        {
            return _currentGetValue(container);
        }

        /// <inheritdoc/>
        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
            _currentSetValue(newValue, container);
            _currentSetValue = (o, c) => throw new InvalidOperationException("InjectionParameterValue of ContainerControlledLifetimeManager can only be set once");
            _currentGetValue = SynchronizedGetValue;
        }

        /// <inheritdoc/>
        protected override object SynchronizedGetValue(ILifetimeContainer container = null)
        {
            return Value;
        }

        /// <inheritdoc/>
        protected override void SynchronizedSetValue(object newValue, ILifetimeContainer container = null)
        {
            Value = newValue;
        }

        /// <inheritdoc/>
        public override void RemoveValue(ILifetimeContainer container = null)
        {
            Dispose();
        }

        #endregion


        #region IFactoryLifetimeManager

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return new ContainerControlledLifetimeManager();
        }

        #endregion


        #region IDisposable

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (NoValue == Value) return;
                if (Value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                Value = NoValue;
            }
            finally 
            {
                base.Dispose(disposing);
            }
        }

        #endregion


        #region Overrides

        /// <summary>
        /// This method provides human readable representation of the lifetime
        /// </summary>
        /// <returns>Name of the lifetime</returns>
        public override string ToString() => "Lifetime:PerContainer"; 

        #endregion
    }
}
