using System;
using CJP.ContentSync.Models;
using CJP.ContentSync.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Settings.Models;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Services;
using Orchard.Tasks.Scheduling;

namespace CJP.ContentSync.Tasks
{
    public class TakeSnapshotTaskHandler : IScheduledTaskHandler
    {
        private readonly ISnapshotService _snapshotService;
        private readonly IScheduledTaskManager _scheduledTaskManager;
        private readonly IClock _clock;
        private readonly IOrchardServices _orchardServices;
        private readonly IMembershipService _membershipService;
        private readonly IAuthenticationService _authenticationService;

        public ILogger Logger { get; set; }

        public TakeSnapshotTaskHandler(ISnapshotService snapshotService, 
                IScheduledTaskManager scheduledTaskManager, 
                IClock clock, 
                IOrchardServices orchardServices, 
                IMembershipService membershipService, 
                IAuthenticationService authenticationService) 
        {
            _snapshotService = snapshotService;
            _scheduledTaskManager = scheduledTaskManager;
            _clock = clock;
            _orchardServices = orchardServices;
            _membershipService = membershipService;
            _authenticationService = authenticationService;

            Logger = NullLogger.Instance;
        }

        public void Process(ScheduledTaskContext context) 
        {
            if (context.Task.TaskType != Constants.TakeSnapshotTaskName) 
            {
                return;
            }

            try {
                //the default export service impl makes an assumption that there is an associated user, so we need to set the owner here
                var siteOwner = _membershipService.GetUser(_orchardServices.WorkContext.CurrentSite.As<SiteSettingsPart>().SuperUser);
                _authenticationService.SetAuthenticatedUserForRequest(siteOwner);

                _snapshotService.TakeSnaphot();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to generate a scheduled Content Sync Snapshot. An exception was thrown by the task handler.");
            }

            //now reschedule the task
            _scheduledTaskManager.DeleteTasks(null, a => a.TaskType == Constants.TakeSnapshotTaskName);

            var snapshotFrequency = _orchardServices.WorkContext.CurrentSite.As<ContentSyncSettingsPart>().SnapshotFrequencyMinutes;

            if (snapshotFrequency == 0) 
            {
                return;
            }

            _scheduledTaskManager.CreateTask(Constants.TakeSnapshotTaskName, _clock.UtcNow.AddMinutes(snapshotFrequency), null);
        }
    }
}