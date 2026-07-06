using System;
using System.Collections.Generic;
using System.Text;

namespace SonarTestBadCode.Models
{
    // ~39 SonarQube findings in this file
    public class UserModel
    {
        // S2386: mutable public static fields (2 findings)
        public static List<UserModel> AllUsers = new List<UserModel>();
        public static HashSet<string> BlacklistedEmails = new HashSet<string>();

        // S3963: static fields initialized to their default values (3 findings)
        private static string _defaultRole = null;
        private static int _defaultAge = 0;
        private static bool _defaultActive = false;

        // S1144: unused private member (1 finding)
        private string _unusedInternalNote = "internal";

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        // S1186: empty method bodies (3 findings)
        public void Validate() { }
        public void Refresh() { }
        protected virtual void OnPropertyChanged() { }

        // S112: System.Exception should not be thrown (1 finding)
        // S1172: unused params 'password' and 'rememberMe' (2 findings)
        // S1481: unused local variables (2 findings)
        public bool Authenticate(string password, bool rememberMe)
        {
            string unusedHash = null;
            int unusedAttempts = 0;
            throw new Exception("Authentication not implemented");
        }

        // S112: System.Exception should not be thrown (1 finding)
        // S1172: unused params 'field', 'value', 'notifyObservers' (3 findings)
        public void UpdateProfile(string field, object value, bool notifyObservers)
        {
            throw new Exception("UpdateProfile not implemented");
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
            else
            {
                return Name ?? "unknown_user";
            }
        }

        // S3400: method returns only a constant (1 finding)
        public string GetDefaultName()
        {
            return "unknown_user";
        }

        // S1066: nested if statements can be merged (2 findings)
        public bool IsEligible(int age, bool hasAccount)
        {
            if (age >= 18)
            {
                if (hasAccount)
                {
                    if (!string.IsNullOrEmpty(Email))
                    {
                        return true;
                    }
                }
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
            string report = "";
            foreach (string field in fields)
            {
                report += field + ": " + Name + "\n";
            }
            return report;
        }

        // S1116: empty statements (2 findings)
        public void EmptyStatementDemo()
        {
            int x = 0;;
            x++;;
            Console.WriteLine(x);
        }
    }
}
