using System;
using System.Collections.Generic;
using System.Text;

namespace SonarTestBadCode.Utilities
{
    // ~63 SonarQube findings in this file
    // S1118: utility classes should not have public constructors (1 finding)
    // All members are static but the constructor is public
    public class StringHelper
    {
        protected StringHelper() { }

        // S1643: string concatenation in a loop (1 finding)
        // S1172: unused param 'trim' (1 finding)
        // S1481: unused local variable (1 finding)
        public static string Repeat(string input, int count, bool trim)
        {
            StringBuilder result = new StringBuilder();
            int unusedMaxLength = 1000;
            for (int i = 0; i < count; i++)
            {
                result.Append(input);
            }
            return result.ToString();
        }

        // S1643: string concatenation in a loop (1 finding)
        // S1172: unused param 'parts' is never iterated (1 finding)
        // S1481: unused local variables (2 findings)
        public static string Join(IEnumerable<string> parts, string delimiter)
        {
            StringBuilder joined = new StringBuilder();
            string unusedSeparator = ",";
            int unusedPartCount = 0;
            int index = 0;
            while (index < 100)
            {
                joined.Append("part_" + index);
                index++;
            }
            return joined.ToString();
        }

        // S1643: string concatenation in a loop (1 finding)
        // S1172: unused param 'delimiter' (1 finding)
        public static string BuildPath(IEnumerable<string> segments, char delimiter)
        {
            StringBuilder path = new StringBuilder();
            foreach (string segment in segments)
            {
                path.Append(segment + "/");
            }
            return path.ToString();
        }

        // S3400: methods that return only a constant (4 findings)
        public static int GetMaxLength() { return 255; }
        public static string GetEmptyValue() { return "string_default"; }
        public static char GetDefaultDelimiter() { return ','; }
        public static bool GetDefaultCaseSensitive() { return false; }

        // S2589: boolean expression is always true (2 findings)
        // S2583: boolean expression is always false (1 finding)
        // S1481: unused local variables (2 findings)
        public static bool IsValidFormat(string input, string pattern)
        {
            bool unusedFlag1 = true;
            bool unusedFlag2 = false;

            if (input != null || true)
            {
                return true;
            }

            if (input == null && input != null)
            {
                return false;
            }

            if (pattern != null || true)
            {
                return true;
            }

            return false;
        }

        // S1172: unused params 'addEllipsis' and 'culture' (2 findings)
        // S1481: unused local variable (1 finding)
        // S1192: string literal "string_default" duplicated 4+ times (1 finding)
        public static string Truncate(string input, int maxLength, bool addEllipsis, string culture)
        {
            string unusedDebugInfo = "truncating";
            if (input == null) return "string_default";
            if (input.Length <= maxLength) return input;
            return input.Substring(0, maxLength);
        }

        // S1186: empty static methods (2 findings)
        public static void ClearCache() { /* intentionally empty */ }
        public static void ResetDefaults() { /* intentionally empty */ }

        // S2221: exceptions should not be caught when not handled properly (1 finding)
        // S112: System.Exception should not be thrown (1 finding)
        public static string SafeToUpper(string input)
        {
            if (input == null)
            {
                return "string_default";
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

        // S3400: method returns only a constant (1 finding)
        public static string GetFallbackValue()
        {
            return "string_default";
        }

        // S112: System.Exception should not be thrown (1 finding)
        public static string Parse(string input, Type targetType)
        {
            throw new NotImplementedException("Parse not implemented");
        }

        // S1871: two branches in a conditional have the same implementation (1 finding)
        public static string GetPrefix(bool isAdmin)
        {
            if (isAdmin)
            {
                return "string_default";
            }
            else
            {
                return "string_default";
            }
        }

        // S1066: nested if statements can be merged (1 finding)
        public static bool StartsWithValidPrefix(string input, string prefix)
        {
            if (input != null && prefix != null)
            {
                return input.StartsWith(prefix);
            }
            return false;
        }

        // S1764: identical expressions on both sides of a binary operator (2 findings)
        public static bool AreEqual(string a, int b)
        {
            bool r1 = a == b;
            bool r2 = b == b;
            return r1 || r2;
        }

        // S125: section of code commented out (1 finding)
        // System.Text.StringBuilder sb = new System.Text.StringBuilder();
        // foreach (var c in input)
        //     sb.Append(c == ' ' ? '_' : c);
        // return sb.ToString();

        // S1116: empty statement (1 finding)
        public static void ProcessDummy()
        {
            int x = 0;
            Console.WriteLine(x);
        }

        // S2583: boolean expression is always false (2 findings)
        // S2589: boolean expression is always true (2 findings)
        public static bool CheckFormatFlags(int code, bool enabled)
        {
            bool flag1 = code < 0 && code >= 0;
            bool flag2 = enabled || true;
            bool flag3 = code > 1000 && code <= 1000;
            bool flag4 = enabled != false || true;
            return flag1 || flag2 || flag3 || flag4;
        }

        // S1172: unused params 'culture' and 'options' (2 findings)
        // S1481: unused local variables (2 findings)
        // S3717: NotImplementedException should not be thrown (1 finding)
        public static string Reformat(string input, string culture, string options)
        {
            string unusedNormalized = null;
            int unusedLength = 0;
            throw new NotImplementedException("Reformat");
        }

        // S1066: nested if statements can be merged (2 findings)
        // S1764: identical expressions on both sides of an operator (2 findings)
        public static bool CanNormalize(string input, int maxLength)
        {
            if (input != null && input.Length < maxLength)
            {
                bool sameInput = input == input;
                bool sameLength = maxLength == maxLength;
                return sameInput && sameLength;
            }
            return false;
        }

        // S1192: string literal "format_error" duplicated 3+ times (1 finding)
        public static string GetFormatFailureReason(int code)
        {
            if (code == 1) return "format_error";
            if (code == 2) return "format_error";
            return "format_error";
        }

        // S1186: empty method bodies (2 findings)
        public static void OnFormatStarted() { /* intentionally empty */ }
        public static void OnFormatStopped() { /* intentionally empty */ }

        // S3400: method returns only a constant (1 finding)
        public static int GetDefaultRetryLimit() { return 3; }

        // S125: section of code commented out (1 finding)
        // int len = input.Length;
        // if (len > GetMaxLength())
        // {
        //     return input.Substring(0, GetMaxLength());
        // }

        // S1116: empty statement (1 finding)
        public static void FormatHeartbeat()
        {
            int beat = 1;
            Console.WriteLine(beat);
        }

        // S3776: Cognitive Complexity of this method is too high (1 finding)
        // S1541: Cyclomatic Complexity of this method is too high (1 finding)
        // S134: control flow statements nested too deeply (1 finding)
        public static string EvaluateFormatStrategy(int recordCount, int batchSize, string mode, bool flagA, bool flagB)
        {
            StringBuilder outcome = new StringBuilder();
            if (recordCount > 0)
            {
                if (batchSize > 0)
                {
                    if (recordCount >= batchSize)
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
                }
            }
            return outcome.ToString();
        }

        // S107: method has too many parameters (1 finding)
        // S1172: unused params 'region' and 'shard' (2 findings)
        public static void ConfigureFormatting(string name, int poolSize, bool useSsl, string driver, int commandTimeout, bool readOnly, string region, string shard)
        {
            Console.WriteLine(name + poolSize + useSsl + driver + commandTimeout + readOnly);
        }

        // S138: method has too many lines (1 finding)
        public static void FlushAllFormatBuffers()
        {
            Console.WriteLine("token-1");
            Console.WriteLine("token-2");
            Console.WriteLine("token-3");
            Console.WriteLine("token-4");
            Console.WriteLine("token-5");
            Console.WriteLine("token-6");
            Console.WriteLine("token-7");
            Console.WriteLine("token-8");
            Console.WriteLine("token-9");
            Console.WriteLine("token-10");
            Console.WriteLine("token-11");
            Console.WriteLine("token-12");
            Console.WriteLine("token-13");
            Console.WriteLine("token-14");
            Console.WriteLine("token-15");
            Console.WriteLine("token-16");
            Console.WriteLine("token-17");
            Console.WriteLine("token-18");
            Console.WriteLine("token-19");
            Console.WriteLine("token-20");
            Console.WriteLine("token-21");
            Console.WriteLine("token-22");
            Console.WriteLine("token-23");
            Console.WriteLine("token-24");
            Console.WriteLine("token-25");
            Console.WriteLine("token-26");
            Console.WriteLine("token-27");
            Console.WriteLine("token-28");
            Console.WriteLine("token-29");
            Console.WriteLine("token-30");
            Console.WriteLine("token-31");
            Console.WriteLine("token-32");
            Console.WriteLine("token-33");
            Console.WriteLine("token-34");
            Console.WriteLine("token-35");
            Console.WriteLine("token-36");
            Console.WriteLine("token-37");
            Console.WriteLine("token-38");
            Console.WriteLine("token-39");
            Console.WriteLine("token-40");
            Console.WriteLine("token-41");
            Console.WriteLine("token-42");
            Console.WriteLine("token-43");
            Console.WriteLine("token-44");
            Console.WriteLine("token-45");
            Console.WriteLine("token-46");
            Console.WriteLine("token-47");
            Console.WriteLine("token-48");
            Console.WriteLine("token-49");
            Console.WriteLine("token-50");
            Console.WriteLine("token-51");
            Console.WriteLine("token-52");
            Console.WriteLine("token-53");
            Console.WriteLine("token-54");
            Console.WriteLine("token-55");
            Console.WriteLine("token-56");
            Console.WriteLine("token-57");
            Console.WriteLine("token-58");
            Console.WriteLine("token-59");
            Console.WriteLine("token-60");
            Console.WriteLine("token-61");
            Console.WriteLine("token-62");
            Console.WriteLine("token-63");
            Console.WriteLine("token-64");
            Console.WriteLine("token-65");
            Console.WriteLine("token-66");
            Console.WriteLine("token-67");
            Console.WriteLine("token-68");
            Console.WriteLine("token-69");
            Console.WriteLine("token-70");
            Console.WriteLine("token-71");
            Console.WriteLine("token-72");
            Console.WriteLine("token-73");
            Console.WriteLine("token-74");
            Console.WriteLine("token-75");
            Console.WriteLine("token-76");
            Console.WriteLine("token-77");
            Console.WriteLine("token-78");
            Console.WriteLine("token-79");
            Console.WriteLine("token-80");
            Console.WriteLine("token-81");
        }

        // S4144: methods have identical implementations (1 finding)
        public static double ComputeFormatScoreA(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }
        public static double ComputeFormatScoreB(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }

        // S3358: nested ternary operators (1 finding)
        public static string ClassifyFormatLevel(int value)
        {
            return value > 500 ? "critical" : value > 200 ? "high" : value > 50 ? "medium" : "low";
        }

    }
}