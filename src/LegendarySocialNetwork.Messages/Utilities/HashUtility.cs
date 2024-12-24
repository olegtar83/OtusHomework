using LegendarySocialNetwork.Messages.Database;
using System.Security.Cryptography;
using System.Text;

namespace LegendarySocialNetwork.Messages.Utilities
{
    public static class HashUtility
    {
        public static DatabaseSettings DbSettings = new DatabaseSettings();
        public static int GetHashId(string from, string to)
        {
            string combinedString = from + to;

            using SHA256 sha256 = SHA256.Create();

            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedString));

            int hashValue = BitConverter.ToInt32(hashBytes, 0);

            var shardCount = DbSettings.ShardsNode * DbSettings.NodesCount;

            return Math.Abs(hashValue % shardCount);
        }
    }
}
