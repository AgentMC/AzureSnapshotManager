using Microsoft.WindowsAzure.Storage.Blob;
using System;

namespace AzuureSnapshotManager.Global
{
    internal static class Extensions
    {
        public static string GetTimeStampHash(this ICloudBlob snapshotBlob)
        {
            return GetTimeStampHash(snapshotBlob.SnapshotTime.Value);
        }
        public static string GetTimeStampHash(this CloudBlob snapshotBlob)
        {
            return GetTimeStampHash(snapshotBlob.SnapshotTime.Value);
        }
        private static string GetTimeStampHash(DateTimeOffset dt)
        {
            return dt.UtcTicks.ToString("D");
        }
    }
}
