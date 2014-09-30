using System.Linq;
using CJP.ContentSync.Models;
using CJP.ContentSync.Services;
using Orchard.Commands;
using Orchard.ImportExport.Services;
using Orchard.Recipes.Services;

namespace CJP.ContentSync.Commands {
    public class DataMigrationCommands : DefaultOrchardCommandHandler {
        private readonly IContentMigrationManager _contentMigrationManager;
        private readonly IContentExportService _contentExportService;
        private readonly IImportExportService _importExportService;
        private readonly IRecipeJournal _recipeJournal;

        public DataMigrationCommands(IContentMigrationManager contentMigrationManager, IContentExportService contentExportService, IImportExportService importExportService, IRecipeJournal recipeJournal) {
            _contentMigrationManager = contentMigrationManager;
            _contentExportService = contentExportService;
            _importExportService = importExportService;
            _recipeJournal = recipeJournal;
        }


        [OrchardSwitch]
        public string Url { get; set; }

        [OrchardSwitch]
        public string Username { get; set; }

        [OrchardSwitch]
        public string Password { get; set; }


        [CommandName("CJP contentsync run migrations")]
        [CommandHelp("CJP contentsync run migrations\r\n\t" + "Runs any pending content migrations")]
        public void RunMigrations()
        {

            var result = _contentMigrationManager.ExecutePendingMigrations().ToList();

            foreach (var migration in result.SelectMany(m => m.SuccessfulMigrations))
            {
                Context.Output.WriteLine(T("Migration '{0}' successfully executed", migration));
            }

            foreach (var migration in result.SelectMany(m => m.FailedMigrations))
            {
                Context.Output.WriteLine(T("Migration '{0}' failed", migration));
            }

            Context.Output.WriteLine(T("Messages:"));
            foreach (var message in result.SelectMany(r => r.Messages))
            {
                Context.Output.WriteLine(T(message));
            }
        }


        [CommandName("CJP contentsync from remote")]
        [CommandHelp("CJP contentsync from remotes\r\n\t" + "Gets an export from the remote site and syncs this site with it")]
        [OrchardSwitches("Url,Username,Password")]
        public void RemoteSync() {
            var result = _contentExportService.GetContentExportFromUrl(Url, Username, Password);


            if (result.Status == ApiResultStatus.Unauthorized)
            {
                Context.Output.WriteLine(T("Either the username and password you supplied is incorrect, or this user does not have the correct permissions to export content"));
                return;
            }

            if (result.Status == ApiResultStatus.Failed)
            {
                Context.Output.WriteLine(T("There was an unexpected error when trying to export the remote site"));
                return;
            }

            Context.Output.WriteLine(T("Site content and configurations have been downloaded and will now be imported"));
            var executionId = _importExportService.Import(result.Text);

            var journal = _recipeJournal.GetRecipeJournal(executionId);

            //if (journal.Status == RecipeStatus.Complete) {
            //    Context.Output.WriteLine(T("You site has been synced with the remote site at {0}", Url));
            //}
            //else
            //{
            //    Context.Output.WriteLine(T("The remote site was successfully exported at {0}, but the import failed:", Url));
            //    foreach (var message in journal.Messages)
            //    {
            //        Context.Output.WriteLine(T(message.Message));
            //    }
            //}
        }
    }
}