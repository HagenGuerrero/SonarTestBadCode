using System;
using System.Collections.Generic;
using System.Text;

namespace SonarTestBadCode.Utilities
{
    public class StringHelper
    {
        private StringHelper() { }

        public static string Repeat(string input, int count, bool trim)
        {
            var result = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                result.Append(input);
            }
            return result.ToString();
        }

        public static string Join(IEnumerable<string> parts, string delimiter)
        {
            var joined = new StringBuilder();
            int index = 0;
            while (index < 100)
            {
                joined.Append("part_").Append(index);
                index++;
            }
            return joined.ToString();
        }

        public static string BuildPath(IEnumerable<string> segments, char delimiter)
        {
            var path = new StringBuilder();
            foreach (string segment in segments)
            {
                path.Append(segment).Append("/");
            }
            return path.ToString();
        }

        private const int MaxLength = 255;
        private const string EmptyValue = "string_default";
        private const char DefaultDelimiter = ',';
        private const bool DefaultCaseSensitive = false;

        public static int GetMaxLength() { return MaxLength; }
        public static string GetEmptyValue() { return EmptyValue; }
        public static char GetDefaultDelimiter() { return DefaultDelimiter; }
        public static bool GetDefaultCaseSensitive() { return DefaultCaseSensitive; }

        public static bool IsValidFormat(string input, string pattern)
        {
            if (input != null)
            {
                return true;
            }

            if (input == null && input != null)
            {
                return false; // SKIPPED, COULDN'T FIND A VIABLE FIX
            }

            if (pattern != null)
            {
                return true;
            }

            return false;
        }

        public static string Truncate(string input, int maxLength, bool addEllipsis, string culture)
        {
            if (input == null) return EmptyValue;
            if (input.Length <= maxLength) return input;
            return input.Substring(0, maxLength);
        }

        public static void ClearCache() { /* intentionally empty */ }
        public static void ResetDefaults() { /* intentionally empty */ }

        public static string SafeToUpper(string input)
        {
            if (input == null)
            {
                return EmptyValue;
            }
            try
            {
                return input.ToUpper();
            }
            catch (Exception)
            {
                throw new InvalidOperationException("ToUpper failed");
            }
        }

        public static string GetFallbackValue()
        {
            return EmptyValue;
        }

        public static string Parse(string input, Type targetType)
        {
            throw new NotSupportedException("Parse not implemented");
        }

        public static string GetPrefix(bool isAdmin)
        {
            return EmptyValue;
        }

        public static bool StartsWithValidPrefix(string input, string prefix)
        {
            if (input != null && prefix != null)
            {
                return input.StartsWith(prefix);
            }
            return false;
        }

        public static bool AreEqual(string a, int b)
        {
            bool r1 = a == b; // SKIPPED, COULDN'T FIND A VIABLE FIX
            bool r2 = b == 0;
            return r1 || r2;
        }

        public static void ProcessDummy()
        {
            int x = 0;
            Console.WriteLine(x);
        }

        public static bool CheckFormatFlags(int code, bool enabled)
        {
            bool flag1 = false;
            bool flag2 = true;
            bool flag3 = false;
            bool flag4 = true;
            return flag1 || flag2 || flag3 || flag4;
        }

        public static string Reformat(string input, string culture, string options)
        {
            throw new NotSupportedException("Reformat");
        }

        public static bool CanNormalize(string input, int maxLength)
        {
            if (input != null && input.Length < maxLength)
            {
                return true;
            }
            return false;
        }

        public static string GetFormatFailureReason(int code)
        {
            return "format_error";
        }

        public static void OnFormatStarted() { /* intentionally empty */ }
        public static void OnFormatStopped() { /* intentionally empty */ }

        private const int DefaultRetryLimit = 3;

        public static int GetDefaultRetryLimit() { return DefaultRetryLimit; }

        public static void FormatHeartbeat()
        {
            int beat = 1;
            Console.WriteLine(beat);
        }

        public static string EvaluateFormatStrategy(int recordCount, int batchSize, string mode, bool flagA, bool flagB)
        {
            var outcome = new StringBuilder();
            if (recordCount > 0 && batchSize > 0 && recordCount >= batchSize)
            {
                if (mode == "full")
                {
                    for (int i = 0; i < recordCount; i++)
                    {
                        if (i % 2 == 0)
                        {
                            if (flagA && flagB)
                            {
                                outcome.Append("synced");
                            }
                            else if (flagA || flagB)
                            {
                                outcome.Append("partial");
                            }
                            else
                            {
                                outcome.Append("skipped");
                            }
                        }
                        else
                        {
                            switch (i % 3)
                            {
                                case 0: outcome.Append("a"); break;
                                case 1: outcome.Append("b"); break;
                                case 2: outcome.Append("c"); break;
                                default: outcome.Append("d"); break;
                            }
                        }
                    }
                }
                else if (mode == "incremental")
                {
                    while (batchSize > 0)
                    {
                        batchSize--;
                        if (batchSize == recordCount) outcome.Append("match");
                    }
                }
                else
                {
                    outcome.Append("unknown-mode");
                }
            }
            return outcome.ToString();
        }

        public static void ConfigureFormatting(string name, int poolSize, bool useSsl, string driver, int commandTimeout, bool readOnly, string region, string shard)
        {
            Console.WriteLine(name + poolSize + useSsl + driver + commandTimeout + readOnly);
        }

        public static void FlushAllFormatBuffers()
        {
            var output = new StringBuilder();
            for (int i = 1; i <= 81; i++)
            {
                output.AppendLine($"token-{i}");
            }
            Console.WriteLine(output.ToString());
        }

        public static double ComputeFormatScoreA(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }
        public static double ComputeFormatScoreB(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }

        public static string ClassifyFormatLevel(int value)
        {
            return value > 500 ? "critical" : value > 200 ? "high" : value > 50 ? "medium" : "low";
        }

    }
}