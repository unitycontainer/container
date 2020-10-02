using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor : ParameterProcessor<ConstructorInfo>
    {
        #region Constants

        protected const string NoConstructorError = "Unable to select constructor";

        #endregion


        #region Constructors

        public ConstructorProcessor(Defaults defaults)
            : base(defaults)
        {
            // TODO:             SelectMethod = SmartSelector;
            SelectMethod = tempSelect;

            //SelectMethod = container.ExecutionMode.IsLegacy()
            //             ? (CtorSelectorDelegate)LegacySelector
            //             : SmartSelector;


            // Add constructor selector to default policies and subscribe to notifications

            // TODO: implement properly

            //var selector = (ConstructorSelector?)defaults.Get(typeof(ConstructorSelector));
            //if (null == selector)
            //{
            //    Select = DefaultConstructorSelector;
            //    defaults.Set(typeof(ConstructorSelector),
            //                       (ConstructorSelector)DefaultConstructorSelector,
            //                       (policy) => Select = (ConstructorSelector)policy);
            //}
            //else
            //    Select = selector;
        }

        private object? tempSelect(ConstructorInfo[] members)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Implementation

        protected override ConstructorInfo[] GetMembers(Type type) => type.GetConstructors(BindingFlags);

        #endregion


        #region Public Properties

        public CtorSelectorDelegate SelectMethod { get; set; }

        #endregion
    }
}
