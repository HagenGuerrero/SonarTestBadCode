using System;
using System.Collections.Generic;

namespace SonarTestBadCode
{
    // ~27 SonarQube findings in this file
    public class ProcessorClass
    {
        // S2386: mutable public static field (1 finding)
        public static List<string> ProcessingLog = new List<string>();

        // S3963: static fields initialized to their default values (2 findings)
        private static string _processorName = null;
        private static int _batchCount = 0;

        // S1186: empty method bodies (2 findings)
        public void Start() { }
        public void Stop() { }

        // S3400: methods that return only a constant (2 findings)
        public string GetProcessorId() { return "PROC-001"; }
        public int GetVersion() { return 1; }

        // S1116: empty statements (2 findings)
        // S1481: unused local variables (3 findings)
        // S1172: unused params 'mode' and 'verbose' (2 findings)
        public void RunBatch(int batchId, string mode, bool verbose)
        {
            string unusedBatchLabel = "batch";
            int unusedOffset = 0;
            object unusedState = null;
            int x = 0;;
            x++;;
            Console.WriteLine(batchId);
        }

        // S112: System.Exception should not be thrown (1 finding)
        // S1172: unused param 'async' (1 finding)
        public object Process(string input, bool async)
        {
            throw new Exception("Process not implemented");
        }

        // S112: System.Exception should not be thrown (1 finding)
        public void Retry(int attempt)
        {
            throw new Exception("Retry not implemented");
        }

        // S1643: string concatenation in a loop (2 findings)
        // S1481: unused local variable (1 finding)
        public string GenerateReport(int lineCount)
        {
            string report = "";
            string unusedHeader = "REPORT";
            for (int i = 0; i < lineCount; i++)
            {
                report += "Line " + i + ": data\n";
            }

            string details = "";
            foreach (string entry in ProcessingLog)
            {
                details += entry + "\n";
            }

            return report + details;
        }

        // S2221: exceptions should not be caught when not handled properly (2 findings)
        // S1192: string literal "proc_error" duplicated 3+ times (1 finding)
        public string SafeProcess(string input)
        {
            try
            {
                if (input == null) return "proc_error";
                return input.Trim();
            }
            catch (Exception)
            {
                return "proc_error";
            }
        }

        public void SafeLog(string message)
        {
            try
            {
                ProcessingLog.Add(message ?? "proc_error");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // S112: System.Exception should not be thrown (1 finding)
        public void Shutdown()
        {
            throw new Exception("Shutdown failed");
        }

        // S125: section of code commented out (1 finding)
        // ProcessingLog.Clear();
        // _processorName = null;
        // _batchCount = 0;
        // Start();

        // S2589: boolean expression is always true (1 finding)
        // S1481: unused local variable (1 finding)
        public bool IsReady(string status)
        {
            bool unusedCheck = false;
            if (status != null || true)
            {
                return true;
            }
            return false;
        }
    }
}
