using System;
using System.Collections.Generic;

namespace SonarTestBadCode.Repositories
{
    // ~48 SonarQube findings in this file
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
    }
}
