using System.Globalization;
using System.Text.Json.Nodes;

namespace SillytavernChatConversions
{
    public class SillytavernFileConverter(string path)
    {
        readonly DirectoryInfo chatsDirectory = new(path);

        public FileInfo FetchFileInfo(string character, string fileName)
        {
            DirectoryInfo characterPath = chatsDirectory
                .GetDirectories()
                .FirstOrDefault(d => d.Name == character)
                ?? throw new Exception($"Character \"{character}\" does not exist in \"{chatsDirectory.FullName}\".");

            fileName = Path.ChangeExtension(fileName, ".jsonl");
            FileInfo fileInfo = characterPath
                .GetFiles()
                .FirstOrDefault(f => f.Name == fileName)
                ?? throw new Exception($"File \"{fileName}\" does not exist in \"{characterPath.FullName}\".");

            return fileInfo;
        }

        public async Task<SillytavernFile> ConvertFile(FileInfo fileInfo)
        {
            HeaderDetails? headerDetails = null;
            List<MessageData> messages = [];
            int index = 0;
            using StreamReader reader = new(fileInfo.FullName);
            while (await reader.ReadLineAsync() is { } line)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                JsonNode? json = JsonNode.Parse(line);
                if (json == null)
                    continue;

                if (index == 0) // Header
                    headerDetails = ParseHeaderDetails(json);
                else
                    messages.Add(ParseMessageData(json));
                index++;
            }
            if (index == 0)
                throw new Exception("Error parsing SillyTavern file: No lines were parsed.");
            headerDetails!.lineCount = index;
            if (messages.Count == 0)
                throw new Exception("Error parsing SillyTavern file: No messages were parsed.");
            return new(headerDetails, [.. messages]);
        }

        public HeaderDetails ParseHeaderDetails(JsonNode json)
        {
            JsonNode metadata = json["chat_metadata"]!;
            return new()
            {
                userName = json["user_name"]!.ToString(),
                characterName = json["character_name"]!.ToString(),
                creationDate = SillyTavernDateToDateTime(json["create_date"]!.ToString()),
                chatMetadata = new()
                {
                    integrity = metadata["integrity"]?.ToString() ?? string.Empty,
                    chatIdHash = (long)metadata["chat_id_hash"]!,
                    attachments = metadata["attachments"] as JsonArray,
                    variables = metadata["variables"] as JsonObject,
                    notePrompt = metadata["note_prompt"]?.ToString() ?? string.Empty,
                    noteInterval = (int?)metadata["note_interval"] ?? -1,
                    notePosition = (int?)metadata["note_position"] ?? -1,
                    noteDepth = (int?)metadata["note_depth"] ?? -1,
                    noteRole = (int?)metadata["note_role"] ?? -1,
                    timedWorldInfo = metadata["timedWorldInfo"] as JsonObject,
                    tainted = (bool?)metadata["tainted"] ?? false,
                    lastInContextMessageId = (int?)metadata["lastInContextMessageId"] ?? -1
                }
            };
        }

        public MessageData ParseMessageData(JsonNode json)
        {
            JsonNode extra = json["extra"]!;
            return new()
            {
                name = (string)json["name"]!,
                isUser = (bool)json["is_user"]!,
                isSystem = (bool?)json["is_system"],
                message = (string)json["mes"]!,
                metadata = new()
                {
                    sendDate = DateTime.Parse((string)json["send_date"]!),
                    reasoning = (string)extra["reasoning"]!,
                    tokenCount = (int)extra["token_count"]!,
                    genStarted = (json["gen_started"] != null) ? SillyTavernDateToDateTime(json["gen_started"]!.ToString()) : null,
                    genFinished = (json["gen_finished"] != null) ? SillyTavernDateToDateTime(json["gen_finished"]!.ToString()) : null,
                    api = (string?)extra["api"],
                    model = (string?)extra["model"],
                    reasoningDurration = (int?)extra["reasoning_duration"],
                    timeToFirstToken = (int?)extra["time_to_first_token"],
                    reasoningType = (string?)extra["reasoning_type"],
                    isSmallSys = (bool?)extra["isSmallSys"],
                    forceAvatar = (string?)json["force_avatar"],
                    title = (string?)json["title"]
                },
                swipeId = (int?)json["swipe_id"],
                swipes = (string[]?)json["swipes"]?.AsArray()!.Select(x => x?.ToString() ?? string.Empty).ToArray() ?? null,
                swipeMetadata = json["swipe_info"]?.AsArray()?.Select(x => new MessageMetadata()
                {
                    sendDate = DateTime.Parse((string)x!["send_date"]!),
                    reasoning = (string)x!["extra"]!["reasoning"]!,
                    tokenCount = (int)x!["extra"]!["token_count"]!,
                    genStarted = (x!["gen_started"] != null) ? SillyTavernDateToDateTime(x!["gen_started"]!.ToString()) : null,
                    genFinished = (x!["gen_finished"] != null) ? SillyTavernDateToDateTime(x!["gen_finished"]!.ToString()) : null,
                    api = (string?)x!["extra"]!["api"],
                    model = (string?)x!["extra"]!["model"],
                    reasoningDurration = (int?)x!["extra"]!["reasoning_duration"],
                    timeToFirstToken = (int?)x!["extra"]!["time_to_first_token"],
                    reasoningType = (string?)x!["extra"]!["reasoning_type"],
                    isSmallSys = (bool?)x!["extra"]!["isSmallSys"]
                }).ToArray()
            };
        }

        public static DateTime SillyTavernDateToDateTime(string date)
        {
            string[] formats =
            [
                "MMMM d, yyyy h:mmtt", // message send_date
                "yyyy-MM-ddTHH:mm:ss.fffZ", // bot response gen_started and finished
                "yyyy-MM-dd@H'h'm'm's's'" // Header create_date
            ];
            if (DateTime.TryParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                return parsedDate;
            return DateTime.Parse(date);
        }
    }
}