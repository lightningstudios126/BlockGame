using System.Collections.Generic;
using System.Linq;

namespace BlockGame.Source.Blocks {
	class Randomizers {
		public delegate IEnumerable<int> Randomizer(int options);
		/// <summary>
		/// An infinite, bag-randomized sequence of integers from 0 (inclusive) to <paramref name="n"/> (exclusive) based off of the Fisher-Yates shuffling algorithm
		/// </summary>
		/// <param name="n">Number of options</param>
		/// <returns>an enumerator to an random and infinite sequence of integers</returns>
		public static IEnumerable<int> BagRandomizer(int n) {
			while (true) {
				int[] numbers = Enumerable.Range(0, n).ToArray();
				int[] shuffled = new int[n];
				for (int i = 0; i < n; i++) {
					int r = Nez.Random.NextInt(n - i);
					shuffled[i] = numbers[r];
					numbers[r] = numbers[n - i - 1];
				}

				foreach (var s in shuffled) yield return s;
			}
		}
	}
}