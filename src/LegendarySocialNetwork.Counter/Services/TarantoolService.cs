using LegendarySocialNetwork.Counter.DataClasses.Models;
using MediatR;
using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;

namespace LegendarySocialNetwork.Counter.Services
{
    public interface ITarantoolService
    {
        Task<Result<int>> GetMessageCounter(string from, string to);
        Task<Result<Unit>> CountMessageIncrement(string from, string to);
        Task<Result<Unit>> CountMessageDecrement(string from, string to);
    }
    public class TarantoolService : ITarantoolService
    {
        private readonly string connStr = Environment.GetEnvironmentVariable("Tarantool:ConnStr")!;
        private readonly Box _box;
        public TarantoolService()
        {
            var clientOptions = new ClientOptions(connStr);
            _box = new Box(clientOptions);
            _box.Connect().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task<Result<Unit>> CountMessageIncrement(string from, string to)
        {
            await _box.Call("increment_counter", TarantoolTuple.Create(from, to));

            return Result<Unit>.Success(Unit.Value);
        }

        public async Task<Result<Unit>> CountMessageDecrement(string from, string to)
        {
            await _box.Call("decrement_counter", TarantoolTuple.Create(from, to));

            return Result<Unit>.Success(Unit.Value);
        }

        public async Task<Result<int>> GetMessageCounter(string from, string to)
        {
            var res = await _box.Call_1_6<TarantoolTuple<string, string>, TarantoolTuple<int>> ("get_counter",TarantoolTuple.Create(from, to));

            if (res.Data == null || res.Data.Count() == 0 || res.Data[0].Item1 == 0)
            {
                return Result<int>.Failure("No data received from Tarantool.");
            }
            var row = res.Data[0].Item1;

            return Result<int>.Success(row);
        }

    }
}
