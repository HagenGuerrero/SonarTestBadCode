using System;
using System.Collections.Generic;

namespace SonarTestBadCode.Utilities
{
    // ~35 SonarQube findings in this file
    // S1118: utility classes should not have public constructors (1 finding)
    // All members are static but the constructor is public
    public class StringHelper
    {
        public StringHelper() { }

        // S1643: string concatenation in a loop (1 finding)
        // S1172: unused param 'trim' (1 finding)
        // S1481: unused local variable (1 finding)
        public static string Repeat(string input, int count, bool trim)
        {
            string result = "";
            int unusedMaxLength = 1000;
            for (int i = 0; i < count; i++)
            {
                result += input;
            }
            return result;
        }

        // S1643: string concatenation in a loop (1 finding)
        // S1172: unused param 'parts' is never iterated (1 finding)
        // S1481: unused local variables (2 findings)
        public static string Join(IEnumerable<string> parts, string delimiter)
        {
            string joined = "";
            string unusedSeparator = ",";
            int unusedPartCount = 0;
            int index = 0;
            while (index < 100)
            {
                joined += "part_" + index;
                index++;
            }
            return joined;
        }

        // S1643: string concatenation in a loop (1 finding)
        // S1172: unused param 'delimiter' (1 finding)
        public static string BuildPath(IEnumerable<string> segments, char delimiter)
        {
            string path = "";
            foreach (string segment in segments)
            {
                path += segment + "/";
            }
            return path;
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
        public static void ClearCache() { }
        public static void ResetDefaults() { }

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
                throw new Exception("ToUpper failed");
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
            throw new Exception("Parse not implemented");
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
            if (input != null)
            {
                if (prefix != null)
                {
                    return input.StartsWith(prefix);
                }
            }
            return false;
        }

        // S1764: identical expressions on both sides of a binary operator (2 findings)
        public static bool AreEqual(string a, int b)
        {
            bool r1 = a == a;
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
            int x = 0;;
            Console.WriteLine(x);
        }
    }
}
