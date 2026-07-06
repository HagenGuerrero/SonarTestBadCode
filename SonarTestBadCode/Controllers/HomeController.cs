using System;
using System.Collections.Generic;
using System.Text;

namespace SonarTestBadCode.Controllers
{
    // ~39 SonarQube findings in this file
    public class HomeController
    {
        // S2386: mutable public static fields (3 findings)
        public static List<string> CachedUsers = new List<string>();
        public static Dictionary<int, string> UserCache = new Dictionary<int, string>();
        public static Queue<string> PendingActions = new Queue<string>();

        // S3963: static fields initialized to their default values (3 findings)
        private static string _apiEndpoint = null;
        private static int _timeout = 0;
        private static bool _debugMode = false;

        // S1144: unused private members (2 findings)
        private string _unusedField = "unused";
        private int _unusedCounter = 0;

        // S1172: unused params 'request' and 'pageSize' (2 findings)
        // S1481: unused local variables (3 findings)
        // S112: System.Exception should not be thrown (1 finding)
        public string GetHome(string request, int pageSize)
        {
            int unusedVar1 = 10;
            string unusedVar2 = "controller_error";
            DateTime unusedTimestamp = DateTime.Now;

            throw new Exception("GetHome is not implemented");
        }

        // S1186: empty method bodies (3 findings)
        public void Initialize() { }
        public void Cleanup() { }
        protected virtual void OnActionExecuted() { }

        // S3400: methods that return only a constant (2 findings)
        public int GetDefaultPageSize() { return 10; }
        public string GetApiVersion() { return "v1"; }

        // S1172: unused param 'verbose' (1 finding)
        // S1481: unused local variables (2 findings)
        // S2589: boolean expression is always true (1 finding)
        public string ProcessRequest(string input, bool verbose)
        {
            StringBuilder sb = new StringBuilder();
            List<int> tempList = new List<int>();

            if (input != null || true)
            {
                return input ?? "controller_error";
            }

            return "controller_error";
        }

        // S1871: two branches in a conditional have the same implementation (1 finding)
        public string GetStatusMessage(int code)
        {
            if (code > 0)
            {
                return "controller_error";
            }
            else
            {
                return "controller_error";
            }
        }

        // S1066: nested if statements can be merged using the && operator (2 findings)
        public bool ValidateUser(string name, string email)
        {
            if (name != null)
            {
                if (name.Length > 0)
                {
                    if (email != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // S1764: identical expressions on both sides of a binary operator (2 findings)
        public bool CheckEquality(int a, string b)
        {
            bool r1 = a == a;
            bool r2 = b == b;
            return r1 && r2;
        }

        // S125: section of code commented out (1 finding)
        // int counter = 0;
        // for (int i = 0; i < 10; i++)
        // {
        //     counter += i;
        // }
        // Console.WriteLine("counter: " + counter);

        // S112: System.Exception should not be thrown (2 findings)
        public void Delete(int id)
        {
            if (id <= 0)
                throw new Exception("Invalid id value");
            throw new Exception("Delete operation not supported");
        }

        // S3400: method returns only a constant (1 finding)
        public string GetDefaultErrorMessage()
        {
            return "controller_error";
        }

        // S1172: unused params 'correlationId' and 'level' (2 findings)
        // S1481: unused local variables (2 findings)
        public void LogAction(string action, string correlationId, int level)
        {
            string logPrefix = "LOG";
            int sequenceNum = 0;
            Console.WriteLine(action);
        }

        // S2221: exceptions should not be caught when not handled properly (2 findings)
        public object SafeGet(string key)
        {
            try
            {
                return CachedUsers[0];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void SafeWrite(string value)
        {
            try
            {
                CachedUsers.Add(value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
