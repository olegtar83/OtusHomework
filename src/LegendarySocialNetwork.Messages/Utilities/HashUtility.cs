using LegendarySocialNetwork.Messages.Database;
using System.Security.Cryptography;
using System.Text;

namespace LegendarySocialNetwork.Messages.Utilities;
public static class HashUtility
{
    public static DatabaseSettings DbSetting { get; set; } = new DatabaseSettings();

    public static int HashIdToShard(string id)
    {
        var shardsCount = DbSetting.NodesCount * DbSetting.ShardsNode;

        byte[] idBytes = Encoding.UTF8.GetBytes(id);
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(idBytes);
            uint hashValue = BitConverter.ToUInt32(hashBytes, 0);
            return (int)(hashValue % shardsCount);
        }
    }
}
