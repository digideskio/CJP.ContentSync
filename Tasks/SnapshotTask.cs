using System;
using Orchard.ContentManagement;
using Orchard.Tasks.Scheduling;

namespace CJP.ContentSync.Tasks
{
    public class SnapshotTask : IScheduledTask
    {
        public string TaskType { get { return Constants.SnapshotTaskName; } }
        public DateTime? ScheduledUtc { get; set; }
        public ContentItem ContentItem { get { return null; } }
    }
}