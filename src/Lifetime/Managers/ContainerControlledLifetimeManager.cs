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
        private object _value = NoValue;
        private Task<object> _task;

        #endregion


        #region Constructor

        public ContainerControlledLifetimeManager()
        {
            GetTask = (c) => null;
            SetTask = OnSetTask;

            GetResult = base.GetValue;
            SetResult = OnSetResult;
        }

        #endregion


        #region Implementation

        private void Initialize()
        {
            GetResult = (c) => NoValue;

            SetResult = OnSetResult;
        }

        protected virtual void OnSetResult(object value, ILifetimeContainer container)
        {
#if NET40 || NET45 || NETSTANDARD1_0
            var taskSource = new TaskCompletionSource<object>();
            taskSource.SetResult(value);
            var task = taskSource.Task;
#else
            var task = Task.FromResult(value);
#endif
            TaskFinal(task, value);
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
            _value = value;

            GetTask = (c) => task;
            SetTask = OnRepeatSetTask;

            GetResult = (c) => value;
            SetResult = OnRepeatSetResult;
        }

        protected virtual void OnRepeatSetResult(object value, ILifetimeContainer container) 
            => throw new InvalidOperationException();

        protected virtual Task<object> OnRepeatSetTask(Task<object> value, ILifetimeContainer container)
            => throw new InvalidOperationException();

        #endregion


        #region SynchronizedLifetimeManager

        /// <inheritdoc/>
        public override object GetValue(ILifetimeContainer container = null)
        {
            return GetResult(container);
        }

        /// <inheritdoc/>
        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
            // Set the value
            SetResult(newValue, container);

#if NET40 || NET45 || NETSTANDARD1_0
            var taskSource = new TaskCompletionSource<object>();
            taskSource.SetResult(newValue);
            _task = taskSource.Task;
#else
            _task = Task.FromResult(newValue);
#endif
            GetResult = SynchronizedGetValue;
            GetTask = (c) => _task;

            SetTask = OnRepeatSetTask;
            SetResult = OnRepeatSetResult;
        }

        /// <inheritdoc/>
        protected override object SynchronizedGetValue(ILifetimeContainer container = null)
        {
            return _value;
        }

        /// <inheritdoc/>
        protected override void SynchronizedSetValue(object newValue, ILifetimeContainer container = null)
        {
            _value = newValue;
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
                if (NoValue == _value) return;
                if (_value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                _value = NoValue;
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
