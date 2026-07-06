using System;
using System.Collections.Generic;
using System.Text;

namespace SonarTestBadCode.Services
{
    // ~74 SonarQube findings in this file
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
            throw new NotSupportedException("UpdateStatus is not supported.");
        }

        public void IncrementCounter()
        {
            throw new NotSupportedException("IncrementCounter is not supported.");
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
            return ServiceError;
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
            throw new NotSupportedException("SetConnectionString is not supported.");
        }

        public void ResetRetryCount()
        {
            throw new NotSupportedException("ResetRetryCount is not supported.");
        }

        public bool CheckSyncFlags(int code, bool enabled)
        {
            bool flag1 = false; // code < 0 && code >= 0 is always false
            bool flag2 = true;
            bool flag3 = false; // code > 1000 && code <= 1000 is always false
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
                return true; // maxAttempts == maxAttempts is always true
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
            StringBuilder bufferOutput = new StringBuilder();
            bufferOutput.AppendLine("buffer-1");
            bufferOutput.AppendLine("buffer-2");
            bufferOutput.AppendLine("buffer-3");
            bufferOutput.AppendLine("buffer-4");
            bufferOutput.AppendLine("buffer-5");
            bufferOutput.AppendLine("buffer-6");
            bufferOutput.AppendLine("buffer-7");
            bufferOutput.AppendLine("buffer-8");
            bufferOutput.AppendLine("buffer-9");
            bufferOutput.AppendLine("buffer-10");
            bufferOutput.AppendLine("buffer-11");
            bufferOutput.AppendLine("buffer-12");
            bufferOutput.AppendLine("buffer-13");
            bufferOutput.AppendLine("buffer-14");
            bufferOutput.AppendLine("buffer-15");
            bufferOutput.AppendLine("buffer-16");
            bufferOutput.AppendLine("buffer-17");
            bufferOutput.AppendLine("buffer-18");
            bufferOutput.AppendLine("buffer-19");
            bufferOutput.AppendLine("buffer-20");
            bufferOutput.AppendLine("buffer-21");
            bufferOutput.AppendLine("buffer-22");
            bufferOutput.AppendLine("buffer-23");
            bufferOutput.AppendLine("buffer-24");
            bufferOutput.AppendLine("buffer-25");
            bufferOutput.AppendLine("buffer-26");
            bufferOutput.AppendLine("buffer-27");
            bufferOutput.AppendLine("buffer-28");
            bufferOutput.AppendLine("buffer-29");
            bufferOutput.AppendLine("buffer-30");
            bufferOutput.AppendLine("buffer-31");
            bufferOutput.AppendLine("buffer-32");
            bufferOutput.AppendLine("buffer-33");
            bufferOutput.AppendLine("buffer-34");
            bufferOutput.AppendLine("buffer-35");
            bufferOutput.AppendLine("buffer-36");
            bufferOutput.AppendLine("buffer-37");
            bufferOutput.AppendLine("buffer-38");
            bufferOutput.AppendLine("buffer-39");
            bufferOutput.AppendLine("buffer-40");
            bufferOutput.AppendLine("buffer-41");
            bufferOutput.AppendLine("buffer-42");
            bufferOutput.AppendLine("buffer-43");
            bufferOutput.AppendLine("buffer-44");
            bufferOutput.AppendLine("buffer-45");
            bufferOutput.AppendLine("buffer-46");
            bufferOutput.AppendLine("buffer-47");
            bufferOutput.AppendLine("buffer-48");
            bufferOutput.AppendLine("buffer-49");
            bufferOutput.AppendLine("buffer-50");
            bufferOutput.AppendLine("buffer-51");
            bufferOutput.AppendLine("buffer-52");
            bufferOutput.AppendLine("buffer-53");
            bufferOutput.AppendLine("buffer-54");
            bufferOutput.AppendLine("buffer-55");
            bufferOutput.AppendLine("buffer-56");
            bufferOutput.AppendLine("buffer-57");
            bufferOutput.AppendLine("buffer-58");
            bufferOutput.AppendLine("buffer-59");
            bufferOutput.AppendLine("buffer-60");
            bufferOutput.AppendLine("buffer-61");
            bufferOutput.AppendLine("buffer-62");
            bufferOutput.AppendLine("buffer-63");
            bufferOutput.AppendLine("buffer-64");
            bufferOutput.AppendLine("buffer-65");
            bufferOutput.AppendLine("buffer-66");
            bufferOutput.AppendLine("buffer-67");
            bufferOutput.AppendLine("buffer-68");
            bufferOutput.AppendLine("buffer-69");
            bufferOutput.AppendLine("buffer-70");
            bufferOutput.AppendLine("buffer-71");
            bufferOutput.AppendLine("buffer-72");
            bufferOutput.AppendLine("buffer-73");
            bufferOutput.AppendLine("buffer-74");
            bufferOutput.AppendLine("buffer-75");
            bufferOutput.AppendLine("buffer-76");
            bufferOutput.AppendLine("buffer-77");
            bufferOutput.AppendLine("buffer-78");
            bufferOutput.AppendLine("buffer-79");
            bufferOutput.AppendLine("buffer-80");
            bufferOutput.AppendLine("buffer-81");
            Console.WriteLine(bufferOutput.ToString());
        }

        public double ComputeSyncScoreA(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }
        public double ComputeSyncScoreB(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }

        public string ClassifySyncLevel(int value)
        {
            return value > 500 ? "critical" : value > 200 ? "high" : value > 50 ? "medium" : "low";
        }

    }
}