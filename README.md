# Sillytavern C# Chat
Reads chats you choose and converts them to the included C# types.
Simply create a SillytavernFileConverter("Path to Chats") giving it the path to your Sillytavern chats. (find your sillytavern file and navigate through sillytavern/default-user/chats/). Once you've made the file converter you can call fileConverter.FetchFileInfo("character name", "file name") so you'll need to know your exact character name and chat name, you can just copy them from the sillytavern ui. Once you have the FileInfo you can create your SillytavernFile with ConvertFile(FileInfo).
Here's an example:
SillytavernFileConverter fileConverter = new("/home/(Your Name)/.local/share/sillytavern/default-user/chats/");
FileInfo fileInfo = fileConverter.FetchFileInfo("Character's name", "(Character's name) - 2025-08-22@19h52m21s");
SillytavernFile stFile = await fileConverter.ConvertFile(fileInfo);
Your "stFile" now contains all messages and lots of extra information.

# Types Overview
```
Everything is inside the SillytavernChatConversions namespace. So you may want to place using SillytavernChatConversions; at the top of your file.
// Don't let the new dotnet context confuse you. All you need to do is new SillytavernFileConverter("Path to Chats"). This is just a way to set the ["Promary Constructor"](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/primary-constructors). 
SillytavernFileConverter(string path)
{
    // Takes your character name and file name or chat/conversation name and returns the FileInfo that you can use in ConverFile.
    FileInfo FetchFileInfo(string character, string fileName)
    // Reads the file from the FileInfo provided and returns a SillytavernFile. It's async so you can either await it or read many files at a time with multithreading.
    async Task<SillytavernFile> ConvertFile(FileInfo fileInfo)
    // These should really only be used by the ConvertFile but they're here if you want to do it manually.
    HeaderDetails ParseHeaderDetails(JsonNode json)
    MessageData ParseMessageData(JsonNode json)
    // Converts the 3 kinds of string dates in sillytavern's jsonl files and converts any of them to a DateTime.
    static DateTime SillyTavernDateToDateTime(string date)
}
// Contains all messages and the header that represents the entire conversation file.
public class SillytavernFile(HeaderDetails header, MessageData[] messages)
{
    public HeaderDetails headerDetails = header;
    public MessageData[] messages = messages;
}
// Represents all messages, yours or the chatbot. If there's a ? after the type that means it doesn't always exist so you'll need to check before using that field.
public class MessageData
{
    // Can be your name or the bots name
    string name = string.Empty;
    // True if you sent the message false if it was the chatbot.
    bool isUser;
    // The whole message but note that this doesn't include reasoning.
    string message = string.Empty;
    MessageMetadata metadata = new();
    // In sillytavern you can use the command /sys that creates a system message among other methods like /?. This is true if it's that kind of message.
    bool? isSystem;
    int? swipeId;
    // These are filled if you create extra swipes for a message. I do not know if your selected swip replaces the "message" above.
    string[]? swipes;
    MessageMetadata[]? swipeMetadata;
}
// If there's a ? after the type that means it doesn't always exist so you'll need to check before using that field.
public class MessageMetadata
{
    DateTime sendDate;
    string reasoning = string.Empty;
    int tokenCount;
    DateTime? genStarted;
    DateTime? genFinished;
    string? api;
    string? model;
    int? reasoningDurration;
    int? timeToFirstToken;
    string? reasoningType;
    bool? isSmallSys;
    string? forceAvatar;
    string? title;
}
// Header represents the first line in all chat jsonl files. It contains information about the conversation.
public class HeaderDetails
{
    string userName = string.Empty;
    string characterName = string.Empty;
    // When the file was created.
    DateTime creationDate;
    // Custom attribute I made to tell you how many lines were present.
    int lineCount;
    ChatMetadata chatMetadata = new();
}

public class ChatMetadata
{
    string integrity = string.Empty;
    long chatIdHash;
    JsonArray? attachments;
    // Your local scope variables are stored here. 
    JsonObject? variables;
    string notePrompt = string.Empty;
    int noteInterval = -1;
    int notePosition = -1;
    int noteDepth = -1;
    int noteRole = -1;
    JsonObject? timedWorldInfo;
    bool tainted = false;
    int lastInContextMessageId = -1;
    // There's sometimes more information here like a quickReply list but I'm not certain how to parse it. If you know the structure feel free to open a bug report or suggest a addition.
}
```
# Contributing
We thrive on community wisdom! To help improve this project:

**Bug Reports**
Please include:
1. The failing SillyTavern file (sanitized if private)
2. Exact error message
3. Environment details (OS/.NET version)

**Feature Requests**
Describe your use case and attach sample JSONL snippets demonstrating new fields needing parsing support.

**Code Contributions**
Submit Pull Requests against `main` with:
- Tests covering new logic
- Concise commit messages

*Your insights make this project unbreakable.* ❤️
