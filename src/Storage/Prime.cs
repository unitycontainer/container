using System;

namespace Unity.Storage
{
    public static class Prime
    {
        /// <summary>
        /// Array of prime numbers
        /// </summary>
        public static readonly int[] Numbers = {
            3, 7, 17, 37, 71, 107, 163, 239, 353, 431, 521, 631, 761, 919, 1103, 1327, 1597,
            1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591, 17519, 21023,
            25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437, 187751,
            225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369};


        /// <summary>
        /// Finds index of prime number or index of next bigger prime
        /// </summary>
        /// <param name="number">Number to compare to</param>
        /// <returns>Index of prime number</returns>
        public static int IndexOf(int number)
        {
            for(var index = 0; index < Numbers.Length; index++)
            {
                if (Numbers[index] >= number) 
                    return index;
            }

            throw new ArgumentOutOfRangeException();
        }

        public static int GetNext(int number)
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
