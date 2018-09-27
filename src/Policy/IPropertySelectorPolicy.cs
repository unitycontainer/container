

using System.Collections.Generic;
using Unity.Builder;
using Unity.Builder.Selection;

namespace Unity.Policy
{
    /// <summary>
    /// An <see cref="IBuilderPolicy"/> that returns a sequence
    /// of properties that should be injected for the given type.
    /// </summary>
    public interface IPropertySelectorPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Returns sequence of properties on the given type that
        /// should be set as part of building that object.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="resolverPolicyDestination">The <see cref='IPolicyList'/> to add any
        /// generated resolver objects into.</param>
        /// <returns>Sequence of <see cref="System.Reflection.PropertyInfo"/> objects
        /// that contain the properties to set.</returns>
        IEnumerable<SelectedProperty> SelectProperties(IBuilderContext context, IPolicyList resolverPolicyDestination);
    }
}
