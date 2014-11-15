using System;

namespace CJP.ContentSync.Models {
    public class SnapshotRecord
    {
        public virtual int Id { get; set; }
        public virtual DateTime TimeTaken { get; set; }
        public virtual string Data { get; set; }
    }
}