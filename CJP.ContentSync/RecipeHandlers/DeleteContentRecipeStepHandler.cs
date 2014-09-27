using System;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Recipes.Models;
using Orchard.Recipes.Services;
using Orchard.Widgets.Models;

namespace CJP.ContentSync.RecipeHandlers
{
    public class DeleteContentRecipeStepHandler : IRecipeHandler
    {
        private readonly IContentManager _contentManager;

        public DeleteContentRecipeStepHandler(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        /*
          <Delete>
            <Content Id="123456789" />
            <Layer Name="Mobile Homepage" />
         */
        //delete content and layers
        public void ExecuteRecipeStep(RecipeContext recipeContext) {
            if (!String.Equals(recipeContext.RecipeStep.Name, "Delete", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var items = recipeContext.RecipeStep.Step.Descendants();

            foreach (var item in items) {
                switch (item.Name.ToString().ToLowerInvariant()) //to consider: we could inject in a collection of interfaces to handle these items to allow for extensibility
                {
                    case "content":
                        var identifier = item.Attribute("Id").Value;
                        var content = _contentManager.Query<IdentityPart, IdentityPartRecord>().Where(c => c.Identifier == identifier).List().FirstOrDefault();
                        if (content != null){
                            _contentManager.Remove(content.ContentItem);    
                        }

                        break;
                    case "layer":
                        var name = item.Attribute("Name").Value;
                        var layer = _contentManager.Query<LayerPart, LayerPartRecord>().Where(l => l.Name == name).List().FirstOrDefault();
                        if (layer != null) {
                            _contentManager.Remove(layer.ContentItem);    
                        }

                        break;
                }
            }

            recipeContext.Executed = true;
        }
    }
}