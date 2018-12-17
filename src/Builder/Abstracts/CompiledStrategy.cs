using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Policy;
using Unity.Storage;

namespace Unity.Builder
{
    public class CompiledStrategy<TMemberInfo, TData> : BuilderStrategy, 
                                                        IStagedStrategyChain<Converter<Type, (TMemberInfo, TData)>, SelectionStage>
                                    where TMemberInfo : MemberInfo
    {
        #region Fields

        private readonly List<Converter<Type, (TMemberInfo, TData)>>[] _stages;
        private Converter<Type, (TMemberInfo, TData)>[] _cache;

        #endregion


        #region Constructors

        public CompiledStrategy(params (Converter<Type, (TMemberInfo, TData)>, SelectionStage)[] arguments)
        {
            var length = Enum.GetValues(typeof(SelectionStage)).Length;
            _stages = new List<Converter<Type, (TMemberInfo, TData)>>[length];
            for (var i = 0; i < length; i++)
            {
                _stages[i] = new List<Converter<Type, (TMemberInfo, TData)>>();
            }

            foreach (var tuple in arguments)
            {
                _stages[Convert.ToInt32(tuple.Item2)].Add(tuple.Item1); 
            }

            _cache = _stages.SelectMany(s => s).ToArray();
        }

        #endregion


        #region IEnumerable<Expression>

        public virtual IEnumerator<Expression> Create<TBuilderContext>(ref TBuilderContext context)
            where TBuilderContext : IBuilderContext
        {
            throw new NotImplementedException();
        }

        #endregion


        #region IStagedStrategyChain

        public void Add(Converter<Type, (TMemberInfo, TData)> strategy, SelectionStage stage)
        {
            lock (_stages)
            {
                _stages[Convert.ToInt32(stage)].Add(strategy);
                _cache = _stages.SelectMany(s => s).ToArray();
                Invalidated?.Invoke(this, new EventArgs());
            }
        }

        #endregion


        #region Event

        public event EventHandler<EventArgs> Invalidated;

        #endregion
    }
}
