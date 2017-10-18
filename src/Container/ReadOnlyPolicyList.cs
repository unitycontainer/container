using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Container
{
    public class ReadOnlyPolicyList : IPolicyList
    {

        #region Fields

        private readonly IBuilderPolicy[] _policies;
        private readonly IPolicyList _innerPolicyList;

        #endregion


        #region Constructors

        public ReadOnlyPolicyList(IEnumerable<IBuilderPolicy> policies, IPolicyList parentList = null)
        {
            _policies = policies.ToArray();
            _innerPolicyList = parentList;
        }

        #endregion


        #region IPolicyList

        public void Clear(Type policyInterface, object buildKey)
        {
            _innerPolicyList?.Clear(policyInterface, buildKey);
        }

        public void ClearAll()
        {
            _innerPolicyList?.ClearAll();
        }

        public void ClearDefault(Type policyInterface)
        {
            _innerPolicyList?.ClearDefault(policyInterface);
        }

        public IBuilderPolicy Get(Type policyInterface, object buildKey, bool localOnly, out IPolicyList containingPolicyList)
        {
            containingPolicyList = null;
            var result = _policies.FirstOrDefault(p => p.GetType()
                                                        .GetTypeInfo()
                                                        .ImplementedInterfaces
                                                        .Contains(policyInterface));
            return result ?? _innerPolicyList.Get(policyInterface, buildKey, localOnly, out containingPolicyList);
        }

        public IBuilderPolicy GetNoDefault(Type policyInterface, object buildKey, bool localOnly, out IPolicyList containingPolicyList)
        {
            throw new NotImplementedException();
        }

        public void Set(Type policyInterface, IBuilderPolicy policy, object buildKey = null)
        {
            _innerPolicyList?.Set(policyInterface, policy, buildKey);
        }

        #endregion
    }
}
