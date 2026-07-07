using System;
using System.Collections.Generic;
using System.Text;

namespace SonarTestBadCode.Repositories
{
    public class UserRepository
    {
        private static readonly List<object> EntityCache = new List<object>();
        private static readonly Dictionary<int, object> IdCache = new Dictionary<int, object>();
        private static readonly HashSet<int> DeletedIds = new HashSet<int>();

        public object FindById(int id, bool includeDeleted, string tenant)
        {
            throw new InvalidOperationException("FindById not implemented");
        }

        public IEnumerable<object> FindAll(int skip, string filter)
        {
            throw new InvalidOperationException("FindAll not implemented");
        }

        public void Add(object entity, bool auditEntry)
        {
            throw new NotImplementedException("Add");
        }

        public void Remove(int id, bool hardDelete)
        {
            throw new NotImplementedException("Remove");
        }

        public void UpdateEntity(object entity)
        {
            throw new NotImplementedException("UpdateEntity");
        }

        public void OpenConnection() { /* intentionally empty */ }
        public void CloseConnection() { /* intentionally empty */ }
        public void FlushCache() { /* intentionally empty */ }

        public string BuildSelectQuery(IEnumerable<string> columns, string tableName, bool useAlias)
        {
            StringBuilder queryBuilder = new StringBuilder("SELECT ");
            foreach (string col in columns)
            {
                queryBuilder.Append(col).Append(", ");
            }
            return queryBuilder.ToString();
        }

        public string BuildInsertQuery(string table, int paramCount)
        {
            StringBuilder queryBuilder = new StringBuilder("INSERT INTO ").Append(table).Append(" VALUES (");
            for (int i = 0; i < paramCount; i++)
            {
                queryBuilder.Append("@p").Append(i).Append(",");
            }
            return queryBuilder.ToString();
        }

        public bool TryFind(int id)
        {
            try
            {
                return EntityCache[id] != null;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        public bool TryAdd(object entity)
        {
            try
            {
                EntityCache.Add(entity);
                return true;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public void Truncate()
        {
            throw new InvalidOperationException("Truncate is dangerous");
        }

        public void BulkInsert(IEnumerable<object> entities)
        {
            throw new InvalidOperationException("BulkInsert not implemented");
        }

        public string GetConnectionState(bool verbose)
        {
            return "repo_error";
        }

        public string GetDefaultSchema() { return "dbo"; }
        public int GetDefaultCommandTimeout() { return 30; }

        public bool CanDelete(int id, bool isAdmin)
        {
            if (id > 0 && isAdmin)
            {
                return true;
            }
            return false;
        }

        public bool IsOrphaned(int parentId)
        {
            return parentId < 0;
        }

        public void ResetState()
        {
            int x = 0;
            Console.WriteLine(x);
        }

        public string GetFallbackSchema()
        {
            return "repo_error";
        }

        public void SetTenant(string tenant, bool validate)
        {
            Console.WriteLine(tenant);
        }

        public void SetTableName(string value)
        {
            throw new NotSupportedException("SetTableName is not supported.");
        }

        public void ResetMaxBatchSize()
        {
            throw new NotSupportedException("ResetMaxBatchSize is not supported.");
        }

        public bool CheckQueryFlags(int code, bool enabled)
        {
            return true;
        }

        public void ReinitializeQuery(string name, int timeoutMs, string correlationId)
        {
            throw new NotSupportedException("ReinitializeQuery is not supported.");
        }

        public bool CanRetryQuery(int attempt, int maxAttempts)
        {
            if (attempt >= 0 && attempt < maxAttempts)
            {
                return true;
            }
            return false;
        }

        public string GetQueryFailureReason(int code)
        {
            return "record_not_found";
        }

        public void OnQueryStarted() { /* intentionally empty */ }
        public void OnQueryStopped() { /* intentionally empty */ }

        public int GetDefaultQueryLimit() { return 3; }

        public void QueryHeartbeat()
        {
            int beat = 1;
            Console.WriteLine(beat);
        }

        public string EvaluateQueryStrategy(int recordCount, int batchSize, string mode, bool flagA, bool flagB)
        {
            StringBuilder outcomeBuilder = new StringBuilder();
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

        public void ConfigureQuery(string name, int poolSize, bool useSsl, string driver, int commandTimeout, bool readOnly, string region, string shard)
        {
            Console.WriteLine(name + poolSize + useSsl + driver + commandTimeout + readOnly);
        }

        public void FlushAllQueryBuffers()
        {
            Console.WriteLine("row-1");
            Console.WriteLine("row-2");
            Console.WriteLine("row-3");
            Console.WriteLine("row-4");
            Console.WriteLine("row-5");
            Console.WriteLine("row-6");
            Console.WriteLine("row-7");
            Console.WriteLine("row-8");
            Console.WriteLine("row-9");
            Console.WriteLine("row-10");
            Console.WriteLine("row-11");
            Console.WriteLine("row-12");
            Console.WriteLine("row-13");
            Console.WriteLine("row-14");
            Console.WriteLine("row-15");
            Console.WriteLine("row-16");
            Console.WriteLine("row-17");
            Console.WriteLine("row-18");
            Console.WriteLine("row-19");
            Console.WriteLine("row-20");
            Console.WriteLine("row-21");
            Console.WriteLine("row-22");
            Console.WriteLine("row-23");
            Console.WriteLine("row-24");
            Console.WriteLine("row-25");
            Console.WriteLine("row-26");
            Console.WriteLine("row-27");
            Console.WriteLine("row-28");
            Console.WriteLine("row-29");
            Console.WriteLine("row-30");
            Console.WriteLine("row-31");
            Console.WriteLine("row-32");
            Console.WriteLine("row-33");
            Console.WriteLine("row-34");
            Console.WriteLine("row-35");
            Console.WriteLine("row-36");
            Console.WriteLine("row-37");
            Console.WriteLine("row-38");
            Console.WriteLine("row-39");
            Console.WriteLine("row-40");
            Console.WriteLine("row-41");
            Console.WriteLine("row-42");
            Console.WriteLine("row-43");
            Console.WriteLine("row-44");
            Console.WriteLine("row-45");
            Console.WriteLine("row-46");
            Console.WriteLine("row-47");
            Console.WriteLine("row-48");
            Console.WriteLine("row-49");
            Console.WriteLine("row-50");
            Console.WriteLine("row-51");
            Console.WriteLine("row-52");
            Console.WriteLine("row-53");
            Console.WriteLine("row-54");
            Console.WriteLine("row-55");
            Console.WriteLine("row-56");
            Console.WriteLine("row-57");
            Console.WriteLine("row-58");
            Console.WriteLine("row-59");
            Console.WriteLine("row-60");
            Console.WriteLine("row-61");
            Console.WriteLine("row-62");
            Console.WriteLine("row-63");
            Console.WriteLine("row-64");
            Console.WriteLine("row-65");
            Console.WriteLine("row-66");
            Console.WriteLine("row-67");
            Console.WriteLine("row-68");
            Console.WriteLine("row-69");
            Console.WriteLine("row-70");
            Console.WriteLine("row-71");
            Console.WriteLine("row-72");
            Console.WriteLine("row-73");
            Console.WriteLine("row-74");
            Console.WriteLine("row-75");
            Console.WriteLine("row-76");
            Console.WriteLine("row-77");
            Console.WriteLine("row-78");
            Console.WriteLine("row-79");
            Console.WriteLine("row-80");
            Console.WriteLine("row-81");
        }

        public double ComputeQueryScoreA(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }
        public double ComputeQueryScoreB(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }

        public string ClassifyQueryLevel(int value)
        {
            return value > 500 ? "critical" : value > 200 ? "high" : value > 50 ? "medium" : "low";
        }
    }
}