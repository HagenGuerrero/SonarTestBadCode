using System;
using System.Collections.Generic;
using System.Text;

namespace SonarTestBadCode.Services
{
    // ~44 SonarQube findings in this file
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
    }
}
