using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;

public class Settings
{
    public const string HmacKey = "E5E278AA1EE34082A088279C83F9BBC806821C52F2AB5D2B4ABD995450355114";

    private const string CONFIG_PATH = "npsSettings.json";
    private static readonly Lazy<Settings> _instance = new Lazy<Settings>(Load);

    // Settings
    public string DownloadDir { get; set; } = @".\downloads\";
    public string PkgPath { get; set; } = @".\pkg2zip.exe";
    public string PkgParams { get; set; } = "-x {pkgFile} \"{zRifKey}\"";
    public bool DeleteAfterUnpack { get; set; } = true;
    public int SimultaneousDl { get; set; } = 2;

    // Game URIs
    public string PsvUri { get; set; } = "https://nopaystation.com/tsv/PSV_GAMES.tsv";
    public string PsmUri { get; set; } = "https://nopaystation.com/tsv/PSM_GAMES.tsv";
    public string PsxUri { get; set; } = "https://nopaystation.com/tsv/PSX_GAMES.tsv";
    public string PspUri { get; set; } = "https://nopaystation.com/tsv/PSP_GAMES.tsv";
    public string Ps3Uri { get; set; } = "https://nopaystation.com/tsv/PS3_GAMES.tsv";
    public string Ps4Uri { get; set; }

    // Avatar URIs
    public string Ps3AvatarUri { get; set; } = "https://nopaystation.com/tsv/PS3_AVATARS.tsv";

    // DLC URIs
    public string PsvDlcUri { get; set; } = "https://nopaystation.com/tsv/PSV_DLCS.tsv";
    public string PspDlcUri { get; set; } = "https://nopaystation.com/tsv/PSP_DLCS.tsv";
    public string Ps3DlcUri { get; set; } = "https://nopaystation.com/tsv/PS3_DLCS.tsv";
    public string Ps4DlcUri { get; set; }

    // Theme URIs
    public string PsvThemeUri { get; set; } = "https://nopaystation.com/tsv/PSV_THEMES.tsv";
    public string PspThemeUri { get; set; } = "https://nopaystation.com/tsv/PSP_THEMES.tsv";
    public string Ps3ThemeUri { get; set; } = "https://nopaystation.com/tsv/PS3_THEMES.tsv";
    public string Ps4ThemeUri { get; set; }

    // Update URIs
    public string PsvUpdateUri { get; set; } = "https://nopaystation.com/tsv/PSV_UPDATES.tsv";
    public string Ps4UpdateUri { get; set; }

    public List<string> SelectedRegions { get; } = new List<string>();
    public List<string> SelectedTypes { get; } = new List<string>();

    public WebProxy Proxy { get; set; }
    public History HistoryInstance { get; } = new History();

    public string CompPackUrl { get; set; } =
        "https://gitlab.com/nopaystation_repos/nps_compati_packs/raw/master/entries.txt";

    public string CompPackPatchUrl { get; set; } =
        "https://gitlab.com/nopaystation_repos/nps_compati_packs/raw/master/entries_patch.txt";

    public static Settings Instance => _instance.Value;

    public static Settings Load()
    {
        Settings settings;
        
        if (File.Exists(CONFIG_PATH))
        {

            try
            {
                var json = File.ReadAllText(CONFIG_PATH);
                settings = JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
            }
            catch (JsonException)
            {
                // fall back to defaults if corrupted
                settings = new Settings();
            }
        }
        else
        {
            settings = new Settings();
        }

        // Only create "./download/" folder if DownloadDir is not null and does not exist
        if (!string.IsNullOrWhiteSpace(settings.DownloadDir) && !Directory.Exists(settings.DownloadDir))
        {
            Directory.CreateDirectory(settings.DownloadDir);
        }

        return settings;
    }

    public void Save()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(this, options);
        File.WriteAllText(CONFIG_PATH, json);
    }
}