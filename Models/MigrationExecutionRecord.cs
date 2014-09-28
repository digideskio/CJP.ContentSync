using System;

namespace CJP.ContentSync.Models
{
    public class MigrationExecutionRecord
    {
        public virtual int Id { get; set; }
        public virtual string MigrationName { get; set; }
        public virtual DateTime ExecutedAt { get; set; }
    }
}