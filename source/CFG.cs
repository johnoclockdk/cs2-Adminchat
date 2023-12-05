using CounterStrikeSharp.API.Modules.Utils;
using System.IO;
using System.Reflection;
using System.Text.Json;


namespace AdminChat;

internal class Cfg
{
    public static Config Config = new();
    private FileSystemWatcher? watcher;

    /// <summary>
    /// Checks the configuration file for the module and creates it if it does not exist.
    /// </summary>
    /// <param name="moduleDirectory">The directory where the module is located.</param>
    public void CheckConfig(string moduleDirectory)
    {
        string path = Path.Join(moduleDirectory, "config.json");

        if (!File.Exists(path))
        {
            CreateAndWriteFile(path);
        }

        using (FileStream fs = new(path, FileMode.Open, FileAccess.Read))
        using (StreamReader sr = new(fs))
        {
            // Deserialize the JSON from the file and load the configuration.
            Config = JsonSerializer.Deserialize<Config>(sr.ReadToEnd())!;
        }

        foreach (PropertyInfo prop in Config.GetType().GetProperties())
        {
            if (prop.PropertyType != typeof(string))
            {
                continue;
            }

            prop.SetValue(Config, ModifyColorValue(prop.GetValue(Config)!.ToString()!));
        }

        // Create a new FileSystemWatcher and set its properties.
        watcher = new FileSystemWatcher();
        watcher.Path = moduleDirectory;
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        watcher.Filter = "config.json";

        // Add event handlers.
        watcher.Changed += OnChanged;

        // Begin watching.
        watcher.EnableRaisingEvents = true;
    }

    /// <summary>
    /// Creates a new file at the specified path and writes the default configuration settings to it.
    /// </summary>
    /// <param name="path">The path where the file should be created.</param>
    private static void CreateAndWriteFile(string path)
    {

        using (FileStream fs = File.Create(path))
        {
            // File is created, and fs will automatically be disposed when the using block exits.
        }

        Console.WriteLine($"File created: {File.Exists(path)}");

        Config = new Config
        {
            ChatPrefix = "[{darkred}Admin-Chat{white}]",
            ChatHighlight = "{lightred}",
            ChatText = "{white}",
        };

        // Serialize the config object to JSON and write it to the file.
        string jsonConfig = JsonSerializer.Serialize(Config, new JsonSerializerOptions()
        {
            WriteIndented = true
        });
        File.WriteAllText(path, jsonConfig);
    }

    // Essential method for replacing chat colors from the config file, the method can be used for other things as well.
    private string ModifyColorValue(string msg)
    {
        if (!msg.Contains('{'))
        {
            return string.IsNullOrEmpty(msg) ? "" : msg;
        }

        string modifiedValue = msg;

        foreach (FieldInfo field in typeof(ChatColors).GetFields())
        {
            string pattern = $"{{{field.Name}}}";
            if (msg.Contains(pattern, StringComparison.OrdinalIgnoreCase))
            {
                modifiedValue = modifiedValue.Replace(pattern, field.GetValue(null)!.ToString(), StringComparison.OrdinalIgnoreCase);
            }
        }
        return modifiedValue;
    }

    // Define the event handlers.
    private void OnChanged(object source, FileSystemEventArgs e)
    {
        // Specify what is done when a file is changed, created, or deleted.
        Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
    }
}

internal class Config
{
    public string? ChatPrefix { get; set; }
    public string? ChatHighlight { get; set; }
    public string? ChatText { get; set; }
}
