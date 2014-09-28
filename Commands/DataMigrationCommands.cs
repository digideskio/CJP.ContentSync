using System.Linq;
using CJP.ContentSync.Services;
using Orchard.Commands;

namespace CJP.ContentSync.Commands {
    public class DataMigrationCommands : DefaultOrchardCommandHandler {
        private readonly IContentMigrationManager _contentMigrationManager;

        public DataMigrationCommands(IContentMigrationManager contentMigrationManager)
        {
            _contentMigrationManager = contentMigrationManager;
        }

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
            foreach (var message in result.SelectMany(r=>r.Messages))
            {
                Context.Output.WriteLine(T(message));
            }
        }
    }
}