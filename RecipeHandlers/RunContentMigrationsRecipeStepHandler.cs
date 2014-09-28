using System;
using CJP.ContentSync.Services;
using Orchard.Recipes.Models;
using Orchard.Recipes.Services;

namespace CJP.ContentSync.RecipeHandlers
{
    public class RunContentMigrationsRecipeStepHandler : IRecipeHandler
    {
        private readonly IContentMigrationManager _contentMigrationManager;

        public RunContentMigrationsRecipeStepHandler(IContentMigrationManager contentMigrationManager) {
            _contentMigrationManager = contentMigrationManager;
        }

        /*
          <RunContentMigrations />
         */
        //Run any pending content migrations
        public void ExecuteRecipeStep(RecipeContext recipeContext) {
            if (!String.Equals(recipeContext.RecipeStep.Name, "RunContentMigrations", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            _contentMigrationManager.ExecutePendingMigrations();

            recipeContext.Executed = true;
        }
    }
}