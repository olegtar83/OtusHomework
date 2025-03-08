using ProGaudi.MsgPack.Light;

namespace LegendarySocialNetwork.Counter.DataClasses.Models
{
    [MsgPackArray]
    public class CounterTar
    {
        public CounterTar()
        {

        }
        [MsgPackArrayElement(0)]
        public required string Id { get; set; }
        [MsgPackArrayElement(1)]
        public required int Count { get; set; }
        [MsgPackArrayElement(2)]
        public required string From { get; set; }
        [MsgPackArrayElement(3)]
        public required string To { get; set; }

    }
}
