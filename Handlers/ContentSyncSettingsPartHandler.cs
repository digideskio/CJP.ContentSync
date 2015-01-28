using CJP.ContentSync.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Services;
using Orchard.Tasks.Scheduling;

namespace CJP.ContentSync.Handlers 
{
    public class ContentSyncSettingsPartHandler : ContentHandler
    {
        private readonly IScheduledTaskManager _scheduledTaskManager;
        private readonly IClock _clock;

        public ContentSyncSettingsPartHandler(IRepository<ContentSyncSettingsRecord> repository, IScheduledTaskManager scheduledTaskManager, IClock clock)
        {
            _scheduledTaskManager = scheduledTaskManager;
            _clock = clock;
            T = NullLocalizer.Instance;

            Filters.Add(new ActivatingFilter<ContentSyncSettingsPart>("Site"));
            Filters.Add(StorageFilter.For(repository));

            OnUpdated<ContentSyncSettingsPart>((ctx, part) =>
            {
                //schedule the snapshot task
                _scheduledTaskManager.DeleteTasks(null, a => a.TaskType == Constants.TakeSnapshotTaskName);

                if (part.SnapshotFrequencyMinutes == 0)
                {
                    return;
                }

                _scheduledTaskManager.CreateTask(Constants.TakeSnapshotTaskName, _clock.UtcNow, null);
                
            });
        }
        public Localizer T { get; set; }

        protected override void GetItemMetadata(GetContentItemMetadataContext context)
        {
            if (context.ContentItem.ContentType != "Site")
                return;
            base.GetItemMetadata(context);

            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("Content Sync")));
        }
    }
}