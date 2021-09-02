using System;

namespace AppCenter.Models
{
    public class BranchBuildResponse
    {
        public int id { get; set; }
        public string buildNumber { get; set; }
        public DateTime queueTime { get; set; }
        public DateTime startTime { get; set; }
        public DateTime finishTime { get; set; }
        public DateTime lastChangedDate { get; set; }
        public string status { get; set; }
        public string result { get; set; }
        public string reason { get; set; }
        public string sourceBranch { get; set; }
        public string sourceVersion { get; set; }
        public string[] tags { get; set; }
    }
}
