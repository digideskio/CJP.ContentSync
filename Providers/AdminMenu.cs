using CJP.ContentSync.Permissions;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace CJP.ContentSync.Providers
{
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.AddImageSet("importexport")
                .Add(T("Import/Export"), "42", BuildMenu);
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {
            menu.Add(T("Content Sync"), "10", item => item.Action("Index", "Admin", new { area = "CJP.ContentSync" }).Permission(ApiPermissions.ContentSyncUI).LocalNav());
        }
    }
}