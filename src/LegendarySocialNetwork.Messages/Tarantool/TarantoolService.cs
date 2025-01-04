using LegendarySocialNetwork.Messages.DataClasses.Models;
using ProGaudi.MsgPack.Light;
using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;
using ProGaudi.Tarantool.Client.Model.Enums;

namespace LegendarySocialNetwork.Messages.Tarantool
{
    public interface ITarantoolService
    {
        Task<int> Sync();
        Task<Result<List<MessageTar>>> GetDialogsAsync(string userId);
    }
    public class TarantoolService : ITarantoolService
    {
        private readonly string connStr = Environment.GetEnvironmentVariable("Tarantool:ConnStr")!;
        private readonly string spaceName = "messages";
        private readonly string spaceFromIndex = "from_index";
        private readonly string spaceToIndex = "to_index";
        private readonly ILogger<TarantoolService> _logger;
        private readonly Box _box;
        private readonly ISpace space;
        private readonly IIndex secondaryIndexFrom;
        private readonly IIndex secondaryIndexTo;

        public TarantoolService(ILogger<TarantoolService> logger)
        {
            _logger = logger;
            var msgPackContext = new MsgPackContext();
            msgPackContext.GenerateAndRegisterArrayConverter<MessageTar>();

            var clientOptions = new ClientOptions(connStr, context: msgPackContext);
            _box = new Box(clientOptions);
            _box.Connect().ConfigureAwait(false).GetAwaiter().GetResult();
            space = _box.Schema[spaceName];
            secondaryIndexFrom = space[spaceFromIndex];
            secondaryIndexTo = space[spaceToIndex];
        }

        public async Task<int> Sync()
        {
            var res = await _box.Call<int>("sync");

            if(res.Data == null)
            {
                _logger.LogError(string.Join(',', res.MetaData));
                return 0;
            }    

            return res.Data[0];
        }

        public async Task<Result<List<MessageTar>>> GetDialogsAsync(string userId)
        {
            var resFrom = await secondaryIndexFrom.Select<TarantoolTuple<string>, MessageTar>(TarantoolTuple.Create(userId), new SelectOptions { Iterator = Iterator.Eq });

            var resTo = await secondaryIndexTo.Select<TarantoolTuple<string>, MessageTar>(TarantoolTuple.Create(userId), new SelectOptions { Iterator = Iterator.Eq }); ;

            if(resFrom.Data == null && resTo.Data == null)
            {
                return Result<List<MessageTar>>.Failure(string.Join(',', resFrom.MetaData));
            }

            var result = resFrom.Data!.Union
                (resTo.Data!).ToList();

            return Result<List<MessageTar>>.Success(result);
        }
    }
}
