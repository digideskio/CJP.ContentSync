using System;
using System.Net;
using System.Web.Mvc;
using CJP.ContentSync.Permissions;
using CJP.ContentSync.Services;
using Orchard.Environment.Configuration;
using Orchard.Security;
using Orchard.Utility.Extensions;

namespace CJP.ContentSync.Controllers.API
{
    public class ContentExportController : Controller {
        private readonly IContentExportService _contentExportService;
        private readonly IMembershipService _membershipService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizer _authorizer;
        private readonly ISnapshotService _snapshotService;
        private readonly ShellSettings _shellSettings;

        public ContentExportController(IContentExportService contentExportService,
            IMembershipService membershipService,
            IAuthenticationService authenticationService,
            IAuthorizer authorizer,
            ISnapshotService snapshotService,
            ShellSettings shellSettings) {
            _contentExportService = contentExportService;
            _membershipService = membershipService;
            _authenticationService = authenticationService;
            _authorizer = authorizer;
            _snapshotService = snapshotService;
            _shellSettings = shellSettings;
        }


        [HttpGet]
        public ActionResult Index(string username, string password, bool takeFreshSnapshot = false) {
            var user = _membershipService.ValidateUser(username, password);

            if (user == null) {
                Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                Response.End();
                return new HttpUnauthorizedResult();
            }

            _authenticationService.SignIn(user, false);

            if (!_authorizer.Authorize(ContentSyncPermissions.ContentExportApi)) {
                Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                Response.End();
                return new HttpUnauthorizedResult();
            }


            if (takeFreshSnapshot) {
                _snapshotService.TakeSnaphot();
            }

            var snapshot = _snapshotService.GetLatestSnaphot();

            if (snapshot == null) {
                Response.StatusCode = (int) HttpStatusCode.NotFound;
                Response.End();
                return new HttpNotFoundResult("No snapshots were available to download. Either manually create a snapshot on the remote site, or add 'takeFreashSnapshot=true' to the querystring of this request.");
            }

            var fileName = string.Format("Snapshot-{0}-taken-{1:yyyyMMddHHmmss}-from-{2}.xml", snapshot.Id, snapshot.TimeTaken, _shellSettings.Name);

            return File(GetBytes(snapshot.Data), "text/xml", fileName);
        }


        private static byte[] GetBytes(string str) 
        {
            var bytes = new byte[str.Length*sizeof (char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}