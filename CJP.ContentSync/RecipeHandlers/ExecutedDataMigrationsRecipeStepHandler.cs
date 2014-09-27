using System;
using System.Linq;
using CJP.ContentSync.Models;
using Orchard.Data;
using Orchard.Environment.Features;
using Orchard.Recipes.Models;
using Orchard.Recipes.Services;
using Orchard.Services;

namespace CJP.ContentSync.RecipeHandlers
{
    public class ExecutedDataMigrationsRecipeStepHandler : IRecipeHandler
    {
        private readonly IRepository<MigrationExecutionRecord> _repository;
        private readonly IClock _clock;

        public ExecutedDataMigrationsRecipeStepHandler(IRepository<MigrationExecutionRecord> repository, IClock clock) {
            _repository = repository;
            _clock = clock;
        }

        /*
          <ExecutedDataMigrations>
            <Migration Name="~/Modules/CJP.ContentSync/ContentMigrations/CJP.ContentSync/TestMigration1" />
         */
        //Save any migrations that are in the list, delete migrations that aren't in the list
        public void ExecuteRecipeStep(RecipeContext recipeContext) {
            if (!String.Equals(recipeContext.RecipeStep.Name, "ExecutedDataMigrations", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var migrations = recipeContext.RecipeStep.Step.Descendants();
            var migrationNames = migrations.Where(f => f.Name == "Migration").Select(f => f.Attribute("Name").Value);

            var locallyRanMigrations = _repository.Table.ToList();

            foreach (var migrationExecutionRecord in locallyRanMigrations.Where(m=>!migrationNames.Contains(m.MigrationName))) {//migrations that have been executed locally, but not in the recipe that is been executed
                _repository.Delete(migrationExecutionRecord);
            }

            foreach (var migrationToAdd in migrationNames.Where(m=>!locallyRanMigrations.Select(lm=>lm.MigrationName).Contains(m))) {
                _repository.Create(new MigrationExecutionRecord{MigrationName = migrationToAdd, ExecutedAt = _clock.UtcNow});
            }

            recipeContext.Executed = true;
        }
    }
}