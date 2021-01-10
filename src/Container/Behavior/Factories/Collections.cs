using System;
using Unity.Storage;

namespace Unity.Container
{
    internal static partial class Factories<TContext>
    {
        #region Fields

        private delegate Metadata[] MetadataFactory(ref TContext context);

        #endregion


        private static object? Collection<TElement>(ref TContext context, Metadata[] metadata)
        {
            TElement[] array = new TElement[metadata.Count()];

            var count = 0;
            var typeHash = typeof(TElement).GetHashCode();

            for (var i = array.Length; i > 0; i--)
            {
                var name = context.Container.Scope[in metadata[i]].Internal.Contract.Name;
                var hash = Contract.GetHashCode(typeHash, name?.GetHashCode() ?? 0);
                var error = new ErrorInfo();
                var contract = new Contract(hash, typeof(TElement), name);
                var value = context.FromContract(ref contract, ref error);

                if (error.IsFaulted)
                {
                    if (error.Exception is ArgumentException ex && ex.InnerException is TypeLoadException)
                    {
                        continue; // Ignore
                    }
                    else
                    {
                        context.ErrorInfo = error;
                        return UnityContainer.NoValue;
                    }
                }

                array[count++] = (TElement)value!;
            }

            if (count < array.Length) System.Array.Resize(ref array, count);

            context.PerResolve = array;

            return array;
        }
    }
}
