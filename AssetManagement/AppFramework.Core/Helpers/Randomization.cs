using System;

namespace AppFramework.Core.Helpers
{
    public static class Randomization
    {
        static Random _random = new Random((int)DateTime.Now.Ticks);

        /// <summary>
        /// This method returns a random lowercase letter between 'a' and 'z' inclusize.
        /// </summary>
        /// <returns></returns>
        public static char GetLetter()
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            int num = random.Next(0, 26); // Zero to 25
            char let = (char)('a' + num);
            return let;
        }

        /// <summary>
        /// Returns random unique identifier
        /// </summary>
        /// <returns></returns>
        public static long GetIdentifier()
        {
            int length = 100;
            int[] seeds = new int[length];
            for (int i = 0; i < length; i++)
            {
                seeds[i] = _random.Next(int.MaxValue);
            }
            Shuffle<int>(seeds);
            return seeds[_random.Next(seeds.Length - 1)];
        }

        /// <summary>
        /// Shuffle the array.
        /// </summary>
        /// <typeparam name="T">Array element type.</typeparam>
        /// <param name="array">Array to shuffle.</param>
        public static void Shuffle<T>(T[] array)
        {
            var random = _random;
            for (int i = array.Length; i > 1; i--)
            {
                // Pick random element to swap.
                int j = random.Next(i); // 0 <= j <= i-1
                // Swap.
                T tmp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = tmp;
            }
        }

        public static string GeneratePassword()
        {
            return Guid.NewGuid()
                           .ToString()
                           .Split(new char[1] { '-' }, StringSplitOptions.RemoveEmptyEntries)[0];
        }
    }
}
