using Microsoft.WindowsAzure.Storage.Blob;

namespace AzuureSnapshotManager.Global
{
    internal static class Extensions
    {
        public static string GetTimeStampHash(this ICloudBlob snapshotBlob)
        {
            return snapshotBlob.SnapshotTime.Value.UtcTicks.ToString("D");
        }
        public static string GetTimeStampHash(this CloudBlob snapshotBlob)
        {
            return snapshotBlob.SnapshotTime.Value.UtcTicks.ToString("D");
        }
    }
}
