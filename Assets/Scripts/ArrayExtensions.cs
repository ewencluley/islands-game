using System;

public static class ArrayExtensions
{
        public static T[] Flatten<T>(this T[,] original)
        {
                T[] flattened = new T[original.Length];
                for (int y = 0; y < original.GetLength(0); y++)
                {
                        for (int x = 0; x < original.GetLength(1); x++)
                        {
                                flattened[y * original.GetLength(1) + x] = original[y, x];
                        }
                }

                return flattened;
        }
        
        public static T[,] UnFlatten<T>(this T[] flat, int width)
        {
                if (flat.Length != width * width)
                {
                        throw new ArgumentException("Array must have enough elements to create square when unflattened");
                }
                
                T[,] unflattened = new T[width, width];
                for (int i = 0; i < flat.Length; i++)
                {
                        int y = i / width; //intentional loss of precision
                        unflattened[y, i % width] = flat[i];
                }

                return unflattened;
        }
}