using System.Data;
using Newtonsoft.Json;

namespace Drawer_Watcher.Localization;

public class Language
{
    public string Name { get; set; }
    public Dictionary<string, string> Data { get; set; }
}

public static class LanguageSystem
{
    private static Language _selectedLanguage = null!;
    public static Dictionary<string, Language> Languages = null!;

    public static int SelectedLanguageIndex = 0;
    
    public static Language CurrentLanguage => _selectedLanguage;

    public static void Init()
    {
        Languages = new Dictionary<string, Language>();
        GetAllAvalibleLanguages();
        _selectedLanguage = Languages["English"];
    }

    public static void SelectLanguage(string lang)
    {
        _selectedLanguage = Languages[lang];
    }

    private static void GetAllAvalibleLanguages()
    {
        var files = Directory.GetFiles("Assets/Languages/", "*.json");
        foreach (var filePath in files)
        {
            var lang = JsonConvert.DeserializeObject<Language>(File.ReadAllText(filePath));
            if (lang == null)
                throw new ArgumentNullException(nameof(lang));
            Languages.Add(lang.Name, lang);
        }
    }

    public static string GetLocalized(string value)
    {
        return _selectedLanguage.Data[value];
    }
}