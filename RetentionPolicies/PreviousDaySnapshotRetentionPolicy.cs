using System.Linq;
using CJP.ContentSync.Models;
using Orchard.Data;
using Orchard.Services;

namespace CJP.ContentSync.RetentionPolicies
{
    public class PreviousDaySnapshotRetentionPolicy : ISnapshotRetentionPolicy
    {
        private readonly IRepository<SnapshotRecord> _repository;
        private readonly IClock _clock;

        public PreviousDaySnapshotRetentionPolicy(IRepository<SnapshotRecord> repository, IClock clock)
        {
            _repository = repository;
            _clock = clock;
        }

        public int[] GetSnapshotIdsToRetain()
        {//keep any snapshots taken in the past 24 hours
            return _repository.Table.Where(r => r.TimeTaken > _clock.UtcNow.AddHours(-24)).Select(r => r.Id).ToArray();
        }
    }
}