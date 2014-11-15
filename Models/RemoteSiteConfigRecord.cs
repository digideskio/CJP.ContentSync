using System;

namespace CJP.ContentSync.Models {
    public class RemoteSiteConfigRecord
    {
        public virtual int Id { get; set; }
        public virtual string Url { get; set; }
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual DateTime? LastSynced { get; set; }
    }
}