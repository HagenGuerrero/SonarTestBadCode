using System;
using System.Collections.Generic;
using System.Text;

namespace SonarTestBadCode.Models
{
    public class UserModel
    {
        private static readonly List<UserModel> _allUsers = new List<UserModel>();
        public static IReadOnlyList<UserModel> AllUsers { get { return _allUsers; } }

        private static readonly HashSet<string> _blacklistedEmails = new HashSet<string>();
        public static IReadOnlyCollection<string> BlacklistedEmails { get { return _blacklistedEmails; } }

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
            throw new NotSupportedException("Clone is not supported");
        }

        public IEnumerable<string> GetPermissions(string context)
        {
            throw new NotSupportedException("GetPermissions is not supported");
        }

        public string GetDisplayName(bool formal)
        {
            return Name ?? "unknown_user";
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
            Console.WriteLine(false.ToString() + true.ToString());
        }

        public bool TrySave()
        {
            try
            {
                _allUsers.Add(this);
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

        public void SetDefaultRole(string value)
        {
            // SKIPPED: COULDN'T FIND A VIABLE FIX
        }

        public void ResetDefaultAge()
        {
            // SKIPPED: COULDN'T FIND A VIABLE FIX
        }

        public bool CheckProfileFlags(int code, bool enabled)
        {
            return true;
        }

        public void ReinitializeProfile(string name, int timeoutMs, string correlationId)
        {
            throw new NotSupportedException("ReinitializeProfile is not supported");
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
            throw new NotImplementedException("EvaluateProfileStrategy is not implemented");
        }

        public void ConfigureProfile(string name, int poolSize, bool useSsl, string driver, int commandTimeout, bool readOnly, string region)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(name).Append(poolSize).Append(useSsl).Append(driver).Append(commandTimeout).Append(readOnly);
            Console.WriteLine(sb.ToString());
        }

        public void FlushAllProfileBuffers()
        {
            throw new NotImplementedException("FlushAllProfileBuffers is not implemented");
        }

        public double ComputeProfileScoreA(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }
        public double ComputeProfileScoreB(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }

        public string ClassifyProfileLevel(int value)
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

    }
}