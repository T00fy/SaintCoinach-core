using SaintCoinach.Cmd_Xplatform.Options;
using SaintCoinach.Imaging;
using SixLabors.ImageSharp;

namespace SaintCoinach.Cmd_Xplatform.Runner;

public class UiRunner
{
    private readonly ARealmReversed _realm;

    public UiRunner(ARealmReversed realm)
    {
        _realm = realm;
    }

    public void InvokeRunner(UiOptions options)
    {
        var count = 0;
        for (int i = options.Min; i <= options.Max; ++i)
        {
            try
            {
                count += Process(i, options.Versions, options.Hd);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"{i:D6}: {e.Message}");
            }
        }

        Console.WriteLine($"{count} images processed");
    }

    private int Process(int i, IEnumerable<string> uiVersions, bool hd)
    {
        var count = 0;
        foreach (var v in uiVersions)
        {
            if (Process(i, v, hd))
                ++count;
        }

        return count;
    }

    private bool Process(int i, string version, bool hd)
    {
        string uiImagePathFormat = !hd ? "ui/icon/{0:D3}000{1}/{2:D6}.tex" : "ui/icon/{0:D3}000{1}/{2:D6}_hr1.tex";
        var filePath = string.Format(uiImagePathFormat, i / 1000, version, i);

        if (!_realm.Packs.TryGetFile(filePath, out var file)) return false;
        if (file is not ImageFile imgFile) return false;

        var img = imgFile.GetImage();

        var target = new FileInfo(Path.Combine(_realm.GameVersion, file.Path));
        if (!target.Directory.Exists)
            target.Directory.Create();
        var pngPath = target.FullName.Substring(0, target.FullName.Length - target.Extension.Length) +
                      ".png";
        img.Save(pngPath);

        return true;
    }
}