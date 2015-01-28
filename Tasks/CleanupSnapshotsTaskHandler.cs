using System;
using System.Collections.Generic;
using System.Linq;
using CJP.ContentSync.Models;
using CJP.ContentSync.RetentionPolicies;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Services;
using Orchard.Tasks.Scheduling;

namespace CJP.ContentSync.Tasks 
{
    public class CleanupSnapshotsTaskHandler : IScheduledTaskHandler 
    {
        private readonly IRepository<SnapshotRecord> _repository;
        private readonly IEnumerable<ISnapshotRetentionPolicy> _retentionPolicies;
        private readonly IScheduledTaskManager _scheduledTaskManager;
        private readonly IClock _clock;

        public ILogger Logger { get; set; }

        public CleanupSnapshotsTaskHandler(IRepository<SnapshotRecord> repository, 
            IEnumerable<ISnapshotRetentionPolicy> retentionPolicies, 
            IScheduledTaskManager scheduledTaskManager, 
            IClock clock) 
        {
            _repository = repository;
            _retentionPolicies = retentionPolicies;
            _scheduledTaskManager = scheduledTaskManager;
            _clock = clock;

            Logger = NullLogger.Instance;
        }

        public void Process(ScheduledTaskContext context) 
        {
            if (context.Task.TaskType != Constants.CleanupSnapshots) 
            {
                return;
            }

            try 
            {
                var idsToRetain = new List<int>();

                foreach (var retentionPolicy in _retentionPolicies) {
                    idsToRetain.AddRange(retentionPolicy.GetSnapshotIdsToRetain());
                }

                var recordsToDelete = _repository.Table.Where(r => !idsToRetain.Contains(r.Id));

                foreach (var record in recordsToDelete) 
                {
                    _repository.Delete(record);
                }
            }
            catch (Exception ex) 
            {
                Logger.Error(ex, "Failed to cleanup Content Sync Snapshots. An exception was thrown by the task handler.");
            }

            //now reschedule the task
            _scheduledTaskManager.DeleteTasks(null, a => a.TaskType == Constants.CleanupSnapshots);
            _scheduledTaskManager.CreateTask(Constants.CleanupSnapshots, _clock.UtcNow.AddMinutes(1), null);
        }
    }
}