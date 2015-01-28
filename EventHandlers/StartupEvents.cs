using Orchard.Environment;
using Orchard.Services;
using Orchard.Tasks.Scheduling;

namespace CJP.ContentSync.EventHandlers 
{
    public class StartupEvents : IOrchardShellEvents
    {
        private readonly IScheduledTaskManager _scheduledTaskManager;
        private readonly IClock _clock;
        public StartupEvents(IScheduledTaskManager scheduledTaskManager, IClock clock) 
        {
            _scheduledTaskManager = scheduledTaskManager;
            _clock = clock;
        }

        public void Activated()
        {
            _scheduledTaskManager.DeleteTasks(null, a => a.TaskType == Constants.CleanupSnapshots);
            _scheduledTaskManager.CreateTask(Constants.CleanupSnapshots, _clock.UtcNow, null);
        }

        public void Terminating() {}
    }
}