using System;
using System.Collections.Generic;
using System.Text;

namespace SonarTestBadCode.Models
{
    public class UserModel
    {
        private static readonly List<UserModel> AllUsers = new List<UserModel>();
        private static readonly HashSet<string> BlacklistedEmails = new HashSet<string>();

        private static string _defaultRole;
        private static int _defaultAge;

        private readonly string _unusedInternalNote;

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
            throw new NotImplementedException("Clone");
        }

        public IEnumerable<string> GetPermissions(string context)
        {
            throw new NotImplementedException("GetPermissions");
        }

        public string GetDisplayName(bool formal)
        {
            return Name ?? "unknown_user";
        }

        public const string DefaultName = "unknown_user";

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
            Console.WriteLine(false.ToString() + true.ToString());
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
            var sb = new System.Text.StringBuilder();
            foreach (string field in fields)
            {
                sb.Append(field).Append(": ").Append(Name).Append("\n");
            }
            return sb.ToString();
        }

        public void EmptyStatementDemo()
        {
            int x = 0;
            x++;
            Console.WriteLine(x);
        }

        public static void SetDefaultRole(string value)
        {
            _defaultRole = value;
        }

        public static void ResetDefaultAge()
        {
            _defaultAge = 0;
        }

        public bool CheckProfileFlags(int code, bool enabled)
        {
            return true;
        }

        public void ReinitializeProfile(string name, int timeoutMs, string correlationId)
        {
            throw new NotImplementedException("ReinitializeProfile");
        }

        public bool CanRetryProfile(int attempt, int maxAttempts)
        {
            if (attempt >= 0 && attempt < maxAttempts)
            {
                return true;
            }
            return false;
        }

        public const string InvalidSession = "invalid_session";

        public string GetProfileFailureReason(int code)
        {
            return InvalidSession;
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
            var sb = new System.Text.StringBuilder();
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
                                sb.Append("synced");
                            }
                            else if (flagA || flagB)
                            {
                                sb.Append("partial");
                            }
                            else
                            {
                                sb.Append("skipped");
                            }
                        }
                        else
                        {
                            switch (i % 3)
                            {
                                case 0: sb.Append("a"); break;
                                case 1: sb.Append("b"); break;
                                case 2: sb.Append("c"); break;
                                default: sb.Append("d"); break;
                            }
                        }
                    }
                }
                else if (mode == "incremental")
                {
                    while (batchSize > 0)
                    {
                        batchSize--;
                        if (batchSize == recordCount) sb.Append("match");
                    }
                }
                else
                {
                    sb.Append("unknown-mode");
                }
            }
            return sb.ToString();
        }

        public void ConfigureProfile(string name, int poolSize, bool useSsl, string driver, int commandTimeout, bool readOnly)
        {
            Console.WriteLine(name + poolSize + useSsl + driver + commandTimeout + readOnly);
        }

        public void FlushAllProfileBuffers()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("field-1");
            sb.AppendLine("field-2");
            sb.AppendLine("field-3");
            sb.AppendLine("field-4");
            sb.AppendLine("field-5");
            sb.AppendLine("field-6");
            sb.AppendLine("field-7");
            sb.AppendLine("field-8");
            sb.AppendLine("field-9");
            sb.AppendLine("field-10");
            sb.AppendLine("field-11");
            sb.AppendLine("field-12");
            sb.AppendLine("field-13");
            sb.AppendLine("field-14");
            sb.AppendLine("field-15");
            sb.AppendLine("field-16");
            sb.AppendLine("field-17");
            sb.AppendLine("field-18");
            sb.AppendLine("field-19");
            sb.AppendLine("field-20");
            sb.AppendLine("field-21");
            sb.AppendLine("field-22");
            sb.AppendLine("field-23");
            sb.AppendLine("field-24");
            sb.AppendLine("field-25");
            sb.AppendLine("field-26");
            sb.AppendLine("field-27");
            sb.AppendLine("field-28");
            sb.AppendLine("field-29");
            sb.AppendLine("field-30");
            sb.AppendLine("field-31");
            sb.AppendLine("field-32");
            sb.AppendLine("field-33");
            sb.AppendLine("field-34");
            sb.AppendLine("field-35");
            sb.AppendLine("field-36");
            sb.AppendLine("field-37");
            sb.AppendLine("field-38");
            sb.AppendLine("field-39");
            sb.AppendLine("field-40");
            sb.AppendLine("field-41");
            sb.AppendLine("field-42");
            sb.AppendLine("field-43");
            sb.AppendLine("field-44");
            sb.AppendLine("field-45");
            sb.AppendLine("field-46");
            sb.AppendLine("field-47");
            sb.AppendLine("field-48");
            sb.AppendLine("field-49");
            sb.AppendLine("field-50");
            sb.AppendLine("field-51");
            sb.AppendLine("field-52");
            sb.AppendLine("field-53");
            sb.AppendLine("field-54");
            sb.AppendLine("field-55");
            sb.AppendLine("field-56");
            sb.AppendLine("field-57");
            sb.AppendLine("field-58");
            sb.AppendLine("field-59");
            sb.AppendLine("field-60");
            sb.AppendLine("field-61");
            sb.AppendLine("field-62");
            sb.AppendLine("field-63");
            sb.AppendLine("field-64");
            sb.AppendLine("field-65");
            sb.AppendLine("field-66");
            sb.AppendLine("field-67");
            sb.AppendLine("field-68");
            sb.AppendLine("field-69");
            sb.AppendLine("field-70");
            sb.AppendLine("field-71");
            sb.AppendLine("field-72");
            sb.AppendLine("field-73");
            sb.AppendLine("field-74");
            sb.AppendLine("field-75");
            sb.AppendLine("field-76");
            sb.AppendLine("field-77");
            sb.AppendLine("field-78");
            sb.AppendLine("field-79");
            sb.AppendLine("field-80");
            sb.AppendLine("field-81");
            Console.WriteLine(sb.ToString());
        }

        public double ComputeProfileScoreA(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }
        public double ComputeProfileScoreB(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }

        public string ClassifyProfileLevel(int value)
        {
            if (value > 500)
            {
                return "critical";
            }
            else if (value > 200)
            {
                return "high";
            }
            else if (value > 50)
            {
                return "medium";
            }
            else
            {
                return "low";
            }
        }

    }
}