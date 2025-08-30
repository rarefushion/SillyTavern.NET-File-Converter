namespace SillytavernChatConversions
{
    // Anything without ? means every message will contain these. i.g only user message contain is_system, character responses never contain is_system.
    public class MessageData
    {
        public string name = string.Empty;
        public bool isUser;
        public string message = string.Empty;
        public int fileLine;
        public MessageMetadata metadata = new();
        public bool? isSystem;
        public int? swipeId;
        public string[]? swipes;
        public MessageMetadata[]? swipeMetadata;
    }

    public class MessageMetadata
    {
        public DateTime sendDate;
        public string? reasoning = string.Empty;
        public int? tokenCount;
        public DateTime? genStarted;
        public DateTime? genFinished;
        public string? api;
        public string? model;
        public int? reasoningDurration;
        public int? timeToFirstToken;
        public string? reasoningType;
        public bool? isSmallSys;
        public string? forceAvatar;
        public string? title;
    }
}