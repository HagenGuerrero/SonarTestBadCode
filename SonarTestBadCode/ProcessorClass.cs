using System;
using System.Collections.Generic;
using System.Text;

namespace SonarTestBadCode
{
    public class ProcessorClass
    {
        private static readonly List<string> ProcessingLog = new List<string>();

        public void Start() { /* intentionally empty */ }
        public void Stop() { /* intentionally empty */ }

        public string GetProcessorId() { return "PROC-001"; }
        public int GetVersion() { return 1; }

        public void RunBatch(int batchId, string mode, bool verbose)
        {
            Console.WriteLine(batchId);
        }

        public object Process(string input, bool isAsync)
        {
            throw new NotImplementedException("Process not implemented");
        }

        public void Retry(int attempt)
        {
            throw new NotImplementedException("Retry not implemented");
        }

        public string GenerateReport(int lineCount)
        {
            StringBuilder reportBuilder = new StringBuilder();
            for (int i = 0; i < lineCount; i++)
            {
                reportBuilder.Append("Line ").Append(i).Append(": data\n");
            }

            StringBuilder detailsBuilder = new StringBuilder();
            foreach (string entry in ProcessingLog)
            {
                detailsBuilder.Append(entry).Append("\n");
            }

            return reportBuilder.ToString() + detailsBuilder.ToString();
        }

        public string SafeProcess(string input)
        {
            try
            {
                if (input == null) return "proc_error";
                return input.Trim();
            }
            catch (ArgumentNullException)
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
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Shutdown()
        {
            throw new NotImplementedException("Shutdown failed");
        }

        public bool IsReady(string status)
        {
            return true;
        }

        public static void SetProcessorName(string value)
        {
            // SKIPPED, COULDN'T FIND A VIABLE FIX
        }

        public static void ResetBatchCount()
        {
            // SKIPPED, COULDN'T FIND A VIABLE FIX
        }

        public bool CheckPipelineFlags(int code, bool enabled)
        {
            return enabled;
        }

        public void ReinitializePipeline(string name, int timeoutMs, string correlationId)
        {
            throw new NotSupportedException("ReinitializePipeline");
        }

        public bool CanRetryPipeline(int attempt, int maxAttempts)
        {
            if (attempt >= 0 && attempt < maxAttempts)
            {
                return true;
            }
            return false;
        }

        public string GetPipelineFailureReason(int code)
        {
            return "batch_failed";
        }

        public void OnPipelineStarted() { /* intentionally empty */ }
        public void OnPipelineStopped() { /* intentionally empty */ }

        public const int DefaultPipelineLimit = 3;

        public void PipelineHeartbeat()
        {
            int beat = 1;
            Console.WriteLine(beat);
        }

        public string EvaluatePipelineStrategy(int recordCount, int batchSize, string mode, bool flagA, bool flagB)
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

        public void ConfigurePipeline(string name, int poolSize, bool useSsl, string driver, int commandTimeout, bool readOnly, string region, string shard)
        {
            Console.WriteLine(name + poolSize + useSsl + driver + commandTimeout + readOnly);
        }

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

        public double ComputePipelineScoreA(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }
        public double ComputePipelineScoreB(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }

        public string ClassifyPipelineLevel(int value)
        {
            return value > 500 ? "critical" : value > 200 ? "high" : value > 50 ? "medium" : "low";
        }

    }
}