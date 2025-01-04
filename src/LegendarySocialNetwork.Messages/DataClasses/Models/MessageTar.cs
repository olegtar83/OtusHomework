using ProGaudi.MsgPack.Light;

namespace LegendarySocialNetwork.Messages.DataClasses.Models
{
    [MsgPackArray]
    public class MessageTar
    {
        public MessageTar()
        {
            
        }
        [MsgPackArrayElement(0)]
        public required int Id { get; set; }
        [MsgPackArrayElement(1)]

        public required string From { get; set; }
        [MsgPackArrayElement(2)]

        public required string To { get; set; }
        [MsgPackArrayElement(3)]

        public required string Text { get; set; }

    }
}
