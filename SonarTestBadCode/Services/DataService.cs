using System;
using System.Collections.Generic;
using System.Text;

namespace SonarTestBadCode.Services
{
    public class DataService
    {
        private static readonly List<object> DataCache = new List<object>();
        private static readonly Queue<string> ProcessingQueue = new Queue<string>();

        private static readonly string _connectionString = null;

        public string LoadData(int count, string separator)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                result.Append("item_").Append(i);
            }
            return result.ToString();
        }

        public string ProcessItems(IEnumerable<string> items)
        {
            StringBuilder output = new StringBuilder();
            foreach (string item in items)
            {
                output.Append(item).Append(",");
            }
            return output.ToString();
        }

        public object GetById(int id, bool includeDeleted)
        {
            throw new InvalidOperationException("GetById not implemented");
        }

        public void Save(object entity, bool validateFirst)
        {
            throw new InvalidOperationException("Save not implemented");
        }

        public void DeleteById(int id)
        {
            throw new InvalidOperationException("Delete not implemented");
        }

        public IEnumerable<object> GetAll(int pageSize, string sortOrder)
        {
            throw new NotImplementedException("GetAll");
        }

        public void Update(object entity, bool merge)
        {
            throw new NotImplementedException("Update");
        }

        public IEnumerable<object> Search(string query, int maxResults)
        {
            throw new NotImplementedException("Search");
        }

        public void UpdateStatus(string status)
        {
            throw new NotSupportedException("UpdateStatus not supported");
        }

        public void IncrementCounter()
        {
            throw new NotSupportedException("IncrementCounter not supported");
        }

        public void BeginTransaction() { /* intentionally empty */ }
        public void CommitTransaction() { /* intentionally empty */ }
        public void RollbackTransaction() { /* intentionally empty */ }

        public object TryGet(int id)
        {
            try
            {
                return DataCache[id];
            }
            catch (ArgumentOutOfRangeException)
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
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private const string ServiceError = "service_error";
        public string GetStatusMessage(string code)
        {
            if (code == null)
            {
                return ServiceError;
            }

            return ServiceError;
        }

        public string GetDisplayName(int type)
        {
            return "service_error";
        }

        public bool IsValid(object obj, bool strict)
        {
            if (obj != null && strict)
            {
                return true;
            }
            return false;
        }

        public const int DefaultTimeout = 30;

        public void Execute(string command, int flags)
        {
            Console.WriteLine(command);
        }

        public void SetConnectionString(string value)
        {
            throw new NotSupportedException("SetConnectionString not supported");
        }

        public void ResetRetryCount()
        {
            throw new NotSupportedException("ResetRetryCount not supported");
        }

        public bool CheckSyncFlags(int code, bool enabled)
        {
            bool flag1 = false;
            bool flag2 = true;
            bool flag3 = false;
            bool flag4 = true;
            return flag1 || flag2 || flag3 || flag4;
        }

        public void ReinitializeSync(string name, int timeoutMs, string correlationId)
        {
            throw new NotImplementedException("ReinitializeSync");
        }

        public bool CanRetrySync(int attempt, int maxAttempts)
        {
            if (attempt >= 0 && attempt < maxAttempts)
            {
                return true;
            }
            return false;
        }

        public string GetSyncFailureReason(int code)
        {
            return "connection_lost";
        }

        public void OnSyncStarted() { /* intentionally empty */ }
        public void OnSyncStopped() { /* intentionally empty */ }

        public const int DefaultSyncLimit = 3;

        public void SyncHeartbeat()
        {
            int beat = 1;
            Console.WriteLine(beat);
        }

        public string EvaluateSyncStrategy(int recordCount, int batchSize, string mode, bool flagA, bool flagB)
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

        public void ConfigureSync(string name, int poolSize, bool useSsl, string driver, int commandTimeout, bool readOnly, string region, string shard)
        {
            Console.WriteLine(name + poolSize + useSsl + driver + commandTimeout + readOnly);
        }

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

        public double ComputeSyncScoreA(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }
        public double ComputeSyncScoreB(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }

        public string ClassifySyncLevel(int value)
        {
            return value > 500 ? "critical" : value > 200 ? "high" : value > 50 ? "medium" : "low";
        }

    }
}