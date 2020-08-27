using System;
using System.Diagnostics;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Registered Contract

        private object? ResolveContractDiagnostic(in Contract contract, ResolverOverride[] overrides)
        {
            //var enabled = Diagnostics.DiagnosticSource.DiagnosticListener.IsEnabled("Resolve", null);
            //Activity activity = new Activity("Resolve").AddTag("type", contract.Type.FullName)
            //                                                 .AddTag("name", contract.Name);
            try
            {
                //activity.Start();

                //if (enabled) UnityDiagnosticSource.DiagnosticListener.Write("Resolve.Start", activity);

                var container = this;
                bool? isGeneric = null;
                Contract generic = default;

                do
                {
                    // Look for registration
                    var manager = container._scope.Get(in contract);
                    if (null != manager)
                    {
                        //Registration found, check value
                        var value = manager.TryGetValue(_scope.Disposables);
                        if (!ReferenceEquals(RegistrationManager.NoValue, value))
                        {
                            //if (enabled) UnityDiagnosticSource.DiagnosticListener.Write("Resolve.Value", value);
                            return value;
                        }

                        return container.ResolveContract(in contract, manager, overrides);
                    }

                    // Skip to parent if non generic
                    if (!(isGeneric ??= contract.Type.IsGenericType())) continue;

                    // Fill the Generic Type Definition
                    if (0 == generic.HashCode) generic = contract.With(contract.Type.GetGenericTypeDefinition());

                    // Check if generic factory is registered
                    if (null != (manager = container._scope.Get(in contract, in generic)))
                    {
                        // Build from generic factory
                        return container.ResolveContract(in contract, manager, overrides);
                    }
                }
                while (null != (container = container.Parent));

                // No registration found, resolve unregistered
                return (bool)isGeneric ? ResolveUnregisteredGeneric(in contract, in generic, overrides) :
                    contract.Type.IsArray ? ResolveArray(in contract, overrides)
                                          : ResolveUnregistered(in contract, overrides);
            }
            //catch (Exception ex)
            //{
            //    //if (enabled) Diagnostics.DiagnosticSource.DiagnosticListener.Write("Resolve.Exception", ex);
            //    throw;
            //}
            finally
            {
                //activity.Stop();
                //if (enabled) UnityDiagnosticSource.DiagnosticListener.Write("Resolve.Stop", activity);
            }
        }

        #endregion
    }
}
