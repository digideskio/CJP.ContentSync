using System;
using System.Linq;
using CJP.ContentSync.Services;
using Orchard.Recipes.Models;
using Orchard.Recipes.Services;

namespace CJP.ContentSync.RecipeHandlers
{
    public class ExecutedContentMigrationsRecipeStepHandler : IRecipeHandler
    {
        private readonly IContentMigrationStateService _contentMigrationStateService;

        public ExecutedContentMigrationsRecipeStepHandler(IContentMigrationStateService contentMigrationStateService) {
            _contentMigrationStateService = contentMigrationStateService;
        }

        /*
          <ExecutedContentMigrations>
            <Migration Name="~/Modules/CJP.ContentSync/ContentMigrations/CJP.ContentSync/TestMigration1" />
         */
        //Save any migrations that are in the list, delete migrations that aren't in the list
        public void ExecuteRecipeStep(RecipeContext recipeContext) {
            if (!String.Equals(recipeContext.RecipeStep.Name, "ExecutedContentMigrations", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var migrations = recipeContext.RecipeStep.Step.Descendants();
            var migrationNames = migrations.Where(f => f.Name == "Migration").Select(f => f.Attribute("Name").Value);

            var locallyRanMigrations = _contentMigrationStateService.GetExecutedMigrations().ToList();

            foreach (var migration in locallyRanMigrations.Where(m=>!migrationNames.Contains(m))) {//migrations that have been executed locally, but not in the recipe that is been executed
                _contentMigrationStateService.MarkMigrationAsPending(migration);
            }

            foreach (var migrationToAdd in migrationNames.Where(m=>!locallyRanMigrations.Contains(m))) {
                _contentMigrationStateService.MarkMigrationAsExecuted(migrationToAdd);
            }

            recipeContext.Executed = true;
        }
    }
}