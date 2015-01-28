using Orchard;

namespace CJP.ContentSync.RetentionPolicies 
{
    public interface ISnapshotRetentionPolicy : IDependency 
    {
        int[] GetSnapshotIdsToRetain();
    }
}