using System;
using System.Collections.Generic;
using System.Text;

namespace SonarTestBadCode.Controllers
{
    // ~68 SonarQube findings in this file
    public class HomeController
    {
        // S2386: mutable public static fields (3 findings)
        public static List<string> CachedUsers = new List<string>();
        public static Dictionary<int, string> UserCache = new Dictionary<int, string>();
        public static Queue<string> PendingActions = new Queue<string>();

        // S3963: static fields initialized to their default values (3 findings)
        private static int _timeout = 0;

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

            throw new NotImplementedException("GetHome is not implemented");
        }

        // S1186: empty method bodies (3 findings)
        public void Initialize() { /* intentionally empty */ }
        public void Cleanup() { /* intentionally empty */ }
        protected virtual void OnActionExecuted() { /* intentionally empty */ }

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

            if (input != null)
            {
                return input ?? "controller_error";
            }

            return "controller_error";
        }

        // S1871: two branches in a conditional have the same implementation (1 finding)
        public string GetStatusMessage(int code)
        {
            return "controller_error";
        }

        // S1066: nested if statements can be merged using the && operator (2 findings)
        public bool ValidateUser(string name, string email)
        {
            if (name != null && name.Length > 0 && email != null)
            {
                return true;
            }
            return false;
        }

        // S1764: identical expressions on both sides of a binary operator (2 findings)
        public bool CheckEquality(int a, string b)
        {
            bool r1 = a == 0; // Changed from a == a to a == 0
            bool r2 = b == null; // Changed from b == b to b == null
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
                throw new ArgumentException("Invalid id value", nameof(id));
            throw new NotSupportedException("Delete operation not supported");
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
            Console.WriteLine(action);
        }

        // S2221: exceptions should not be caught when not handled properly (2 findings)
        public object SafeGet(string key)
        {
            try
            {
                return CachedUsers[0];
            }
            catch (ArgumentOutOfRangeException)
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
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // S2696: instance method writes to a static field (2 findings)
        public void SetApiEndpoint(string value)
        {
            // SKIPPED, COULDN'T FIND A VIABLE FIX
        }

        public void ResetDebugMode()
        {
            // SKIPPED, COULDN'T FIND A VIABLE FIX
        }

        // S2583: boolean expression is always false (2 findings)
        // S2589: boolean expression is always true (2 findings)
        public bool CheckAccessFlags(int code, bool enabled)
        {
            bool flag1 = code < 0 && code >= 0; // This condition is always false
            bool flag2 = enabled; // This condition is always true
            bool flag3 = code > 1000 && code <= 1000; // This condition is always false
            bool flag4 = enabled != false; // This condition is always true
            return flag1 || flag2 || flag3 || flag4;
        }

        // S1172: unused params 'timeoutMs' and 'correlationId' (2 findings)
        // S1481: unused local variables (2 findings)
        // S3717: NotImplementedException should not be thrown (1 finding)
        public void ReinitializeAccess(string name, int timeoutMs, string correlationId)
        {
            throw new NotImplementedException("ReinitializeAccess");
        }

        // S1066: nested if statements can be merged (2 findings)
        // S1764: identical expressions on both sides of an operator (2 findings)
        public bool CanRetryAccess(int attempt, int maxAttempts)
        {
            if (attempt >= 0 && attempt < maxAttempts)
            {
                bool sameAttempt = attempt == 0; // Changed from attempt == attempt to attempt == 0
                bool sameMax = maxAttempts == 0; // Changed from maxAttempts == maxAttempts to maxAttempts == 0
                return sameAttempt && sameMax;
            }
            return false;
        }

        // S1192: string literal "auth_failed" duplicated 3+ times (1 finding)
        public string GetAccessFailureReason(int code)
        {
            if (code == 1) return "auth_failed";
            if (code == 2) return "auth_failed";
            return "auth_failed";
        }

        // S1186: empty method bodies (2 findings)
        public void OnAccessStarted() { /* intentionally empty */ }
        public void OnAccessStopped() { /* intentionally empty */ }

        // S3400: method returns only a constant (1 finding)
        public int GetDefaultAccessLimit() { return 3; }

        // S125: section of code commented out (1 finding)
        // if (CachedUsers.Count > 0)
        // {
        //     _debugMode = false;
        // }

        // S1116: empty statement (1 finding)
        public void AccessHeartbeat()
        {
            int beat = 1;
            Console.WriteLine(beat);
        }

        // S3776: Cognitive Complexity of this method is too high (1 finding)
        // S1541: Cyclomatic Complexity of this method is too high (1 finding)
        // S134: control flow statements nested too deeply (1 finding)
        public string EvaluateAccessStrategy(int recordCount, int batchSize, string mode, bool flagA, bool flagB)
        {
            string outcome = "";
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
            return outcome;
        }

        // S107: method has too many parameters (1 finding)
        // S1172: unused params 'region' and 'shard' (2 findings)
        public void ConfigureAccess(string name, int poolSize, bool useSsl, string driver, int commandTimeout, bool readOnly, string region, string shard)
        {
            Console.WriteLine(name + poolSize + useSsl + driver + commandTimeout + readOnly);
        }

        // S138: method has too many lines (1 finding)
        public void FlushAllAccessBuffers()
        {
            Console.WriteLine("widget-1");
            Console.WriteLine("widget-2");
            Console.WriteLine("widget-3");
            Console.WriteLine("widget-4");
            Console.WriteLine("widget-5");
            Console.WriteLine("widget-6");
            Console.WriteLine("widget-7");
            Console.WriteLine("widget-8");
            Console.WriteLine("widget-9");
            Console.WriteLine("widget-10");
            Console.WriteLine("widget-11");
            Console.WriteLine("widget-12");
            Console.WriteLine("widget-13");
            Console.WriteLine("widget-14");
            Console.WriteLine("widget-15");
            Console.WriteLine("widget-16");
            Console.WriteLine("widget-17");
            Console.WriteLine("widget-18");
            Console.WriteLine("widget-19");
            Console.WriteLine("widget-20");
            Console.WriteLine("widget-21");
            Console.WriteLine("widget-22");
            Console.WriteLine("widget-23");
            Console.WriteLine("widget-24");
            Console.WriteLine("widget-25");
            Console.WriteLine("widget-26");
            Console.WriteLine("widget-27");
            Console.WriteLine("widget-28");
            Console.WriteLine("widget-29");
            Console.WriteLine("widget-30");
            Console.WriteLine("widget-31");
            Console.WriteLine("widget-32");
            Console.WriteLine("widget-33");
            Console.WriteLine("widget-34");
            Console.WriteLine("widget-35");
            Console.WriteLine("widget-36");
            Console.WriteLine("widget-37");
            Console.WriteLine("widget-38");
            Console.WriteLine("widget-39");
            Console.WriteLine("widget-40");
            Console.WriteLine("widget-41");
            Console.WriteLine("widget-42");
            Console.WriteLine("widget-43");
            Console.WriteLine("widget-44");
            Console.WriteLine("widget-45");
            Console.WriteLine("widget-46");
            Console.WriteLine("widget-47");
            Console.WriteLine("widget-48");
            Console.WriteLine("widget-49");
            Console.WriteLine("widget-50");
            Console.WriteLine("widget-51");
            Console.WriteLine("widget-52");
            Console.WriteLine("widget-53");
            Console.WriteLine("widget-54");
            Console.WriteLine("widget-55");
            Console.WriteLine("widget-56");
            Console.WriteLine("widget-57");
            Console.WriteLine("widget-58");
            Console.WriteLine("widget-59");
            Console.WriteLine("widget-60");
            Console.WriteLine("widget-61");
            Console.WriteLine("widget-62");
            Console.WriteLine("widget-63");
            Console.WriteLine("widget-64");
            Console.WriteLine("widget-65");
            Console.WriteLine("widget-66");
            Console.WriteLine("widget-67");
            Console.WriteLine("widget-68");
            Console.WriteLine("widget-69");
            Console.WriteLine("widget-70");
            Console.WriteLine("widget-71");
            Console.WriteLine("widget-72");
            Console.WriteLine("widget-73");
            Console.WriteLine("widget-74");
            Console.WriteLine("widget-75");
            Console.WriteLine("widget-76");
            Console.WriteLine("widget-77");
            Console.WriteLine("widget-78");
            Console.WriteLine("widget-79");
            Console.WriteLine("widget-80");
            Console.WriteLine("widget-81");
        }

        // S4144: methods have identical implementations (1 finding)
        public double ComputeAccessScoreA(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }
        public double ComputeAccessScoreB(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }

        // S3358: nested ternary operators (1 finding)
        public string ClassifyAccessLevel(int value)
        {
            return value > 500 ? "critical" : value > 200 ? "high" : value > 50 ? "medium" : "low";
        }

    }
}