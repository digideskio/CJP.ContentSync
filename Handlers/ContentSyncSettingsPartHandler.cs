using CJP.ContentSync.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Orchard.Services;
using Orchard.Tasks.Scheduling;

namespace CJP.ContentSync.Handlers 
{
    public class ContentSyncSettingsPartHandler : ContentHandler
    {
        private readonly IScheduledTaskManager _scheduledTaskManager;
        private readonly IClock _clock;

        public ContentSyncSettingsPartHandler(IScheduledTaskManager scheduledTaskManager, IClock clock)
        {
            _scheduledTaskManager = scheduledTaskManager;
            _clock = clock;
            T = NullLocalizer.Instance;

            Filters.Add(new ActivatingFilter<ContentSyncSettingsPart>("Site"));

            OnUpdated<ContentSyncSettingsPart>((ctx, part) =>
            {
                // Schedule the snapshot task.
                _scheduledTaskManager.DeleteTasks(null, a => a.TaskType == Constants.TakeSnapshotTaskName);

                if (part.SnapshotFrequencyMinutes == 0)
                    return;

                _scheduledTaskManager.CreateTask(Constants.TakeSnapshotTaskName, _clock.UtcNow, null);
                
            });

            OnGetContentItemMetadata<ContentSyncSettingsPart>((ctx, part) => ctx.Metadata.EditorGroupInfo.Add(new GroupInfo(T("Content Sync"))));
        }

        public Localizer T { get; set; }
    }
}