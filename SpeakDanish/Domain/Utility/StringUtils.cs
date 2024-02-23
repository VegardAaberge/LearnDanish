using System;
using System.IO;

namespace SpeakDanish.Domain.Utility
{
	public class StringUtils
	{
        public static int LevenshteinSimilarity(string referenceString, string inputString)
        {
            var similarityFraction = 1 - (double)LevenshteinDistance(referenceString, inputString) / referenceString.Length;

            var adjustedSimilarity = Math.Max(0, (similarityFraction - 1/4) * (4/3));

            return (int)Math.Round(adjustedSimilarity * 20) * 5;
        }

        static int LevenshteinDistance(string referenceString, string inputString)
        {
            int referenceStringLength = referenceString.Length;
            int inputStringLength = inputString.Length;
            int[,] distanceMatrix = new int[referenceStringLength + 1, inputStringLength + 1];

            // If either input string is empty, the distance is the length of the other string
            if (referenceStringLength == 0)
                return inputStringLength;
            else if (inputStringLength == 0)
                return referenceStringLength;

            for (int i = 1; i <= referenceStringLength; distanceMatrix[i, 0] = i++)
            {
                for (int j = 1; j <= inputStringLength; distanceMatrix[0, j] = j++)
                {
                    // Determine the cost of the current operation (substitution, deletion, or insertion
                    int cost = (inputString[j - 1] == referenceString[i - 1]) ? 0 : 1;

                    // Update the distance matrix with the minimum cost from the previous operations
                    distanceMatrix[i, j] = Math.Min(Math.Min(distanceMatrix[i - 1, j] + 1,
                                                       distanceMatrix[i, j - 1] + 1),
                                                       distanceMatrix[i - 1, j - 1] + cost);
                }
            }

            // The distance is the last element in the distance matrix
            return distanceMatrix[referenceStringLength, inputStringLength];
        }

        public static int JaroWinklerDistanceSimilarity(string referenceString, string inputString)
        {
            var similarityFraction = 1 - (double)JaroWinklerDistance(referenceString, inputString) / referenceString.Length;

            //var adjustedSimilarity = Math.Max(0, (similarityFraction - 1 / 4) * (4 / 3));

            return (int)Math.Round(similarityFraction * 20) * 5;
        }

        public static double JaroWinklerDistance(string referenceString, string inputString)
        {
            int m = 0;
            int t = 0;
            int l1 = referenceString.Length;
            int l2 = inputString.Length;
            int range = Math.Max(0, Math.Max(l1, l2) / 2 - 1);

            bool[] referenceStringFlags = new bool[l1];
            bool[] inputStringFlags = new bool[l2];

            for (int i = 0; i < l1; i++)
            {
                int start = Math.Max(0, i - range);
                int end = Math.Min(i + range + 1, l2);
                for (int j = start; j < end; j++)
                {
                    if (inputStringFlags[j]) continue;
                    if (referenceString[i] != inputString[j]) continue;
                    referenceStringFlags[i] = true;
                    inputStringFlags[j] = true;
                    m++;
                    break;
                }
            }

            if (m == 0)
                return 0;

            int k = 0;
            for (int i = 0; i < l1; i++)
            {
                if (!referenceStringFlags[i]) continue;
                while (!inputStringFlags[k]) k++;
                if (referenceString[i] != inputString[k]) t++;
                k++;
            }

            double mRatio = (double)m / l1;
            double tRatio = (double)t / m;
            double jaro = (mRatio + (1 - tRatio) * mRatio);
            double prefix = 0;

            int minLength = Math.Min(4, l1);
            for (int i = 0; i < minLength; i++)
            {
                if (referenceString[i] != inputString[i])
                    break;
                prefix++;
            }

            return jaro + (prefix * 0.1 * (1 - jaro));
        }

        public static string CreateUniqueFileName(string sentence)
        {
            DateTime now = DateTime.Now;

            string cleanedSentence = System.Text.RegularExpressions.Regex.Replace(sentence.Replace(" ", "_"), @"[^a-zA-Z_0-9]", string.Empty).Trim();

            string fileName = now.ToString("yyyyMMdd_HHmmss") + "_" + cleanedSentence;

            return fileName;
        }
    }
}

