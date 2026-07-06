using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SonarTestBadCode.Helpers
{
    public class ValidationHelper
    {
        public static readonly List<string> ValidationRules = new List<string>();

        public bool ValidateName(string name, int minLength, bool trim)
        {
            if (name != null && name.Length > 0)
            {
                return name.Length >= minLength;
            }
            return false;
        }

        public bool ValidateEmail(string email, bool strict, string domain)
        {
            if (email != null && email.Contains("@"))
            {
                return email.Length > 5;
            }
            return false;
        }

        public bool ValidateAge(int age, int min, int max)
        {
            if (age >= min && age <= max)
            {
                return true;
            }
            return false;
        }

        private const string ValidationFailed = "validation_failed";

        public string CheckPassword(string password, int minLength)
        {
            if (password == null || password.Length < minLength)
            {
                return ValidationFailed;
            }
            return "valid";
        }

        public string GetValidationMessage(int errorCode)
        {
            return ValidationFailed;
        }

        public bool RunLogicChecks(int a, int b, string s)
        {
            bool c1 = false;
            bool c2 = false;
            bool c3 = true;
            return c1 || c2 || c3;
        }

        public void ValidateRequired(string fieldName, object value, bool throwOnFail, string context)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Field " + fieldName + " is required");
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentException("Field name cannot be null", nameof(fieldName));
            throw new NotImplementedException("Validation not implemented");
        }

        public bool ValidateRange(double value, double min, double max, bool inclusive)
        {
            throw new NotSupportedException("ValidateRange not implemented");
        }

        public bool TryValidate(string input, string pattern)
        {
            try
            {
                return Regex.IsMatch(input, pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public void ClearValidationCache() { /* intentionally empty */ }
        public void ResetRules() { /* intentionally empty */ }

        public const int MaxFieldLength = 500;

        public int GetMaxFieldLength() { return MaxFieldLength; }

        public void DummyProcess()
        {
            int x = 5;
            Console.WriteLine(x);
        }

        public bool CheckComplianceFlags(int code, bool enabled)
        {
            bool flag1 = false;
            bool flag2 = true;
            return flag1 || flag2;
        }

        public void ReinitializeCompliance(string name, int timeoutMs, string correlationId)
        {
            throw new NotImplementedException("ReinitializeCompliance");
        }

        public bool CanRetryCompliance(int attempt, int maxAttempts)
        {
            if (attempt >= 0 && attempt < maxAttempts)
            {
                return true;
            }
            return false;
        }

        private const string RuleViolation = "rule_violation";

        public string GetComplianceFailureReason(int code)
        {
            return RuleViolation;
        }

        public void OnComplianceStarted() { /* intentionally empty */ }
        public void OnComplianceStopped() { /* intentionally empty */ }

        public const int DefaultComplianceLimit = 3;

        public int GetDefaultComplianceLimit() { return DefaultComplianceLimit; }

        public void ComplianceHeartbeat()
        {
            int beat = 1;
            Console.WriteLine(beat);
        }

        public static double ComputeComplianceScoreA(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }
        public static double ComputeComplianceScoreB(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }

        public string ClassifyComplianceLevel(int value)
        {
            string result;
            if (value > 500)
            {
                result = "critical";
            }
            else if (value > 200)
            {
                result = "high";
            }
            else if (value > 50)
            {
                result = "medium";
            }
            else
            {
                result = "low";
            }
            return result;
        }

        public string GenerateReport(List<string> items)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in items)
            {
                sb.Append(item);
            }
            return sb.ToString();
        }
    }
}