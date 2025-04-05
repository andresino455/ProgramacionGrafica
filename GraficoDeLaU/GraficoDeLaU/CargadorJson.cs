using System.IO;
using System.Text.Json;

public static class JsonLoader
{
    public static T LoadFromJson<T>(string filePath)
    {
        string jsonString = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<T>(jsonString);
    }
}
