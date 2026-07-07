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

        // S1172: unused params 'request' and 'pageSize' (2 findings)
        // S1481: unused local variables (3 findings)
        // S112: System.Exception should not be thrown (1 finding)
        public string GetHome(string request, int pageSize)
        {
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
                return input;
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
            return a == int.Parse(b);
        }

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
            return enabled;
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
            const string AuthFailed = "auth_failed";
            return AuthFailed;
        }

        // S1186: empty method bodies (2 findings)
        public void OnAccessStarted() { /* intentionally empty */ }
        public void OnAccessStopped() { /* intentionally empty */ }

        // S3400: method returns only a constant (1 finding)
        public int GetDefaultAccessLimit() { return 3; }

        // S1116: empty statement (1 finding)
        public void AccessHeartbeat()
        {
            int beat = 1;
            Console.WriteLine(beat);
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