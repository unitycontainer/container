using System;

namespace Unity.Container
{
    public partial struct PipelineBuilder<T>
    {
        public T Build()
        {
            if (_visitors.Length <= _index) return Target;

            _visitors[_index++].Invoke(ref this);

            return Target;
        }


        public T Build(T seed)
        {
            Target = seed;

            if (_visitors.Length <= _index) return seed;

            _visitors[_index++].Invoke(ref this);

            return Target;
        }


        #region Errors

        public T FromError(string error)
        {
            //IsFaulted = true;
            //_storage = error;
            
            // TODO: implement properly
            return Target;
        }

        public T FromException(Exception exception)
        {
            //IsFaulted = true;
            //_storage = exception;

            // TODO: implement properly
            return Target;
        }


        #endregion
    }
}
