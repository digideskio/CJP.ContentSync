using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace CJP.ContentSync.Permissions
{
    public class ApiPermissions : IPermissionProvider
    {
        public static readonly Permission ContentSyncUI = new Permission { Description = "Provides access to the content sync UI", Name = "ContentSyncUI" };
        public static readonly Permission ContentExportApi = new Permission { Description = "Allows a user's credentials to be used to autheticate a Content Export API request", Name = "ContentExportApi" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                ContentExportApi,
                ContentSyncUI
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ContentSyncUI}
                }
            };
        }
    }
}


