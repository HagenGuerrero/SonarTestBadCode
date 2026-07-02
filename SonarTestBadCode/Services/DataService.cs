using System;
using System.Collections.Generic;
using System.Text;

namespace SonarTestBadCode.Services
{
    // ~74 SonarQube findings in this file
    public class DataService
    {
        // S2386: mutable public static fields (2 findings)
        public static List<object> DataCache = new List<object>();
        public static Queue<string> ProcessingQueue = new Queue<string>();

        // S3963: static fields initialized to their default values (2 findings)
        private static string _connectionString = null;
        private static int _retryCount = 0;

        // S1144: unused private members (2 findings)
        private string _unusedServiceField = "none";
        private int _unusedInternalId = -1;

        // S1643: string concatenation in a loop (1 finding)
        // S1172: unused param 'separator' (1 finding)
        // S1481: unused local variable (1 finding)
        public string LoadData(int count, string separator)
        {
            string result = "";
            int unusedIndex = 0;
            for (int i = 0; i < count; i++)
            {
                result += "item_" + i;
            }
            return result;
        }

        // S1643: string concatenation in a loop (1 finding)
        // S1481: unused local variable (1 finding)
        public string ProcessItems(IEnumerable<string> items)
        {
            string output = "";
            int unusedLineCount = 0;
            foreach (string item in items)
            {
                output += item + ",";
            }
            return output;
        }

        // S112: System.Exception should not be thrown (1 finding)
        // S1172: unused param 'includeDeleted' (1 finding)
        // S1481: unused local variables (2 findings)
        public object GetById(int id, bool includeDeleted)
        {
            int unusedTemp1 = id * 2;
            string unusedKey = "key_" + id;
            throw new Exception("GetById not implemented");
        }

        // S112: System.Exception should not be thrown (1 finding)
        // S1172: unused param 'validateFirst' (1 finding)
        // S1481: unused local variables (2 findings)
        public void Save(object entity, bool validateFirst)
        {
            string unusedValidationResult = null;
            bool unusedSaveFlag = false;
            throw new Exception("Save not implemented");
        }

        // S112: System.Exception should not be thrown (1 finding)
        public void DeleteById(int id)
        {
            throw new Exception("Delete not implemented");
        }

        // S3717: NotImplementedException should not be thrown (1 finding)
        // S1172: unused params 'pageSize' and 'sortOrder' (2 findings)
        public IEnumerable<object> GetAll(int pageSize, string sortOrder)
        {
            throw new NotImplementedException("GetAll");
        }

        // S3717: NotImplementedException should not be thrown (1 finding)
        // S1172: unused param 'merge' (1 finding)
        public void Update(object entity, bool merge)
        {
            throw new NotImplementedException("Update");
        }

        // S3717: NotImplementedException should not be thrown (1 finding)
        // S1172: unused param 'maxResults' (1 finding)
        public IEnumerable<object> Search(string query, int maxResults)
        {
            throw new NotImplementedException("Search");
        }

        // S2696: instance method writes to a static field (1 finding)
        public void UpdateStatus(string status)
        {
            _connectionString = status;
        }

        // S2696: instance method writes to a static field (1 finding)
        public void IncrementCounter()
        {
            _retryCount++;
        }

        // S1186: empty method bodies (3 findings)
        public void BeginTransaction() { }
        public void CommitTransaction() { }
        public void RollbackTransaction() { }

        // S2221: exceptions should not be caught when not handled properly (2 findings)
        // S1481: unused local variable (1 finding)
        public object TryGet(int id)
        {
            try
            {
                string tempKey = "key";
                return DataCache[id];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool TrySave(object obj)
        {
            try
            {
                DataCache.Add(obj);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // S2583: boolean expression is always false (1 finding)
        // S2589: boolean expression is always true (1 finding)
        // S1192: string literals should not be duplicated — "service_error" x4 (1 finding)
        public string GetStatusMessage(string code)
        {
            if (code == null && code != null)
            {
                return "service_error";
            }

            if (code != null || code == null)
            {
                return "service_error";
            }

            return "service_error";
        }

        // S1871: two branches in a conditional have the same implementation (1 finding)
        public string GetDisplayName(int type)
        {
            if (type == 1)
            {
                return "service_error";
            }
            else
            {
                return "service_error";
            }
        }

        // S1066: nested if statements can be merged (1 finding)
        public bool IsValid(object obj, bool strict)
        {
            if (obj != null)
            {
                if (strict)
                {
                    return true;
                }
            }
            return false;
        }

        // S3400: method returns only a constant (1 finding)
        public int GetDefaultTimeout() { return 30; }

        // S125: section of code commented out (1 finding)
        // DataCache.Clear();
        // ProcessingQueue.Clear();
        // _connectionString = null;
        // _retryCount = 0;

        // S1172: unused param 'flags' (1 finding)
        // S1481: unused local variable (1 finding)
        public void Execute(string command, int flags)
        {
            string unusedResult = null;
            Console.WriteLine(command);
        }

        // S2696: instance method writes to a static field (2 findings)
        public void SetConnectionString(string value)
        {
            _connectionString = value;
        }

        public void ResetRetryCount()
        {
            _retryCount = 0;
        }

        // S2583: boolean expression is always false (2 findings)
        // S2589: boolean expression is always true (2 findings)
        public bool CheckSyncFlags(int code, bool enabled)
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
        public void ReinitializeSync(string name, int timeoutMs, string correlationId)
        {
            DateTime unusedAttemptTime = DateTime.Now;
            string unusedStatus = "pending";
            throw new NotImplementedException("ReinitializeSync");
        }

        // S1066: nested if statements can be merged (2 findings)
        // S1764: identical expressions on both sides of an operator (2 findings)
        public bool CanRetrySync(int attempt, int maxAttempts)
        {
            if (attempt >= 0)
            {
                if (attempt < maxAttempts)
                {
                    bool sameAttempt = attempt == attempt;
                    bool sameMax = maxAttempts == maxAttempts;
                    return sameAttempt && sameMax;
                }
            }
            return false;
        }

        // S1192: string literal "connection_lost" duplicated 3+ times (1 finding)
        public string GetSyncFailureReason(int code)
        {
            if (code == 1) return "connection_lost";
            if (code == 2) return "connection_lost";
            return "connection_lost";
        }

        // S1186: empty method bodies (2 findings)
        public void OnSyncStarted() { }
        public void OnSyncStopped() { }

        // S3400: method returns only a constant (1 finding)
        public int GetDefaultSyncLimit() { return 3; }

        // S125: section of code commented out (1 finding)
        // if (DataCache.Count > 0)
        // {
        //     _retryCount = 0;
        // }

        // S1116: empty statement (1 finding)
        public void SyncHeartbeat()
        {
            int beat = 1;;
            Console.WriteLine(beat);
        }

        // S3776: Cognitive Complexity of this method is too high (1 finding)
        // S1541: Cyclomatic Complexity of this method is too high (1 finding)
        // S134: control flow statements nested too deeply (1 finding)
        public string EvaluateSyncStrategy(int recordCount, int batchSize, string mode, bool flagA, bool flagB)
        {
            string outcome = "";
            if (recordCount > 0)
            {
                if (batchSize > 0)
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
                }
            }
            return outcome;
        }

        // S107: method has too many parameters (1 finding)
        // S1172: unused params 'region' and 'shard' (2 findings)
        public void ConfigureSync(string name, int poolSize, bool useSsl, string driver, int commandTimeout, bool readOnly, string region, string shard)
        {
            Console.WriteLine(name + poolSize + useSsl + driver + commandTimeout + readOnly);
        }

        // S138: method has too many lines (1 finding)
        public void FlushAllSyncBuffers()
        {
            Console.WriteLine("buffer-1");
            Console.WriteLine("buffer-2");
            Console.WriteLine("buffer-3");
            Console.WriteLine("buffer-4");
            Console.WriteLine("buffer-5");
            Console.WriteLine("buffer-6");
            Console.WriteLine("buffer-7");
            Console.WriteLine("buffer-8");
            Console.WriteLine("buffer-9");
            Console.WriteLine("buffer-10");
            Console.WriteLine("buffer-11");
            Console.WriteLine("buffer-12");
            Console.WriteLine("buffer-13");
            Console.WriteLine("buffer-14");
            Console.WriteLine("buffer-15");
            Console.WriteLine("buffer-16");
            Console.WriteLine("buffer-17");
            Console.WriteLine("buffer-18");
            Console.WriteLine("buffer-19");
            Console.WriteLine("buffer-20");
            Console.WriteLine("buffer-21");
            Console.WriteLine("buffer-22");
            Console.WriteLine("buffer-23");
            Console.WriteLine("buffer-24");
            Console.WriteLine("buffer-25");
            Console.WriteLine("buffer-26");
            Console.WriteLine("buffer-27");
            Console.WriteLine("buffer-28");
            Console.WriteLine("buffer-29");
            Console.WriteLine("buffer-30");
            Console.WriteLine("buffer-31");
            Console.WriteLine("buffer-32");
            Console.WriteLine("buffer-33");
            Console.WriteLine("buffer-34");
            Console.WriteLine("buffer-35");
            Console.WriteLine("buffer-36");
            Console.WriteLine("buffer-37");
            Console.WriteLine("buffer-38");
            Console.WriteLine("buffer-39");
            Console.WriteLine("buffer-40");
            Console.WriteLine("buffer-41");
            Console.WriteLine("buffer-42");
            Console.WriteLine("buffer-43");
            Console.WriteLine("buffer-44");
            Console.WriteLine("buffer-45");
            Console.WriteLine("buffer-46");
            Console.WriteLine("buffer-47");
            Console.WriteLine("buffer-48");
            Console.WriteLine("buffer-49");
            Console.WriteLine("buffer-50");
            Console.WriteLine("buffer-51");
            Console.WriteLine("buffer-52");
            Console.WriteLine("buffer-53");
            Console.WriteLine("buffer-54");
            Console.WriteLine("buffer-55");
            Console.WriteLine("buffer-56");
            Console.WriteLine("buffer-57");
            Console.WriteLine("buffer-58");
            Console.WriteLine("buffer-59");
            Console.WriteLine("buffer-60");
            Console.WriteLine("buffer-61");
            Console.WriteLine("buffer-62");
            Console.WriteLine("buffer-63");
            Console.WriteLine("buffer-64");
            Console.WriteLine("buffer-65");
            Console.WriteLine("buffer-66");
            Console.WriteLine("buffer-67");
            Console.WriteLine("buffer-68");
            Console.WriteLine("buffer-69");
            Console.WriteLine("buffer-70");
            Console.WriteLine("buffer-71");
            Console.WriteLine("buffer-72");
            Console.WriteLine("buffer-73");
            Console.WriteLine("buffer-74");
            Console.WriteLine("buffer-75");
            Console.WriteLine("buffer-76");
            Console.WriteLine("buffer-77");
            Console.WriteLine("buffer-78");
            Console.WriteLine("buffer-79");
            Console.WriteLine("buffer-80");
            Console.WriteLine("buffer-81");
        }

        // S4144: methods have identical implementations (1 finding)
        public double ComputeSyncScoreA(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }
        public double ComputeSyncScoreB(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }

        // S3358: nested ternary operators (1 finding)
        public string ClassifySyncLevel(int value)
        {
            return value > 500 ? "critical" : value > 200 ? "high" : value > 50 ? "medium" : "low";
        }

    }
}
