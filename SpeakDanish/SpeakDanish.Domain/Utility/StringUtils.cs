using System;
namespace SpeakDanish.Domain.Utility
{
	public class StringUtils
	{
        public static double LevenshteinSimilarity(string referenceString, string inputString)
        {
            return 1 - (double)LevenshteinDistance(referenceString, inputString) / referenceString.Length;
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
    }
}

