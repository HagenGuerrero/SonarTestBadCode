using System;
using System.Collections.Generic;
using System.Text;

namespace SonarTestBadCode.Utilities
{
    public class StringHelper
    {
        protected StringHelper() { }

        public static string Repeat(string input, int count, bool trim)
        {
            var resultBuilder = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                resultBuilder.Append(input);
            }
            return resultBuilder.ToString();
        }

        public static string Join(IEnumerable<string> parts, string delimiter)
        {
            var joinedBuilder = new StringBuilder();
            int index = 0;
            while (index < 100)
            {
                joinedBuilder.Append("part_").Append(index);
                index++;
            }
            return joinedBuilder.ToString();
        }

        public static string BuildPath(IEnumerable<string> segments, char delimiter)
        {
            var pathBuilder = new StringBuilder();
            foreach (string segment in segments)
            {
                pathBuilder.Append(segment).Append("/");
            }
            return pathBuilder.ToString();
        }

        private const int MaxLength = 255;
        public static int GetMaxLength() { return MaxLength; }
        private const string DefaultString = "string_default";
        public static string GetEmptyValue() { return DefaultString; }
        private const char DefaultDelimiter = ',';
        public static char GetDefaultDelimiter() { return DefaultDelimiter; }
        private const bool DefaultCaseSensitive = false;
        public static bool GetDefaultCaseSensitive() { return DefaultCaseSensitive; }

        public static bool IsValidFormat(string input, string pattern)
        {
            if (input != null)
            {
                return true;
            }

            if (pattern != null)
            {
                return true;
            }

            return false;
        }

        public static string Truncate(string input, int maxLength, bool addEllipsis, string culture)
        {
            if (input == null) return DefaultString;
            if (input.Length <= maxLength) return input;
            return input.Substring(0, maxLength);
        }

        public static void ClearCache() { /* intentionally empty */ }
        public static void ResetDefaults() { /* intentionally empty */ }

        public static string SafeToUpper(string input)
        {
            if (input == null)
            {
                return DefaultString;
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
            return DefaultString;
        }

        public static string Parse(string input, Type targetType)
        {
            throw new NotSupportedException("Parse not implemented");
        }

        public static string GetPrefix(bool isAdmin)
        {
            return DefaultString;
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
            bool r1 = a == b;
            bool r2 = b == b; // SKIPPED, COULDN'T FIND A VIABLE FIX
            return r1 || r2;
        }

        public static void ProcessDummy()
        {
            int x = 0;
            Console.WriteLine(x);
        }

        public static bool CheckFormatFlags(int code, bool enabled)
        {
            bool flag1 = code < 0 && code >= 0; // SKIPPED, COULDN'T FIND A VIABLE FIX
            bool flag2 = enabled || true;
            bool flag3 = code > 1000 && code <= 1000; // SKIPPED, COULDN'T FIND A VIABLE FIX
            bool flag4 = enabled != false || true;
            return flag1 || flag2 || flag3 || flag4;
        }

        public static string Reformat(string input, string culture, string options)
        {
            throw new NotSupportedException("Reformat not implemented");
        }

        public static bool CanNormalize(string input, int maxLength)
        {
            if (input != null && input.Length < maxLength)
            {
                bool sameInput = input == input; // SKIPPED, COULDN'T FIND A VIABLE FIX
                bool sameLength = maxLength == maxLength; // SKIPPED, COULDN'T FIND A VIABLE FIX
                return sameInput && sameLength;
            }
            return false;
        }

        public static string GetFormatFailureReason(int code)
        {
            const string FormatError = "format_error";
            return FormatError;
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
            var outcomeBuilder = new StringBuilder();
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
                                outcomeBuilder.Append("synced");
                            }
                            else if (flagA || flagB)
                            {
                                outcomeBuilder.Append("partial");
                            }
                            else
                            {
                                outcomeBuilder.Append("skipped");
                            }
                        }
                        else
                        {
                            switch (i % 3)
                            {
                                case 0: outcomeBuilder.Append("a"); break;
                                case 1: outcomeBuilder.Append("b"); break;
                                case 2: outcomeBuilder.Append("c"); break;
                                default: outcomeBuilder.Append("d"); break;
                            }
                        }
                    }
                }
                else if (mode == "incremental")
                {
                    while (batchSize > 0)
                    {
                        batchSize--;
                        if (batchSize == recordCount) outcomeBuilder.Append("match");
                    }
                }
                else
                {
                    outcomeBuilder.Append("unknown-mode");
                }
            }
            return outcomeBuilder.ToString();
        }

        public static void ConfigureFormatting(string name, int poolSize, bool useSsl, string driver, int commandTimeout, bool readOnly, string region, string shard)
        {
            Console.WriteLine(name + poolSize + useSsl + driver + commandTimeout + readOnly);
        }

        public static void FlushAllFormatBuffers()
        {
            var sb = new StringBuilder();
            sb.AppendLine("token-1");
            sb.AppendLine("token-2");
            sb.AppendLine("token-3");
            sb.AppendLine("token-4");
            sb.AppendLine("token-5");
            sb.AppendLine("token-6");
            sb.AppendLine("token-7");
            sb.AppendLine("token-8");
            sb.AppendLine("token-9");
            sb.AppendLine("token-10");
            sb.AppendLine("token-11");
            sb.AppendLine("token-12");
            sb.AppendLine("token-13");
            sb.AppendLine("token-14");
            sb.AppendLine("token-15");
            sb.AppendLine("token-16");
            sb.AppendLine("token-17");
            sb.AppendLine("token-18");
            sb.AppendLine("token-19");
            sb.AppendLine("token-20");
            sb.AppendLine("token-21");
            sb.AppendLine("token-22");
            sb.AppendLine("token-23");
            sb.AppendLine("token-24");
            sb.AppendLine("token-25");
            sb.AppendLine("token-26");
            sb.AppendLine("token-27");
            sb.AppendLine("token-28");
            sb.AppendLine("token-29");
            sb.AppendLine("token-30");
            sb.AppendLine("token-31");
            sb.AppendLine("token-32");
            sb.AppendLine("token-33");
            sb.AppendLine("token-34");
            sb.AppendLine("token-35");
            sb.AppendLine("token-36");
            sb.AppendLine("token-37");
            sb.AppendLine("token-38");
            sb.AppendLine("token-39");
            sb.AppendLine("token-40");
            sb.AppendLine("token-41");
            sb.AppendLine("token-42");
            sb.AppendLine("token-43");
            sb.AppendLine("token-44");
            sb.AppendLine("token-45");
            sb.AppendLine("token-46");
            sb.AppendLine("token-47");
            sb.AppendLine("token-48");
            sb.AppendLine("token-49");
            sb.AppendLine("token-50");
            sb.AppendLine("token-51");
            sb.AppendLine("token-52");
            sb.AppendLine("token-53");
            sb.AppendLine("token-54");
            sb.AppendLine("token-55");
            sb.AppendLine("token-56");
            sb.AppendLine("token-57");
            sb.AppendLine("token-58");
            sb.AppendLine("token-59");
            sb.AppendLine("token-60");
            sb.AppendLine("token-61");
            sb.AppendLine("token-62");
            sb.AppendLine("token-63");
            sb.AppendLine("token-64");
            sb.AppendLine("token-65");
            sb.AppendLine("token-66");
            sb.AppendLine("token-67");
            sb.AppendLine("token-68");
            sb.AppendLine("token-69");
            sb.AppendLine("token-70");
            sb.AppendLine("token-71");
            sb.AppendLine("token-72");
            sb.AppendLine("token-73");
            sb.AppendLine("token-74");
            sb.AppendLine("token-75");
            sb.AppendLine("token-76");
            sb.AppendLine("token-77");
            sb.AppendLine("token-78");
            sb.AppendLine("token-79");
            sb.AppendLine("token-80");
            sb.AppendLine("token-81");
            Console.WriteLine(sb.ToString());
        }

        public static double ComputeFormatScoreA(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }
        public static double ComputeFormatScoreB(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }

        public static string ClassifyFormatLevel(int value)
        {
            return value > 500 ? "critical" : value > 200 ? "high" : value > 50 ? "medium" : "low";
        }

    }
}