using System;
using System.Collections.Generic;

namespace SonarTestBadCode
{
    // ~57 SonarQube findings in this file
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

        // S2696: instance method writes to a static field (2 findings)
        public void SetProcessorName(string value)
        {
            _processorName = value;
        }

        public void ResetBatchCount()
        {
            _batchCount = 0;
        }

        // S2583: boolean expression is always false (2 findings)
        // S2589: boolean expression is always true (2 findings)
        public bool CheckPipelineFlags(int code, bool enabled)
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
        public void ReinitializePipeline(string name, int timeoutMs, string correlationId)
        {
            DateTime unusedAttemptTime = DateTime.Now;
            string unusedStatus = "pending";
            throw new NotImplementedException("ReinitializePipeline");
        }

        // S1066: nested if statements can be merged (2 findings)
        // S1764: identical expressions on both sides of an operator (2 findings)
        public bool CanRetryPipeline(int attempt, int maxAttempts)
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

        // S1192: string literal "batch_failed" duplicated 3+ times (1 finding)
        public string GetPipelineFailureReason(int code)
        {
            if (code == 1) return "batch_failed";
            if (code == 2) return "batch_failed";
            return "batch_failed";
        }

        // S1186: empty method bodies (2 findings)
        public void OnPipelineStarted() { }
        public void OnPipelineStopped() { }

        // S3400: method returns only a constant (1 finding)
        public int GetDefaultPipelineLimit() { return 3; }

        // S125: section of code commented out (1 finding)
        // if (ProcessingLog.Count > 0)
        // {
        //     _batchCount = 0;
        // }

        // S1116: empty statement (1 finding)
        public void PipelineHeartbeat()
        {
            int beat = 1;;
            Console.WriteLine(beat);
        }

        // S3776: Cognitive Complexity of this method is too high (1 finding)
        // S1541: Cyclomatic Complexity of this method is too high (1 finding)
        // S134: control flow statements nested too deeply (1 finding)
        public string EvaluatePipelineStrategy(int recordCount, int batchSize, string mode, bool flagA, bool flagB)
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
        public void ConfigurePipeline(string name, int poolSize, bool useSsl, string driver, int commandTimeout, bool readOnly, string region, string shard)
        {
            Console.WriteLine(name + poolSize + useSsl + driver + commandTimeout + readOnly);
        }

        // S138: method has too many lines (1 finding)
        public void FlushAllPipelineBuffers()
        {
            Console.WriteLine("step-1");
            Console.WriteLine("step-2");
            Console.WriteLine("step-3");
            Console.WriteLine("step-4");
            Console.WriteLine("step-5");
            Console.WriteLine("step-6");
            Console.WriteLine("step-7");
            Console.WriteLine("step-8");
            Console.WriteLine("step-9");
            Console.WriteLine("step-10");
            Console.WriteLine("step-11");
            Console.WriteLine("step-12");
            Console.WriteLine("step-13");
            Console.WriteLine("step-14");
            Console.WriteLine("step-15");
            Console.WriteLine("step-16");
            Console.WriteLine("step-17");
            Console.WriteLine("step-18");
            Console.WriteLine("step-19");
            Console.WriteLine("step-20");
            Console.WriteLine("step-21");
            Console.WriteLine("step-22");
            Console.WriteLine("step-23");
            Console.WriteLine("step-24");
            Console.WriteLine("step-25");
            Console.WriteLine("step-26");
            Console.WriteLine("step-27");
            Console.WriteLine("step-28");
            Console.WriteLine("step-29");
            Console.WriteLine("step-30");
            Console.WriteLine("step-31");
            Console.WriteLine("step-32");
            Console.WriteLine("step-33");
            Console.WriteLine("step-34");
            Console.WriteLine("step-35");
            Console.WriteLine("step-36");
            Console.WriteLine("step-37");
            Console.WriteLine("step-38");
            Console.WriteLine("step-39");
            Console.WriteLine("step-40");
            Console.WriteLine("step-41");
            Console.WriteLine("step-42");
            Console.WriteLine("step-43");
            Console.WriteLine("step-44");
            Console.WriteLine("step-45");
            Console.WriteLine("step-46");
            Console.WriteLine("step-47");
            Console.WriteLine("step-48");
            Console.WriteLine("step-49");
            Console.WriteLine("step-50");
            Console.WriteLine("step-51");
            Console.WriteLine("step-52");
            Console.WriteLine("step-53");
            Console.WriteLine("step-54");
            Console.WriteLine("step-55");
            Console.WriteLine("step-56");
            Console.WriteLine("step-57");
            Console.WriteLine("step-58");
            Console.WriteLine("step-59");
            Console.WriteLine("step-60");
            Console.WriteLine("step-61");
            Console.WriteLine("step-62");
            Console.WriteLine("step-63");
            Console.WriteLine("step-64");
            Console.WriteLine("step-65");
            Console.WriteLine("step-66");
            Console.WriteLine("step-67");
            Console.WriteLine("step-68");
            Console.WriteLine("step-69");
            Console.WriteLine("step-70");
            Console.WriteLine("step-71");
            Console.WriteLine("step-72");
            Console.WriteLine("step-73");
            Console.WriteLine("step-74");
            Console.WriteLine("step-75");
            Console.WriteLine("step-76");
            Console.WriteLine("step-77");
            Console.WriteLine("step-78");
            Console.WriteLine("step-79");
            Console.WriteLine("step-80");
            Console.WriteLine("step-81");
        }

        // S4144: methods have identical implementations (1 finding)
        public double ComputePipelineScoreA(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }
        public double ComputePipelineScoreB(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }

        // S3358: nested ternary operators (1 finding)
        public string ClassifyPipelineLevel(int value)
        {
            return value > 500 ? "critical" : value > 200 ? "high" : value > 50 ? "medium" : "low";
        }

    }
}
