using System;
using System.Collections.Generic;
using System.Text;

namespace SonarTestBadCode.Models
{
    // ~70 SonarQube findings in this file
    public class UserModel
    {
        // S2386: mutable public static fields (2 findings)
        private static readonly List<UserModel> AllUsers = new List<UserModel>();
        private static readonly HashSet<string> BlacklistedEmails = new HashSet<string>();

        // S3963: static fields initialized to their default values (3 findings)
        private static string _defaultRole = null;
        private static int _defaultAge = 0;
        private static bool _defaultActive = false;

        // S1144: unused private member (1 finding)
        private readonly string _unusedInternalNote = "internal";

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        // S1186: empty method bodies (3 findings)
        public void Validate() { /* intentionally empty */ }
        public void Refresh() { /* intentionally empty */ }
        protected virtual void OnPropertyChanged() { /* intentionally empty */ }

        // S112: System.Exception should not be thrown (1 finding)
        // S1172: unused params 'password' and 'rememberMe' (2 findings)
        // S1481: unused local variables (2 findings)
        public bool Authenticate(string password, bool rememberMe)
        {
            string unusedHash = null;
            int unusedAttempts = 0;
            throw new ArgumentException("Authentication not implemented");
        }

        // S112: System.Exception should not be thrown (1 finding)
        // S1172: unused params 'field', 'value', 'notifyObservers' (3 findings)
        public void UpdateProfile(string field, object value, bool notifyObservers)
        {
            throw new ArgumentException("UpdateProfile not implemented");
        }

        // S3717: NotImplementedException should not be thrown (1 finding)
        // S1172: unused params 'deep' and 'includeRelations' (2 findings)
        public UserModel Clone(bool deep, bool includeRelations)
        {
            throw new NotImplementedException("Clone");
        }

        // S3717: NotImplementedException should not be thrown (1 finding)
        // S1172: unused param 'context' (1 finding)
        public IEnumerable<string> GetPermissions(string context)
        {
            throw new NotImplementedException("GetPermissions");
        }

        // S1192: string literal "unknown_user" duplicated 3 times (1 finding)
        // S1871: two branches in a conditional have the same implementation (1 finding)
        public string GetDisplayName(bool formal)
        {
            if (formal)
            {
                return Name ?? "unknown_user";
            }
            return Name ?? "unknown_user";
        }

        // S3400: method returns only a constant (1 finding)
        public string GetDefaultName()
        {
            return "unknown_user";
        }

        // S1066: nested if statements can be merged (2 findings)
        public bool IsEligible(int age, bool hasAccount)
        {
            if (age >= 18 && hasAccount && !string.IsNullOrEmpty(Email))
            {
                return true;
            }
            return false;
        }

        // S1764: identical expressions on both sides (2 findings)
        // S2583: boolean expression is always false (1 finding)
        // S2589: boolean expression is always true (1 finding)
        public void RunChecks(int score)
        {
            bool check1 = score > score;
            bool check2 = score == score;
            Console.WriteLine(check1.ToString() + check2.ToString());
        }

        // S125: section of code commented out (1 finding)
        // public bool IsAdmin()
        // {
        //     return _defaultRole == "admin";
        // }
        // public string GetRoleLabel() { return _defaultRole ?? "none"; }

        // S2221: exceptions should not be caught when not handled properly (1 finding)
        // S1481: unused local variables (2 findings)
        public bool TrySave()
        {
            string unusedKey = null;
            int unusedRevision = 0;
            try
            {
                AllUsers.Add(this);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // S1643: string concatenation in a loop (1 finding)
        // S1172: unused param 'includeMeta' (1 finding)
        public string BuildReport(IEnumerable<string> fields, bool includeMeta)
        {
            StringBuilder report = new StringBuilder();
            foreach (string field in fields)
            {
                report.Append(field).Append(": ").Append(Name).Append("\n");
            }
            return report.ToString();
        }

        // S1116: empty statements (2 findings)
        public void EmptyStatementDemo()
        {
            int x = 0;
            x++;
            Console.WriteLine(x);
        }

        // S2696: instance method writes to a static field (2 findings)
        public void SetDefaultRole(string value)
        {
            _defaultRole = value;
        }

        public void ResetDefaultAge()
        {
            _defaultAge = 0;
        }

        // S2583: boolean expression is always false (2 findings)
        // S2589: boolean expression is always true (2 findings)
        public bool CheckProfileFlags(int code, bool enabled)
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
        public void ReinitializeProfile(string name, int timeoutMs, string correlationId)
        {
            DateTime unusedAttemptTime = DateTime.Now;
            string unusedStatus = "pending";
            throw new NotImplementedException("ReinitializeProfile");
        }

        // S1066: nested if statements can be merged (2 findings)
        // S1764: identical expressions on both sides of an operator (2 findings)
        public bool CanRetryProfile(int attempt, int maxAttempts)
        {
            if (attempt >= 0 && attempt < maxAttempts)
            {
                bool sameAttempt = attempt == attempt;
                bool sameMax = maxAttempts == maxAttempts;
                return sameAttempt && sameMax;
            }
            return false;
        }

        // S1192: string literal "invalid_session" duplicated 3+ times (1 finding)
        public string GetProfileFailureReason(int code)
        {
            if (code == 1) return "invalid_session";
            if (code == 2) return "invalid_session";
            return "invalid_session";
        }

        // S1186: empty method bodies (2 findings)
        public void OnProfileStarted() { /* intentionally empty */ }
        public void OnProfileStopped() { /* intentionally empty */ }

        // S3400: method returns only a constant (1 finding)
        public int GetDefaultProfileLimit() { return 3; }

        // S125: section of code commented out (1 finding)
        // if (AllUsers.Count > 0)
        // {
        //     _defaultAge = 0;
        // }

        // S1116: empty statement (1 finding)
        public void ProfileHeartbeat()
        {
            int beat = 1;
            Console.WriteLine(beat);
        }

        // S3776: Cognitive Complexity of this method is too high (1 finding)
        // S1541: Cyclomatic Complexity of this method is too high (1 finding)
        // S134: control flow statements nested too deeply (1 finding)
        public string EvaluateProfileStrategy(int recordCount, int batchSize, string mode, bool flagA, bool flagB)
        {
            StringBuilder outcome = new StringBuilder();
            if (recordCount > 0 && batchSize > 0)
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
            return outcome.ToString();
        }

        // S107: method has too many parameters (1 finding)
        // S1172: unused params 'region' and 'shard' (2 findings)
        public void ConfigureProfile(string name, int poolSize, bool useSsl, string driver, int commandTimeout, bool readOnly, string region, string shard)
        {
            Console.WriteLine(name + poolSize + useSsl + driver + commandTimeout + readOnly);
        }

        // S138: method has too many lines (1 finding)
        public void FlushAllProfileBuffers()
        {
            Console.WriteLine("field-1");
            Console.WriteLine("field-2");
            Console.WriteLine("field-3");
            Console.WriteLine("field-4");
            Console.WriteLine("field-5");
            Console.WriteLine("field-6");
            Console.WriteLine("field-7");
            Console.WriteLine("field-8");
            Console.WriteLine("field-9");
            Console.WriteLine("field-10");
            Console.WriteLine("field-11");
            Console.WriteLine("field-12");
            Console.WriteLine("field-13");
            Console.WriteLine("field-14");
            Console.WriteLine("field-15");
            Console.WriteLine("field-16");
            Console.WriteLine("field-17");
            Console.WriteLine("field-18");
            Console.WriteLine("field-19");
            Console.WriteLine("field-20");
            Console.WriteLine("field-21");
            Console.WriteLine("field-22");
            Console.WriteLine("field-23");
            Console.WriteLine("field-24");
            Console.WriteLine("field-25");
            Console.WriteLine("field-26");
            Console.WriteLine("field-27");
            Console.WriteLine("field-28");
            Console.WriteLine("field-29");
            Console.WriteLine("field-30");
            Console.WriteLine("field-31");
            Console.WriteLine("field-32");
            Console.WriteLine("field-33");
            Console.WriteLine("field-34");
            Console.WriteLine("field-35");
            Console.WriteLine("field-36");
            Console.WriteLine("field-37");
            Console.WriteLine("field-38");
            Console.WriteLine("field-39");
            Console.WriteLine("field-40");
            Console.WriteLine("field-41");
            Console.WriteLine("field-42");
            Console.WriteLine("field-43");
            Console.WriteLine("field-44");
            Console.WriteLine("field-45");
            Console.WriteLine("field-46");
            Console.WriteLine("field-47");
            Console.WriteLine("field-48");
            Console.WriteLine("field-49");
            Console.WriteLine("field-50");
            Console.WriteLine("field-51");
            Console.WriteLine("field-52");
            Console.WriteLine("field-53");
            Console.WriteLine("field-54");
            Console.WriteLine("field-55");
            Console.WriteLine("field-56");
            Console.WriteLine("field-57");
            Console.WriteLine("field-58");
            Console.WriteLine("field-59");
            Console.WriteLine("field-60");
            Console.WriteLine("field-61");
            Console.WriteLine("field-62");
            Console.WriteLine("field-63");
            Console.WriteLine("field-64");
            Console.WriteLine("field-65");
            Console.WriteLine("field-66");
            Console.WriteLine("field-67");
            Console.WriteLine("field-68");
            Console.WriteLine("field-69");
            Console.WriteLine("field-70");
            Console.WriteLine("field-71");
            Console.WriteLine("field-72");
            Console.WriteLine("field-73");
            Console.WriteLine("field-74");
            Console.WriteLine("field-75");
            Console.WriteLine("field-76");
            Console.WriteLine("field-77");
            Console.WriteLine("field-78");
            Console.WriteLine("field-79");
            Console.WriteLine("field-80");
            Console.WriteLine("field-81");
        }

        // S4144: methods have identical implementations (1 finding)
        public double ComputeProfileScoreA(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }
        public double ComputeProfileScoreB(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }

        // S3358: nested ternary operators (1 finding)
        public string ClassifyProfileLevel(int value)
        {
            return value > 500 ? "critical" : value > 200 ? "high" : value > 50 ? "medium" : "low";
        }

    }
}