using UnityEngine;


public static class Options
{
    public enum Resolution {
        Fullscreen,
        Windowed720,
        Windowed480
    }

    public enum Language { 
        EN,
        VI,
        };
    public enum Server { 
        Asia,
        Australia,
        CanadaEast,
        Europe,
        India,
        Japan,
        RussiaEast,
        RussiaWest,
        SouthAfrica,
        SouthAmerica,
        SouthKorea,
        Turkey,
        USAEast,
        USAWest }

    #region Properties


    public static Resolution ActiveResolution { get; set; }
    public static Language ActiveLanguage { get; set; }
    public static Server ActiveServer { get; set; }

    #endregion
    public static void SaveOptions()
    {
        SettingsData data = new SettingsData
        {
            resolution = ActiveResolution,
            language = ActiveLanguage,
            server = ActiveServer
        };

        SaveManager.SaveSettings(data);
    }

    public static void LoadOptions()
    {
        SettingsData data = SaveManager.LoadSettings();

        ActiveResolution = data.resolution;
        ActiveLanguage = data.language;
        ActiveServer = data.server;
    }
    public static void DefaultValues()
    {

        ActiveResolution = Resolution.Windowed480;

        if (Application.systemLanguage == SystemLanguage.Vietnamese)
        {
            ActiveLanguage = Language.VI;
        }
        else
        {
            ActiveLanguage = Language.EN;
        }

      

        ActiveServer = Server.Europe;

        SaveOptions();
    }
}