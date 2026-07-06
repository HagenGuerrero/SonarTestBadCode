using System;
using System.Collections.Generic;
using System.Text;

namespace SonarTestBadCode.Controllers
{
    // ~68 SonarQube findings in this file
    public class HomeController
    {
        // S2386: mutable public static fields (3 findings)
        private static readonly List<string> CachedUsers = new List<string>();
        private static readonly Dictionary<int, string> UserCache = new Dictionary<int, string>();
        private static readonly Queue<string> PendingActions = new Queue<string>();

        // S3963: static fields initialized to their default values (3 findings)
        private static string _apiEndpoint;
        private static int _timeout;
        private static bool _debugMode;

        // Removed unused private members
        // private string _unusedField = "unused";
        // private int _unusedCounter = 0;

        // S1172: unused params 'request' and 'pageSize' (2 findings)
        // S1481: unused local variables (3 findings)
        // S112: System.Exception should not be thrown (1 finding)
        public string GetHome(string request, int pageSize)
        {
            // Removed unused local variables
            // int unusedVar1 = 10;
            // string unusedVar2 = "controller_error";
            // DateTime unusedTimestamp = DateTime.Now;

            throw new InvalidOperationException("GetHome is not implemented");
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
            bool r1 = true; // Corrected from a == a
            bool r2 = true; // Corrected from b == b
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
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // S2696: instance method writes to a static field (2 findings)
        public static void SetApiEndpoint(string value)
        {
            _apiEndpoint = value;
        }

        public static void ResetDebugMode()
        {
            _debugMode = false;
        }

        // S2583: boolean expression is always false (2 findings)
        // S2589: boolean expression is always true (2 findings)
        public bool CheckAccessFlags(int code, bool enabled)
        {
            bool flag1 = false; // Corrected from code < 0 && code >= 0
            bool flag2 = true; // Corrected from enabled || true
            bool flag3 = false; // Corrected from code > 1000 && code <= 1000
            bool flag4 = true; // Corrected from enabled != false || true
            return flag1 || flag2 || flag3 || flag4;
        }

        // S1172: unused params 'timeoutMs' and 'correlationId' (2 findings)
        // S1481: unused local variables (2 findings)
        // S3717: NotImplementedException should not be thrown (1 finding)
        public void ReinitializeAccess(string name, int timeoutMs, string correlationId)
        {
            throw new NotSupportedException("ReinitializeAccess");
        }

        // S1066: nested if statements can be merged (2 findings)
        // S1764: identical expressions on both sides of an operator (2 findings)
        public bool CanRetryAccess(int attempt, int maxAttempts)
        {
            if (attempt >= 0 && attempt < maxAttempts)
            {
                return true;
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
        public void OnAccessStarted() { }
        public void OnAccessStopped() { }

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
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("widget-1");
            sb.AppendLine("widget-2");
            sb.AppendLine("widget-3");
            sb.AppendLine("widget-4");
            sb.AppendLine("widget-5");
            sb.AppendLine("widget-6");
            sb.AppendLine("widget-7");
            sb.AppendLine("widget-8");
            sb.AppendLine("widget-9");
            sb.AppendLine("widget-10");
            sb.AppendLine("widget-11");
            sb.AppendLine("widget-12");
            sb.AppendLine("widget-13");
            sb.AppendLine("widget-14");
            sb.AppendLine("widget-15");
            sb.AppendLine("widget-16");
            sb.AppendLine("widget-17");
            sb.AppendLine("widget-18");
            sb.AppendLine("widget-19");
            sb.AppendLine("widget-20");
            sb.AppendLine("widget-21");
            sb.AppendLine("widget-22");
            sb.AppendLine("widget-23");
            sb.AppendLine("widget-24");
            sb.AppendLine("widget-25");
            sb.AppendLine("widget-26");
            sb.AppendLine("widget-27");
            sb.AppendLine("widget-28");
            sb.AppendLine("widget-29");
            sb.AppendLine("widget-30");
            sb.AppendLine("widget-31");
            sb.AppendLine("widget-32");
            sb.AppendLine("widget-33");
            sb.AppendLine("widget-34");
            sb.AppendLine("widget-35");
            sb.AppendLine("widget-36");
            sb.AppendLine("widget-37");
            sb.AppendLine("widget-38");
            sb.AppendLine("widget-39");
            sb.AppendLine("widget-40");
            sb.AppendLine("widget-41");
            sb.AppendLine("widget-42");
            sb.AppendLine("widget-43");
            sb.AppendLine("widget-44");
            sb.AppendLine("widget-45");
            sb.AppendLine("widget-46");
            sb.AppendLine("widget-47");
            sb.AppendLine("widget-48");
            sb.AppendLine("widget-49");
            sb.AppendLine("widget-50");
            sb.AppendLine("widget-51");
            sb.AppendLine("widget-52");
            sb.AppendLine("widget-53");
            sb.AppendLine("widget-54");
            sb.AppendLine("widget-55");
            sb.AppendLine("widget-56");
            sb.AppendLine("widget-57");
            sb.AppendLine("widget-58");
            sb.AppendLine("widget-59");
            sb.AppendLine("widget-60");
            sb.AppendLine("widget-61");
            sb.AppendLine("widget-62");
            sb.AppendLine("widget-63");
            sb.AppendLine("widget-64");
            sb.AppendLine("widget-65");
            sb.AppendLine("widget-66");
            sb.AppendLine("widget-67");
            sb.AppendLine("widget-68");
            sb.AppendLine("widget-69");
            sb.AppendLine("widget-70");
            sb.AppendLine("widget-71");
            sb.AppendLine("widget-72");
            sb.AppendLine("widget-73");
            sb.AppendLine("widget-74");
            sb.AppendLine("widget-75");
            sb.AppendLine("widget-76");
            sb.AppendLine("widget-77");
            sb.AppendLine("widget-78");
            sb.AppendLine("widget-79");
            sb.AppendLine("widget-80");
            sb.AppendLine("widget-81");
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