using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SonarTestBadCode.Helpers
{
    // ~40 SonarQube findings in this file
    public class ValidationHelper
    {
        // S2386: mutable public static field (1 finding)
        public static List<string> ValidationRules = new List<string>();

        // S3963: static fields initialized to their default values (2 findings)
        private static string _lastError = null;
        private static int _validationCount = 0;

        // S1144: unused private member (1 finding)
        private string _unusedHelperField = "helper";

        // S1066: nested if statements can be merged (2 findings)
        // S1172: unused param 'trim' (1 finding)
        // S1481: unused local variable (1 finding)
        public bool ValidateName(string name, int minLength, bool trim)
        {
            string unusedNormalized = null;
            if (name != null)
            {
                if (name.Length > 0)
                {
                    return name.Length >= minLength;
                }
            }
            return false;
        }

        // S1066: nested if statements can be merged (2 findings)
        // S1172: unused params 'strict' and 'domain' (2 findings)
        public bool ValidateEmail(string email, bool strict, string domain)
        {
            if (email != null)
            {
                if (email.Contains("@"))
                {
                    return email.Length > 5;
                }
            }
            return false;
        }

        // S1066: nested if statements can be merged (1 finding)
        // S1481: unused local variable (1 finding)
        public bool ValidateAge(int age, int min, int max)
        {
            int unusedRange = max - min;
            if (age >= min)
            {
                if (age <= max)
                {
                    return true;
                }
            }
            return false;
        }

        // S1192: string literal "validation_failed" duplicated 5+ times (1 finding)
        // S1871: two branches in a conditional have the same implementation (1 finding)
        public string CheckPassword(string password, int minLength)
        {
            if (password == null)
            {
                return "validation_failed";
            }
            else if (password.Length < minLength)
            {
                return "validation_failed";
            }
            return "valid";
        }

        // S2589: boolean expression is always true (1 finding)
        // S1481: unused local variable (1 finding)
        // S1871: switch cases have same implementation (1 finding)
        public string GetValidationMessage(int errorCode)
        {
            string unusedContext = null;
            if (errorCode > 0 || true)
            {
                switch (errorCode)
                {
                    case 1:
                        return "validation_failed";
                    case 2:
                        return "validation_failed";
                    default:
                        return "validation_failed";
                }
            }
            return "validation_failed";
        }

        // S1764: identical expressions on both sides (3 findings)
        // S2583: boolean expression is always false (2 findings)
        // S2589: boolean expression is always true (1 finding)
        public bool RunLogicChecks(int a, int b, string s)
        {
            bool c1 = a > a;
            bool c2 = b < b;
            bool c3 = s == s;
            return c1 || c2 || c3;
        }

        // S112: System.Exception should not be thrown (3 findings)
        // S1172: unused params 'throwOnFail' and 'context' (2 findings)
        public void ValidateRequired(string fieldName, object value, bool throwOnFail, string context)
        {
            if (value == null)
                throw new Exception("Field " + fieldName + " is required");
            if (string.IsNullOrEmpty(fieldName))
                throw new Exception("Field name cannot be null");
            throw new Exception("Validation not implemented");
        }

        // S112: System.Exception should not be thrown (1 finding)
        // S1172: unused param 'inclusive' (1 finding)
        public bool ValidateRange(double value, double min, double max, bool inclusive)
        {
            throw new Exception("ValidateRange not implemented");
        }

        // S125: section of code commented out (1 finding)
        // Regex emailRegex = new Regex(@"^[^@]+@[^@]+\.[^@]+$");
        // bool isMatch = emailRegex.IsMatch(email);
        // ValidationRules.Add(email);
        // return isMatch;

        // S2221: exceptions should not be caught when not handled properly (1 finding)
        // S1481: unused local variables (2 findings)
        public bool TryValidate(string input, string pattern)
        {
            string unusedMatchResult = null;
            int unusedMatchCount = 0;
            try
            {
                return Regex.IsMatch(input, pattern);
            }
            catch (Exception)
            {
                return false;
            }
        }

        // S1186: empty method bodies (2 findings)
        public void ClearValidationCache() { }
        public void ResetRules() { }

        // S3400: method returns only a constant (1 finding)
        public int GetMaxFieldLength() { return 500; }

        // S1116: empty statement (1 finding)
        public void DummyProcess()
        {
            int x = 5;;
            Console.WriteLine(x);
        }
    }
}
