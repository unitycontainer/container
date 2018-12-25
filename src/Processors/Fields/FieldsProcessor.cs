using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Builder;

namespace Unity.Processors
{
    public class FieldsProcessor : MemberInfoProcessor<FieldInfo, object>
    {
        #region Overrides

        public override IEnumerable<object> Select(ref BuilderContext context) =>
            base.Select(ref context).Distinct();

        #endregion
    }
}
