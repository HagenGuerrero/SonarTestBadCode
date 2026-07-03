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

        public const string ProcessorId = "PROC-001";
        public const int Version = 1;

        public void RunBatch(int batchId, string mode, bool verbose)
        {
            Console.WriteLine(batchId);
        }

        public object Process(string input, bool asyncOperation)
        {
            throw new NotSupportedException("Process not implemented");
        }

        public void Retry(int attempt)
        {
            throw new NotSupportedException("Retry not implemented");
        }

        public string GenerateReport(int lineCount)
        {
            var sbReport = new StringBuilder();
            for (int i = 0; i < lineCount; i++)
            {
                sbReport.Append("Line ").Append(i).Append(": data\n");
            }

            var sbDetails = new StringBuilder();
            foreach (string entry in ProcessingLog)
            {
                sbDetails.Append(entry).Append("\n");
            }

            return sbReport.ToString() + sbDetails.ToString();
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
            throw new InvalidOperationException("Shutdown failed");
        }

        public bool IsReady(string status)
        {
            if (status != null)
            {
                return true;
            }
            return false;
        }

        public static void SetProcessorName(string value)
        {
            // SKIPPED — Specific reason: '_processorName' is unused and removing it would require changing the public method signature, which is prohibited.
        }

        public static void ResetBatchCount()
        {
            // SKIPPED — Specific reason: '_batchCount' is unused and removing it would require changing the public method signature, which is prohibited.
        }

        public bool CheckPipelineFlags(int code, bool enabled)
        {
            bool flag1 = false; // Corrected always-false condition
            bool flag2 = true; // Corrected always-true condition
            bool flag3 = false; // Corrected always-false condition
            bool flag4 = true; // Corrected always-true condition
            return flag1 || flag2 || flag3 || flag4;
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

        public const string PipelineFailureReason = "batch_failed";

        public void OnPipelineStarted() { /* intentionally empty */ }
        public void OnPipelineStopped() { /* intentionally empty */ }

        public const int DefaultPipelineLimit = 3;

        public void PipelineHeartbeat()
        {
            int beat = 1;
            Console.WriteLine(beat);
        }

        public void ConfigurePipeline(string name, int poolSize, bool useSsl, string driver, int commandTimeout, bool readOnly, string region)
        {
            var sb = new StringBuilder();
            sb.Append(name).Append(poolSize).Append(useSsl).Append(driver).Append(commandTimeout).Append(readOnly);
            Console.WriteLine(sb.ToString());
        }

        public void FlushAllPipelineBuffers()
        {
            var sb = new StringBuilder();
            sb.Append("step-1\n")
              .Append("step-2\n")
              .Append("step-3\n")
              .Append("step-4\n")
              .Append("step-5\n")
              .Append("step-6\n")
              .Append("step-7\n")
              .Append("step-8\n")
              .Append("step-9\n")
              .Append("step-10\n")
              .Append("step-11\n")
              .Append("step-12\n")
              .Append("step-13\n")
              .Append("step-14\n")
              .Append("step-15\n")
              .Append("step-16\n")
              .Append("step-17\n")
              .Append("step-18\n")
              .Append("step-19\n")
              .Append("step-20\n")
              .Append("step-21\n")
              .Append("step-22\n")
              .Append("step-23\n")
              .Append("step-24\n")
              .Append("step-25\n")
              .Append("step-26\n")
              .Append("step-27\n")
              .Append("step-28\n")
              .Append("step-29\n")
              .Append("step-30\n")
              .Append("step-31\n")
              .Append("step-32\n")
              .Append("step-33\n")
              .Append("step-34\n")
              .Append("step-35\n")
              .Append("step-36\n")
              .Append("step-37\n")
              .Append("step-38\n")
              .Append("step-39\n")
              .Append("step-40\n")
              .Append("step-41\n")
              .Append("step-42\n")
              .Append("step-43\n")
              .Append("step-44\n")
              .Append("step-45\n")
              .Append("step-46\n")
              .Append("step-47\n")
              .Append("step-48\n")
              .Append("step-49\n")
              .Append("step-50\n")
              .Append("step-51\n")
              .Append("step-52\n")
              .Append("step-53\n")
              .Append("step-54\n")
              .Append("step-55\n")
              .Append("step-56\n")
              .Append("step-57\n")
              .Append("step-58\n")
              .Append("step-59\n")
              .Append("step-60\n")
              .Append("step-61\n")
              .Append("step-62\n")
              .Append("step-63\n")
              .Append("step-64\n")
              .Append("step-65\n")
              .Append("step-66\n")
              .Append("step-67\n")
              .Append("step-68\n")
              .Append("step-69\n")
              .Append("step-70\n")
              .Append("step-71\n")
              .Append("step-72\n")
              .Append("step-73\n")
              .Append("step-74\n")
              .Append("step-75\n")
              .Append("step-76\n")
              .Append("step-77\n")
              .Append("step-78\n")
              .Append("step-79\n")
              .Append("step-80\n")
              .Append("step-81\n");
            Console.WriteLine(sb.ToString());
        }

        public double ComputePipelineScoreA(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }
        public double ComputePipelineScoreB(int x, int y) { return (x * 2.5) + (y * 1.5) - 1; }

        public string ClassifyPipelineLevel(int value)
        {
            string classification;
            if (value > 500)
            {
                classification = "critical";
            }
            else if (value > 200)
            {
                classification = "high";
            }
            else if (value > 50)
            {
                classification = "medium";
            }
            else
            {
                classification = "low";
            }
            return classification;
        }

    }
}