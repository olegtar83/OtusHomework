namespace LegendarySocialNetwork.Application.Common.Caching
{
    public static class CacheKeys
    {
        public static class Feed
        {
            private const string FeedKey = "feed";
            public static string ForUser(string userId) => $"{FeedKey}-{userId}";
        }
    }
}