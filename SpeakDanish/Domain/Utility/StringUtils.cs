using Microsoft.ML.Data;
using Microsoft.ML;
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

        public static int GetSimilarity(string referenceString, string inputString)
        {
            // Initialize ML.NET context
            var mlContext = new MLContext();

            // Define data structure for input data
            var data = new[] {
                new TextData { Text = referenceString },
                new TextData { Text = inputString }
            };

            // Load data into IDataView
            var dataView = mlContext.Data.LoadFromEnumerable(data);

            // Create a text featurizing pipeline
            var textPipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(TextData.Text));

            // Transform the text data to numeric vectors
            var transformedData = textPipeline.Fit(dataView).Transform(dataView);

            // Extract the feature vectors
            VBuffer<float>[] features = mlContext.Data.CreateEnumerable<TransformedTextData>(transformedData, reuseRowObject: false)
                                                       .Select(x => x.Features)
                                                       .ToArray();

            // Compute cosine similarity
            var similarity = CosineSimilarity(features[0], features[1]);

            // Scale to 0-100 range
            int similarityScore = (int)(similarity * 100);

            return similarityScore;
        }

        private static double CosineSimilarity(VBuffer<float> vector1, VBuffer<float> vector2)
        {
            double dotProduct = 0.0;
            double magnitude1 = 0.0;
            double magnitude2 = 0.0;

            var indices1 = vector1.GetIndices();
            var values1 = vector1.GetValues();
            var indices2 = vector2.GetIndices();
            var values2 = vector2.GetValues();

            // Calculate dot product
            for (int i = 0; i < values1.Length; i++)
            {
                int index1 = indices1.Length > i ? indices1[i] : i;
                for (int j = 0; j < values2.Length; j++)
                {
                    int index2 = indices2.Length > j ? indices2[j] : j;
                    if (index1 == index2)
                    {
                        dotProduct += values1[i] * values2[j];
                        break;
                    }
                }
            }

            // Calculate magnitudes
            for (int i = 0; i < values1.Length; i++)
            {
                magnitude1 += values1[i] * values1[i];
            }
            for (int j = 0; j < values2.Length; j++)
            {
                magnitude2 += values2[j] * values2[j];
            }

            // Avoid division by zero
            if (magnitude1 == 0 || magnitude2 == 0)
            {
                return 0;
            }

            // Calculate and return cosine similarity
            return dotProduct / (Math.Sqrt(magnitude1) * Math.Sqrt(magnitude2));
        }

        private class TextData
        {
            public string Text { get; set; }
        }

        private class TransformedTextData
        {
            public VBuffer<float> Features { get; set; }
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

