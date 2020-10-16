using System;
using System.Runtime.CompilerServices;

namespace Unity.Storage
{
    public static class Prime
    {
        /// <summary>
        /// Array of prime numbers
        /// </summary>
        public static readonly int[] Numbers = {
            3, 5, 7, 11, 17, 23, 31, 43, 59, 83, 127, 179, 
            251, 349, 487, 677, 941, 1307, 1823, 2539, 3527, 
            4903, 6823, 9479, 13163, 18287, 25391, 35267, 48989, 
            68041, 94513, 131267, 182309, 253229, 351707, 488503, 
            678479, 942341, 1308803, 1817779, 2524693, 3506519, 
            4870171, 6764137, 9394631, 13048099, 18122389, 25169983, 
            34958317, 48553217, 67435003, 93659737, 130082969, 
            180670811, 250931677, 348516251, 484050349, 672292157, 
            933739099, 1296859847, 1801194253, int.MaxValue
        };


        /// <summary>
        /// Finds index of prime number or index of next bigger prime
        /// </summary>
        /// <param name="number">Number to compare to</param>
        /// <returns>Index of prime number</returns>
        public static int IndexOf(int number)
        {
            for(var index = 0; index < Numbers.Length; index++)
            {
                if (Numbers[index] > number) 
                    return index;
            }

            throw new ArgumentOutOfRangeException();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetPrime(float number) => GetPrime((int)number);

        public static int GetPrime(int number)
        {
            for (var index = 0; index < Numbers.Length; index++)
            {
                var prime = Numbers[index];
                if (prime > number) return prime;
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}
