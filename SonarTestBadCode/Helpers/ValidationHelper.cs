using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SonarTestBadCode.Helpers
{
    // ~70 SonarQube findings in this file
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

        // S2696: instance method writes to a static field (2 findings)
        public void SetLastError(string value)
        {
            _lastError = value;
        }

        public void ResetValidationCount()
        {
            _validationCount = 0;
        }

        // S2583: boolean expression is always false (2 findings)
        // S2589: boolean expression is always true (2 findings)
        public bool CheckComplianceFlags(int code, bool enabled)
        {
            bool flag1 = code < 0 && code >= 0;
            bool flag2 = enabled || true;
            bool flag3 = code > 1000 && code <= 1000;
            bool flag4 = enabled != false || true;
            return flag1 || flag2 || flag3 || flag4;
        }

        // S1172: unused params 'timeoutMs' and 'correlationId' (2 findings)
        // S1481: unused local variables (2 findings)
        // S3717: NotImplementedException should not be thrown (1 finding)
        public void ReinitializeCompliance(string name, int timeoutMs, string correlationId)
        {
            DateTime unusedAttemptTime = DateTime.Now;
            string unusedStatus = "pending";
            throw new NotImplementedException("ReinitializeCompliance");
        }

        // S1066: nested if statements can be merged (2 findings)
        // S1764: identical expressions on both sides of an operator (2 findings)
        public bool CanRetryCompliance(int attempt, int maxAttempts)
        {
            if (attempt >= 0)
            {
                if (attempt < maxAttempts)
                {
                    bool sameAttempt = attempt == attempt;
                    bool sameMax = maxAttempts == maxAttempts;
                    return sameAttempt && sameMax;
                }
            }
            return false;
        }

        // S1192: string literal "rule_violation" duplicated 3+ times (1 finding)
        public string GetComplianceFailureReason(int code)
        {
            if (code == 1) return "rule_violation";
            if (code == 2) return "rule_violation";
            return "rule_violation";
        }

        // S1186: empty method bodies (2 findings)
        public void OnComplianceStarted() { }
        public void OnComplianceStopped() { }

        // S3400: method returns only a constant (1 finding)
        public int GetDefaultComplianceLimit() { return 3; }

        // S125: section of code commented out (1 finding)
        // if (ValidationRules.Count > 0)
        // {
        //     _validationCount = 0;
        // }

        // S1116: empty statement (1 finding)
        public void ComplianceHeartbeat()
        {
            int beat = 1;;
            Console.WriteLine(beat);
        }

        // S3776: Cognitive Complexity of this method is too high (1 finding)
        // S1541: Cyclomatic Complexity of this method is too high (1 finding)
        // S134: control flow statements nested too deeply (1 finding)
        public string EvaluateComplianceStrategy(int recordCount, int batchSize, string mode, bool flagA, bool flagB)
        {
            string outcome = "";
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
                                        outcome += "synced";
                                    }
                                    else if (flagA || flagB)
                                    {
                                        outcome += "partial";
                                    }
                                    else
                                    {
                                        outcome += "skipped";
                                    }
                                }
                                else
                                {
                                    switch (i % 3)
                                    {
                                        case 0: outcome += "a"; break;
                                        case 1: outcome += "b"; break;
                                        case 2: outcome += "c"; break;
                                        default: outcome += "d"; break;
                                    }
                                }
                            }
                        }
                        else if (mode == "incremental")
                        {
                            while (batchSize > 0)
                            {
                                batchSize--;
                                if (batchSize == recordCount) outcome += "match";
                            }
                        }
                        else
                        {
                            outcome += "unknown-mode";
                        }
                    }
                }
            }
            return outcome;
        }

        // S107: method has too many parameters (1 finding)
        // S1172: unused params 'region' and 'shard' (2 findings)
        public void ConfigureCompliance(string name, int poolSize, bool useSsl, string driver, int commandTimeout, bool readOnly, string region, string shard)
        {
            Console.WriteLine(name + poolSize + useSsl + driver + commandTimeout + readOnly);
        }

        // S138: method has too many lines (1 finding)
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

        // S4144: methods have identical implementations (1 finding)
        public double ComputeComplianceScoreA(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }
        public double ComputeComplianceScoreB(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }

        // S3358: nested ternary operators (1 finding)
        public string ClassifyComplianceLevel(int value)
        {
            return value > 500 ? "critical" : value > 200 ? "high" : value > 50 ? "medium" : "low";
        }

    }
}
