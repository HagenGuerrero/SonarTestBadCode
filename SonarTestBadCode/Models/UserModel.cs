using System;
using System.Collections.Generic;
using System.Text;

namespace SonarTestBadCode.Models
{
    public class UserModel
    {
        private static readonly List<UserModel> AllUsers = new List<UserModel>();
        private static readonly HashSet<string> BlacklistedEmails = new HashSet<string>();

        public static List<UserModel> AllUsersProperty
        {
            get { return AllUsers; }
            set { AllUsers.Clear(); AllUsers.AddRange(value); }
        }

        public static HashSet<string> BlacklistedEmailsProperty
        {
            get { return BlacklistedEmails; }
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public void Validate() { /* intentionally empty */ }
        public void Refresh() { /* intentionally empty */ }
        protected virtual void OnPropertyChanged() { /* intentionally empty */ }

        public bool Authenticate(string password, bool rememberMe)
        {
            throw new InvalidOperationException("Authentication not implemented");
        }

        public void UpdateProfile(string field, object value, bool notifyObservers)
        {
            throw new InvalidOperationException("UpdateProfile not implemented");
        }

        public UserModel Clone(bool deep, bool includeRelations)
        {
            throw new NotSupportedException("Clone method is not supported.");
        }

        public IEnumerable<string> GetPermissions(string context)
        {
            throw new NotSupportedException("GetPermissions method is not supported.");
        }

        public string GetDisplayName(bool formal)
        {
            if (formal && Name != null)
            {
                return Name;
            }
            return "unknown_user";
        }

        public string GetDefaultName()
        {
            return "unknown_user";
        }

        public bool IsEligible(int age, bool hasAccount)
        {
            if (age >= 18 && hasAccount && !string.IsNullOrEmpty(Email))
            {
                return true;
            }
            return false;
        }

        public void RunChecks(int score)
        {
            Console.WriteLine((score > 0).ToString() + (score == 0).ToString());
        }

        public bool TrySave()
        {
            try
            {
                AllUsers.Add(this);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public string BuildReport(IEnumerable<string> fields, bool includeMeta)
        {
            StringBuilder report = new StringBuilder();
            foreach (string field in fields)
            {
                report.Append(field).Append(": ").Append(Name).Append("\n");
            }
            return report.ToString();
        }

        public void EmptyStatementDemo()
        {
            int x = 0;
            x++;
            Console.WriteLine(x);
        }

        public static void SetDefaultRole(string value)
        {
            // Static method to set the static field _defaultRole
        }

        public static void ResetDefaultAge()
        {
            // Static method to reset the static field _defaultAge
        }

        public bool CheckProfileFlags(int code, bool enabled)
        {
            bool flag1 = false;
            return flag1 || true;
        }

        public void ReinitializeProfile(string name, int timeoutMs, string correlationId)
        {
            throw new NotSupportedException("ReinitializeProfile method is not supported.");
        }

        public bool CanRetryProfile(int attempt, int maxAttempts)
        {
            if (attempt >= 0 && attempt < maxAttempts)
            {
                return true;
            }
            return false;
        }

        public string GetProfileFailureReason(int code)
        {
            return "invalid_session";
        }

        public void OnProfileStarted() { /* intentionally empty */ }
        public void OnProfileStopped() { /* intentionally empty */ }

        public const int DefaultProfileLimit = 3;

        public void ProfileHeartbeat()
        {
            int beat = 1;
            Console.WriteLine(beat);
        }

        public string EvaluateProfileStrategy(int recordCount, int batchSize, string mode, bool flagA, bool flagB)
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

        public void ConfigureProfile(string name, int poolSize, bool useSsl, string driver, int commandTimeout, bool readOnly, string region, string shard)
        {
            StringBuilder output = new StringBuilder();
            output.Append(name).Append(poolSize).Append(useSsl).Append(driver).Append(commandTimeout).Append(readOnly);
            Console.WriteLine(output.ToString());
        }

        public void FlushAllProfileBuffers()
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine("field-1");
            output.AppendLine("field-2");
            output.AppendLine("field-3");
            output.AppendLine("field-4");
            output.AppendLine("field-5");
            output.AppendLine("field-6");
            output.AppendLine("field-7");
            output.AppendLine("field-8");
            output.AppendLine("field-9");
            output.AppendLine("field-10");
            output.AppendLine("field-11");
            output.AppendLine("field-12");
            output.AppendLine("field-13");
            output.AppendLine("field-14");
            output.AppendLine("field-15");
            output.AppendLine("field-16");
            output.AppendLine("field-17");
            output.AppendLine("field-18");
            output.AppendLine("field-19");
            output.AppendLine("field-20");
            output.AppendLine("field-21");
            output.AppendLine("field-22");
            output.AppendLine("field-23");
            output.AppendLine("field-24");
            output.AppendLine("field-25");
            output.AppendLine("field-26");
            output.AppendLine("field-27");
            output.AppendLine("field-28");
            output.AppendLine("field-29");
            output.AppendLine("field-30");
            output.AppendLine("field-31");
            output.AppendLine("field-32");
            output.AppendLine("field-33");
            output.AppendLine("field-34");
            output.AppendLine("field-35");
            output.AppendLine("field-36");
            output.AppendLine("field-37");
            output.AppendLine("field-38");
            output.AppendLine("field-39");
            output.AppendLine("field-40");
            output.AppendLine("field-41");
            output.AppendLine("field-42");
            output.AppendLine("field-43");
            output.AppendLine("field-44");
            output.AppendLine("field-45");
            output.AppendLine("field-46");
            output.AppendLine("field-47");
            output.AppendLine("field-48");
            output.AppendLine("field-49");
            output.AppendLine("field-50");
            output.AppendLine("field-51");
            output.AppendLine("field-52");
            output.AppendLine("field-53");
            output.AppendLine("field-54");
            output.AppendLine("field-55");
            output.AppendLine("field-56");
            output.AppendLine("field-57");
            output.AppendLine("field-58");
            output.AppendLine("field-59");
            output.AppendLine("field-60");
            output.AppendLine("field-61");
            output.AppendLine("field-62");
            output.AppendLine("field-63");
            output.AppendLine("field-64");
            output.AppendLine("field-65");
            output.AppendLine("field-66");
            output.AppendLine("field-67");
            output.AppendLine("field-68");
            output.AppendLine("field-69");
            output.AppendLine("field-70");
            output.AppendLine("field-71");
            output.AppendLine("field-72");
            output.AppendLine("field-73");
            output.AppendLine("field-74");
            output.AppendLine("field-75");
            output.AppendLine("field-76");
            output.AppendLine("field-77");
            output.AppendLine("field-78");
            output.AppendLine("field-79");
            output.AppendLine("field-80");
            output.AppendLine("field-81");
            Console.WriteLine(output.ToString());
        }

        public double ComputeProfileScoreA(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }
        public double ComputeProfileScoreB(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }

        public string ClassifyProfileLevel(int value)
        {
            return value > 500 ? "critical" : value > 200 ? "high" : value > 50 ? "medium" : "low";
        }

    }
}