using System;
using System.Collections.Generic;
using System.Text;

namespace SonarTestBadCode.Models
{
    public class UserModel
    {
        public static readonly List<UserModel> AllUsers = new List<UserModel>();
        public static readonly HashSet<string> BlacklistedEmails = new HashSet<string>();

        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

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
            throw new NotSupportedException("GetPermissions is not supported.");
        }

        public string GetDisplayName(bool formal)
        {
            const string UnknownUser = "unknown_user";
            return Name ?? UnknownUser;
        }

        public const string DefaultName = "unknown_user";

        public string GetDefaultName()
        {
            return DefaultName;
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
            bool check1 = false;
            bool check2 = true;
            Console.WriteLine(check1.ToString() + check2.ToString());
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
            var reportBuilder = new StringBuilder();
            foreach (string field in fields)
            {
                reportBuilder.Append(field).Append(": ").Append(Name).Append("\n");
            }
            return reportBuilder.ToString();
        }

        public void EmptyStatementDemo()
        {
            int x = 0;
            x++;
            Console.WriteLine(x);
        }

        public static void SetDefaultRole(string value)
        {
            throw new NotSupportedException("SetDefaultRole is not supported.");
        }

        public static void ResetDefaultAge()
        {
            throw new NotSupportedException("ResetDefaultAge is not supported.");
        }

        public bool CheckProfileFlags(int code, bool enabled)
        {
            return true;
        }

        public void ReinitializeProfile(string name, int timeoutMs, string correlationId)
        {
            throw new NotSupportedException("ReinitializeProfile is not supported.");
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
            const string InvalidSession = "invalid_session";
            return InvalidSession;
        }

        public void OnProfileStarted() { /* intentionally empty */ }
        public void OnProfileStopped() { /* intentionally empty */ }

        public const int DefaultProfileLimit = 3;

        public int GetDefaultProfileLimit() { return DefaultProfileLimit; }

        public void ProfileHeartbeat()
        {
            int beat = 1;
            Console.WriteLine(beat);
        }

        public string EvaluateProfileStrategy(int recordCount, int batchSize, string mode, bool flagA, bool flagB)
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

        public void ConfigureProfile(string name, int poolSize, bool useSsl, string driver, int commandTimeout, bool readOnly, string region, string shard)
        {
            var outputBuilder = new StringBuilder();
            outputBuilder.Append(name).Append(poolSize).Append(useSsl).Append(driver).Append(commandTimeout).Append(readOnly);
            Console.WriteLine(outputBuilder.ToString());
        }

        public void FlushAllProfileBuffers()
        {
            var bufferBuilder = new StringBuilder();
            bufferBuilder.AppendLine("field-1");
            bufferBuilder.AppendLine("field-2");
            bufferBuilder.AppendLine("field-3");
            bufferBuilder.AppendLine("field-4");
            bufferBuilder.AppendLine("field-5");
            bufferBuilder.AppendLine("field-6");
            bufferBuilder.AppendLine("field-7");
            bufferBuilder.AppendLine("field-8");
            bufferBuilder.AppendLine("field-9");
            bufferBuilder.AppendLine("field-10");
            bufferBuilder.AppendLine("field-11");
            bufferBuilder.AppendLine("field-12");
            bufferBuilder.AppendLine("field-13");
            bufferBuilder.AppendLine("field-14");
            bufferBuilder.AppendLine("field-15");
            bufferBuilder.AppendLine("field-16");
            bufferBuilder.AppendLine("field-17");
            bufferBuilder.AppendLine("field-18");
            bufferBuilder.AppendLine("field-19");
            bufferBuilder.AppendLine("field-20");
            bufferBuilder.AppendLine("field-21");
            bufferBuilder.AppendLine("field-22");
            bufferBuilder.AppendLine("field-23");
            bufferBuilder.AppendLine("field-24");
            bufferBuilder.AppendLine("field-25");
            bufferBuilder.AppendLine("field-26");
            bufferBuilder.AppendLine("field-27");
            bufferBuilder.AppendLine("field-28");
            bufferBuilder.AppendLine("field-29");
            bufferBuilder.AppendLine("field-30");
            bufferBuilder.AppendLine("field-31");
            bufferBuilder.AppendLine("field-32");
            bufferBuilder.AppendLine("field-33");
            bufferBuilder.AppendLine("field-34");
            bufferBuilder.AppendLine("field-35");
            bufferBuilder.AppendLine("field-36");
            bufferBuilder.AppendLine("field-37");
            bufferBuilder.AppendLine("field-38");
            bufferBuilder.AppendLine("field-39");
            bufferBuilder.AppendLine("field-40");
            bufferBuilder.AppendLine("field-41");
            bufferBuilder.AppendLine("field-42");
            bufferBuilder.AppendLine("field-43");
            bufferBuilder.AppendLine("field-44");
            bufferBuilder.AppendLine("field-45");
            bufferBuilder.AppendLine("field-46");
            bufferBuilder.AppendLine("field-47");
            bufferBuilder.AppendLine("field-48");
            bufferBuilder.AppendLine("field-49");
            bufferBuilder.AppendLine("field-50");
            bufferBuilder.AppendLine("field-51");
            bufferBuilder.AppendLine("field-52");
            bufferBuilder.AppendLine("field-53");
            bufferBuilder.AppendLine("field-54");
            bufferBuilder.AppendLine("field-55");
            bufferBuilder.AppendLine("field-56");
            bufferBuilder.AppendLine("field-57");
            bufferBuilder.AppendLine("field-58");
            bufferBuilder.AppendLine("field-59");
            bufferBuilder.AppendLine("field-60");
            bufferBuilder.AppendLine("field-61");
            bufferBuilder.AppendLine("field-62");
            bufferBuilder.AppendLine("field-63");
            bufferBuilder.AppendLine("field-64");
            bufferBuilder.AppendLine("field-65");
            bufferBuilder.AppendLine("field-66");
            bufferBuilder.AppendLine("field-67");
            bufferBuilder.AppendLine("field-68");
            bufferBuilder.AppendLine("field-69");
            bufferBuilder.AppendLine("field-70");
            bufferBuilder.AppendLine("field-71");
            bufferBuilder.AppendLine("field-72");
            bufferBuilder.AppendLine("field-73");
            bufferBuilder.AppendLine("field-74");
            bufferBuilder.AppendLine("field-75");
            bufferBuilder.AppendLine("field-76");
            bufferBuilder.AppendLine("field-77");
            bufferBuilder.AppendLine("field-78");
            bufferBuilder.AppendLine("field-79");
            bufferBuilder.AppendLine("field-80");
            bufferBuilder.AppendLine("field-81");
            Console.WriteLine(bufferBuilder.ToString());
        }

        public double ComputeProfileScoreA(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }
        public double ComputeProfileScoreB(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }

        public string ClassifyProfileLevel(int value)
        {
            return value > 500 ? "critical" : value > 200 ? "high" : value > 50 ? "medium" : "low";
        }

    }
}