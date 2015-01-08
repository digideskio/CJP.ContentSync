using System;
using System.Linq;
using System.Web.Routing;
using Orchard.Alias;
using Orchard.Localization;
using Orchard.Recipes.Models;
using Orchard.Recipes.Services;

namespace CJP.ContentSync.RecipeHandlers
{
    public class AliasesRecipeStepHandler : IRecipeHandler
    {
        private readonly IAliasService _aliasService;

        public AliasesRecipeStepHandler(IAliasService aliasService)
        {
            _aliasService = aliasService;

            T = NullLocalizer.Instance;
        }
        public Localizer T { get; set; }

        /*
          <Aliases>
            <Alias Path="Profile/Edit" Area="Custom.Profile">
              <RouteValues>
                <Add Key="area" Value="Custom.Profile" />
                <Add Key="controller" Value="Profile" />
                <Add Key="action" Value="Edit" />
              </RouteValues>
            </Alias>
         */
        //Enable any features that are in the list, disable features that aren't in the list
        public void ExecuteRecipeStep(RecipeContext recipeContext) {
            if (!String.Equals(recipeContext.RecipeStep.Name, "Aliases", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var aliasElements = recipeContext.RecipeStep.Step.Descendants("Alias");

            foreach (var aliasElement in aliasElements) {
                var path = aliasElement.Attribute("Path").Value;
                var rvd = new RouteValueDictionary();

                var routeValuesElement = aliasElement.Descendants("RouteValues").FirstOrDefault();

                if (routeValuesElement != null) {
                    foreach (var routeValue in routeValuesElement.Descendants("Add")) {
                        rvd.Add(routeValue.Attribute("Key").Value, routeValue.Attribute("Value").Value);
                    }
                }

                _aliasService.Set(path, rvd, "Custom");
            }

            recipeContext.Executed = true;
        }
    }
}