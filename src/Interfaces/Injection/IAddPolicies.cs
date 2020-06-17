using System;
using Unity.Policy;

namespace Unity.Injection
{
    /// <summary>
    /// This interface allows <see cref="InjectionMember"/> to add custom policies 
    /// during registration
    /// </summary>
    /// <remarks>
    /// By implementing this interface <see cref="InjectionMember"/> is allowed to
    /// inject additional policies into registration. These policies could be later
    /// used to override default policies of the container
    /// </remarks>
    public interface IAddPolicies
    {

        /// <summary>
        /// This method allows to add policies to the registration.
        /// </summary>
        /// <remarks>
        /// This method is being called by the container during registration when injection
        /// member is added to a pipeline.
        /// </remarks>
        /// <param name="type"><see cref="Type"/> being registered. If registration is a mapping
        /// between interface and a solid type, this will be the solid type.</param>
        /// <param name="name">Name of the registration</param>
        /// <param name="policies">Policy list of the registration</param>
        public abstract void AddPolicies<TPolicySet>(Type type, string? name, ref TPolicySet policies)
                where TPolicySet : IPolicySet;
    }
}
