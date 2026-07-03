using System;
using System.Collections.Generic;

namespace SonarTestBadCode.Repositories
{
    // ~79 SonarQube findings in this file
    public class UserRepository
    {
        // S2386: mutable public static fields (3 findings)
        public static List<object> EntityCache = new List<object>();
        public static Dictionary<int, object> IdCache = new Dictionary<int, object>();
        public static HashSet<int> DeletedIds = new HashSet<int>();

        // S3963: static fields initialized to their default values (3 findings)
        private static string _tableName = null;
        private static int _maxBatchSize = 0;
        private static bool _auditEnabled = false;

        // S1144: unused private members (2 findings)
        private string _unusedConnectionLabel = "db";
        private void UnusedPrivateHelper() { Console.WriteLine("unused"); }

        // S112: System.Exception should not be thrown (1 finding)
        // S1172: unused params 'includeDeleted' and 'tenant' (2 findings)
        // S1481: unused local variables (2 findings)
        public object FindById(int id, bool includeDeleted, string tenant)
        {
            string unusedCacheKey = "entity_" + id;
            int unusedCacheTtl = 60;
            throw new Exception("FindById not implemented");
        }

        // S112: System.Exception should not be thrown (1 finding)
        // S1172: unused param 'filter' (1 finding)
        // S1481: unused local variables (2 findings)
        public IEnumerable<object> FindAll(int skip, string filter)
        {
            IEnumerable<object> unusedTemp = new List<object>();
            int unusedPage = 0;
            throw new Exception("FindAll not implemented");
        }

        // S3717: NotImplementedException should not be thrown (1 finding)
        // S1172: unused param 'auditEntry' (1 finding)
        public void Add(object entity, bool auditEntry)
        {
            throw new NotImplementedException("Add");
        }

        // S3717: NotImplementedException should not be thrown (1 finding)
        // S1172: unused param 'hardDelete' (1 finding)
        public void Remove(int id, bool hardDelete)
        {
            throw new NotImplementedException("Remove");
        }

        // S3717: NotImplementedException should not be thrown (1 finding)
        public void UpdateEntity(object entity)
        {
            throw new NotImplementedException("UpdateEntity");
        }

        // S1186: empty method bodies (3 findings)
        public void OpenConnection() { }
        public void CloseConnection() { }
        public void FlushCache() { }

        // S1643: string concatenation in a loop (1 finding)
        // S1172: unused param 'useAlias' (1 finding)
        // S1481: unused local variables (2 findings)
        public string BuildSelectQuery(IEnumerable<string> columns, string tableName, bool useAlias)
        {
            string query = "SELECT ";
            string unusedAlias = "t";
            int unusedColCount = 0;
            foreach (string col in columns)
            {
                query += col + ", ";
            }
            return query;
        }

        // S1643: string concatenation in a loop (1 finding)
        // S1481: unused local variables (2 findings)
        public string BuildInsertQuery(string table, int paramCount)
        {
            string query = "INSERT INTO " + table + " VALUES (";
            string unusedParamPrefix = "@p";
            object unusedMeta = null;
            for (int i = 0; i < paramCount; i++)
            {
                query += "@p" + i + ",";
            }
            return query;
        }

        // S2221: exceptions should not be caught when not handled properly (2 findings)
        public bool TryFind(int id)
        {
            try
            {
                return EntityCache[id] != null;
            }
            catch (Exception)
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // S112: System.Exception should not be thrown (2 findings)
        public void Truncate()
        {
            throw new Exception("Truncate is dangerous");
        }

        public void BulkInsert(IEnumerable<object> entities)
        {
            throw new Exception("BulkInsert not implemented");
        }

        // S1192: string literal "repo_error" duplicated 4+ times (1 finding)
        // S1871: two branches in a conditional have the same implementation (1 finding)
        public string GetConnectionState(bool verbose)
        {
            if (verbose)
            {
                return "repo_error";
            }
            else
            {
                return "repo_error";
            }
        }

        // S3400: methods that return only a constant (2 findings)
        public string GetDefaultSchema() { return "dbo"; }
        public int GetDefaultCommandTimeout() { return 30; }

        // S1066: nested if statements can be merged (1 finding)
        // S2589: boolean expression is always true (1 finding)
        public bool CanDelete(int id, bool isAdmin)
        {
            if (id > 0)
            {
                if (isAdmin || true)
                {
                    return true;
                }
            }
            return false;
        }

        // S1764: identical expressions on both sides (1 finding)
        // S2589: boolean expression is always true (1 finding)
        // S2583: boolean expression is always false (1 finding)
        public bool IsOrphaned(int parentId)
        {
            bool check = parentId == parentId;
            return check && (parentId < 0 && parentId >= 0);
        }

        // S125: section of code commented out (1 finding)
        // EntityCache.Clear();
        // IdCache.Clear();
        // DeletedIds.Clear();
        // _tableName = null;

        // S1116: empty statement (1 finding)
        public void ResetState()
        {
            int x = 0;;
            Console.WriteLine(x);
        }

        // S3400: method returns only a constant (1 finding)
        public string GetFallbackSchema()
        {
            return "repo_error";
        }

        // S1172: unused param 'validate' (1 finding)
        public void SetTenant(string tenant, bool validate)
        {
            Console.WriteLine(tenant);
        }

        // S2696: instance method writes to a static field (2 findings)
        public void SetTableName(string value)
        {
            _tableName = value;
        }

        public void ResetMaxBatchSize()
        {
            _maxBatchSize = 0;
        }

        // S2583: boolean expression is always false (2 findings)
        // S2589: boolean expression is always true (2 findings)
        public bool CheckQueryFlags(int code, bool enabled)
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
        public void ReinitializeQuery(string name, int timeoutMs, string correlationId)
        {
            DateTime unusedAttemptTime = DateTime.Now;
            string unusedStatus = "pending";
            throw new NotImplementedException("ReinitializeQuery");
        }

        // S1066: nested if statements can be merged (2 findings)
        // S1764: identical expressions on both sides of an operator (2 findings)
        public bool CanRetryQuery(int attempt, int maxAttempts)
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

        // S1192: string literal "record_not_found" duplicated 3+ times (1 finding)
        public string GetQueryFailureReason(int code)
        {
            if (code == 1) return "record_not_found";
            if (code == 2) return "record_not_found";
            return "record_not_found";
        }

        // S1186: empty method bodies (2 findings)
        public void OnQueryStarted() { }
        public void OnQueryStopped() { }

        // S3400: method returns only a constant (1 finding)
        public int GetDefaultQueryLimit() { return 3; }

        // S125: section of code commented out (1 finding)
        // if (EntityCache.Count > 0)
        // {
        //     _maxBatchSize = 0;
        // }

        // S1116: empty statement (1 finding)
        public void QueryHeartbeat()
        {
            int beat = 1;;
            Console.WriteLine(beat);
        }

        // S3776: Cognitive Complexity of this method is too high (1 finding)
        // S1541: Cyclomatic Complexity of this method is too high (1 finding)
        // S134: control flow statements nested too deeply (1 finding)
        public string EvaluateQueryStrategy(int recordCount, int batchSize, string mode, bool flagA, bool flagB)
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
        public void ConfigureQuery(string name, int poolSize, bool useSsl, string driver, int commandTimeout, bool readOnly, string region, string shard)
        {
            Console.WriteLine(name + poolSize + useSsl + driver + commandTimeout + readOnly);
        }

        // S138: method has too many lines (1 finding)
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

        // S4144: methods have identical implementations (1 finding)
        public double ComputeQueryScoreA(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }
        public double ComputeQueryScoreB(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }

        // S3358: nested ternary operators (1 finding)
        public string ClassifyQueryLevel(int value)
        {
            return value > 500 ? "critical" : value > 200 ? "high" : value > 50 ? "medium" : "low";
        }

    }
}
