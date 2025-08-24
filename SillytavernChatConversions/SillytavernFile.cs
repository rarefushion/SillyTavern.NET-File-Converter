namespace SillytavernChatConversions
{
    public class SillytavernFile(HeaderDetails header, MessageData[] messages)
    {
        public HeaderDetails headerDetails = header;
        public MessageData[] messages = messages;
    }
}