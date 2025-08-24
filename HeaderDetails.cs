using System.Text.Json.Nodes;

namespace SillytavernChatConversions
{
    //Information listed at the top of the jsonl file.
    public class HeaderDetails
    {
        public string userName = string.Empty;
        public string characterName = string.Empty;
        public DateTime creationDate;
        public int lineCount;
        public ChatMetadata chatMetadata = new();
    }

    public class ChatMetadata
    {
        public string integrity = string.Empty;
        public long chatIdHash;
        public JsonArray? attachments;
        public JsonObject? variables;
        public string notePrompt = string.Empty;
        public int noteInterval = -1;
        public int notePosition = -1;
        public int noteDepth = -1;
        public int noteRole = -1;
        public JsonObject? timedWorldInfo;
        public bool tainted = false;
        public int lastInContextMessageId = -1;
        // Missing "quickReply":{"setList":[]} 
    }
}