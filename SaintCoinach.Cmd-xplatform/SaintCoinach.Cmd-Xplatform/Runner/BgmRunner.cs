using SaintCoinach.Cmd_Xplatform.Options;
using SaintCoinach.Sound;
using SaintCoinach.Xiv;

namespace SaintCoinach.Cmd_Xplatform;

public class BgmRunner
{
    private readonly ARealmReversed _realm;

    public BgmRunner(ARealmReversed realm)
    {
        _realm = realm;
    }

    public void InvokeRunner(BgmOptions options)
    {
        var bgms = _realm.GameData.GetSheet("BGM");
        var searchStrings = options.FilesToExport.ToList();

        var successCount = 0;
        var failCount = 0;
        foreach (IXivRow bgm in bgms)
        {
            var filePath = bgm["File"].ToString();

            try
            {
                if (string.IsNullOrWhiteSpace(filePath) || !IsMatch(searchStrings, filePath))
                    continue;

                if (ExportFile(filePath, null))
                {
                    ++successCount;
                }
                else
                {
                    Console.Error.WriteLine($"File {filePath} not found.");
                    ++failCount;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Export of {filePath} failed: {e.Message}");
                ++failCount;
            }
        }

        var orchestrion = _realm.GameData.GetSheet("Orchestrion");
        var orchestrionPath = _realm.GameData.GetSheet("OrchestrionPath");
        foreach (IXivRow orchestrionInfo in orchestrion)
        {
            var path = orchestrionPath[orchestrionInfo.Key];
            var name = orchestrionInfo["Name"].ToString();
            var filePath = path["File"].ToString();

            if (string.IsNullOrWhiteSpace(filePath) || !IsMatch(searchStrings, filePath))
                continue;

            try
            {
                if (ExportFile(filePath, name))
                {
                    ++successCount;
                }
                else
                {
                    Console.Error.WriteLine($"File {filePath} not found.");
                    ++failCount;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Export of {filePath} failed: {e.Message}");
                ++failCount;
            }
        }

        Console.WriteLine($"{successCount} files exported, {failCount} failed");
    }

    private bool ExportFile(string filePath, string suffix)
    {
        if (!_realm.Packs.TryGetFile(filePath, out var file))
            return false;

        var scdFile = new ScdFile(file);
        var count = 0;
        for (var i = 0; i < scdFile.ScdHeader.EntryCount; ++i)
        {
            var e = scdFile.Entries[i];
            if (e == null)
                continue;

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            if (suffix != null)
                fileNameWithoutExtension += "-" + suffix;
            if (++count > 1)
                fileNameWithoutExtension += "-" + count;

            foreach (var invalidChar in Path.GetInvalidFileNameChars())
                fileNameWithoutExtension = fileNameWithoutExtension.Replace(invalidChar.ToString(), "");

            var targetPath = Path.Combine(_realm.GameVersion, Path.GetDirectoryName(filePath),
                fileNameWithoutExtension);

            switch (e.Header.Codec)
            {
                case ScdCodec.MSADPCM:
                    targetPath += ".wav";
                    break;
                case ScdCodec.OGG:
                    targetPath += ".ogg";
                    break;
                default:
                    throw new NotSupportedException();
            }

            var fInfo = new FileInfo(targetPath);

            if (!fInfo.Directory.Exists)
                fInfo.Directory.Create();
            File.WriteAllBytes(fInfo.FullName, e.GetDecoded());
        }

        return true;
    }

    private bool IsMatch(List<string> searchStrings, string filePath)
    {
        return !searchStrings.Any() || searchStrings.Any(searchString =>
            filePath.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) > 0);
    }
}