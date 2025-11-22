// src/ImageEditor/Core/ConfigurationManager.cs
using System.Collections.Concurrent;

namespace ImageEditor.Core;

/// <summary>
/// Singleton — єдиний менеджер налаштувань у всій програмі
/// </summary>
public sealed class ConfigurationManager
{
    private readonly ConcurrentDictionary<string, string> _settings;
    private const string ConfigPath = "config.ini";

    private ConfigurationManager()
    {
        _settings = new ConcurrentDictionary<string, string>();
        LoadFromFile();
    }

    private static readonly Lazy<ConfigurationManager> _instance 
        = new(() => new ConfigurationManager());

    public static ConfigurationManager Instance => _instance.Value;

    // === Методи доступу ===
    public string Get(string key, string defaultValue = "")
        => _settings.TryGetValue(key, out var value) ? value : defaultValue;

    public void Set(string key, string value)
        => _settings[key] = value;

    public int GetInt(string key, int defaultValue = 0)
        => int.TryParse(Get(key), out var v) ? v : defaultValue;

    public bool GetBool(string key, bool defaultValue = false)
        => bool.TryParse(Get(key), out var v) && v;

    // === Реальні налаштування редактора ===
    public string LastOpenPath
    {
        get => Get("LastOpenPath", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
        set => Set("LastOpenPath", value);
    }

    public string LastTool
    {
        get => Get("LastTool", "Select");
        set => Set("LastTool", value);
    }

    public bool GridVisible
    {
        get => GetBool("GridVisible", false);
        set => Set("GridVisible", value.ToString());
    }

    // === Збереження/завантаження у файл ===
    private void LoadFromFile()
    {
        if (!File.Exists(ConfigPath)) return;

        try
        {
            foreach (var line in File.ReadAllLines(ConfigPath))
            {
                var parts = line.Split('=', 2);
                if (parts.Length == 2)
                    _settings[parts[0].Trim()] = parts[1].Trim();
            }
        }
        catch { /* ігноруємо помилки читання */ }
    }

    public void Save()
    {
        try
        {
            var lines = _settings.Select(kv => $"{kv. Key}={kv.Value}");
            File.WriteAllLines(ConfigPath, lines);
        }
        catch { /* ігноруємо помилки запису */ }
    }
}