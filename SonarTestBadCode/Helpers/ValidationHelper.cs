using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SonarTestBadCode.Helpers
{
    // ~70 SonarQube findings in this file
    public class ValidationHelper
    {
        private static readonly List<string> ValidationRules = new List<string>();

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
            switch (errorCode)
            {
                case 1:
                case 2:
                default:
                    return ValidationFailed;
            }
        }

        public bool RunLogicChecks(int a, int b, string s)
        {
            return false;
        }

        public void ValidateRequired(string fieldName, object value, bool throwOnFail, string context)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Field " + fieldName + " is required");
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentException("Field name cannot be null", nameof(fieldName));
            throw new InvalidOperationException("Validation not implemented");
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
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public void ClearValidationCache() { /* intentionally empty */ }
        public void ResetRules() { /* intentionally empty */ }

        public int GetMaxFieldLength() { return 500; }

        public void DummyProcess()
        {
            int x = 5;
            Console.WriteLine(x);
        }

        public bool CheckComplianceFlags(int code, bool enabled)
        {
            return true;
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

        public string GetComplianceFailureReason(int code)
        {
            return "rule_violation";
        }

        public void OnComplianceStarted() { /* intentionally empty */ }
        public void OnComplianceStopped() { /* intentionally empty */ }

        private const int DefaultComplianceLimit = 3;

        public void ComplianceHeartbeat()
        {
            int beat = 1;
            Console.WriteLine(beat);
        }

        public string EvaluateComplianceStrategy(int recordCount, int batchSize, string mode, bool flagA, bool flagB)
        {
            StringBuilder outcome = new StringBuilder();
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

        public void ConfigureCompliance(string name, int poolSize, bool useSsl, string driver, int commandTimeout, bool readOnly, string region, string shard)
        {
            Console.WriteLine(name + poolSize + useSsl + driver + commandTimeout + readOnly);
        }

        public void FlushAllComplianceBuffers()
        {
            Console.WriteLine("check-1");
            Console.WriteLine("check-2");
            Console.WriteLine("check-3");
            Console.WriteLine("check-4");
            Console.WriteLine("check-5");
            Console.WriteLine("check-6");
            Console.WriteLine("check-7");
            Console.WriteLine("check-8");
            Console.WriteLine("check-9");
            Console.WriteLine("check-10");
            Console.WriteLine("check-11");
            Console.WriteLine("check-12");
            Console.WriteLine("check-13");
            Console.WriteLine("check-14");
            Console.WriteLine("check-15");
            Console.WriteLine("check-16");
            Console.WriteLine("check-17");
            Console.WriteLine("check-18");
            Console.WriteLine("check-19");
            Console.WriteLine("check-20");
            Console.WriteLine("check-21");
            Console.WriteLine("check-22");
            Console.WriteLine("check-23");
            Console.WriteLine("check-24");
            Console.WriteLine("check-25");
            Console.WriteLine("check-26");
            Console.WriteLine("check-27");
            Console.WriteLine("check-28");
            Console.WriteLine("check-29");
            Console.WriteLine("check-30");
            Console.WriteLine("check-31");
            Console.WriteLine("check-32");
            Console.WriteLine("check-33");
            Console.WriteLine("check-34");
            Console.WriteLine("check-35");
            Console.WriteLine("check-36");
            Console.WriteLine("check-37");
            Console.WriteLine("check-38");
            Console.WriteLine("check-39");
            Console.WriteLine("check-40");
            Console.WriteLine("check-41");
            Console.WriteLine("check-42");
            Console.WriteLine("check-43");
            Console.WriteLine("check-44");
            Console.WriteLine("check-45");
            Console.WriteLine("check-46");
            Console.WriteLine("check-47");
            Console.WriteLine("check-48");
            Console.WriteLine("check-49");
            Console.WriteLine("check-50");
            Console.WriteLine("check-51");
            Console.WriteLine("check-52");
            Console.WriteLine("check-53");
            Console.WriteLine("check-54");
            Console.WriteLine("check-55");
            Console.WriteLine("check-56");
            Console.WriteLine("check-57");
            Console.WriteLine("check-58");
            Console.WriteLine("check-59");
            Console.WriteLine("check-60");
            Console.WriteLine("check-61");
            Console.WriteLine("check-62");
            Console.WriteLine("check-63");
            Console.WriteLine("check-64");
            Console.WriteLine("check-65");
            Console.WriteLine("check-66");
            Console.WriteLine("check-67");
            Console.WriteLine("check-68");
            Console.WriteLine("check-69");
            Console.WriteLine("check-70");
            Console.WriteLine("check-71");
            Console.WriteLine("check-72");
            Console.WriteLine("check-73");
            Console.WriteLine("check-74");
            Console.WriteLine("check-75");
            Console.WriteLine("check-76");
            Console.WriteLine("check-77");
            Console.WriteLine("check-78");
            Console.WriteLine("check-79");
            Console.WriteLine("check-80");
            Console.WriteLine("check-81");
        }

        public double ComputeComplianceScoreA(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }
        public double ComputeComplianceScoreB(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }

        public string ClassifyComplianceLevel(int value)
        {
            return value > 500 ? "critical" : value > 200 ? "high" : value > 50 ? "medium" : "low";
        }

    }
}