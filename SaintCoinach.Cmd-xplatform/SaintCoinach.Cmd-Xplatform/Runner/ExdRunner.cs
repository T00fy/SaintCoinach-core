using SaintCoinach.Cmd_Xplatform.Options;
using SaintCoinach.Ex;

namespace SaintCoinach.Cmd_Xplatform;

public class ExdRunner
{
    private readonly ARealmReversed _realm;

    public ExdRunner(ARealmReversed realm)
    {
        _realm = realm;
    }

    public void InvokeRunner(ExdOptions options)
    {
        const string csvFileFormat = "exd-all/{0}{1}.csv";

        IEnumerable<string> filesToExport;
        if (options.FilesToExport == null || !options.FilesToExport.Any())
        {
            //export all files
            filesToExport = _realm.GameData.AvailableSheets;
        }
        else
        {
            filesToExport = options.FilesToExport.Select(_ => _realm.GameData.FixName(_));
        }

        var successCount = 0;
        var failCount = 0;

        foreach (var name in filesToExport)
        {
            try
            {
                var sheet = _realm.GameData.GetSheet(name);
                IEnumerable<Language> languagesToExport;
                if (options.LanguagesToExport == null || !options.LanguagesToExport.Any())
                {
                    //export all languages
                    languagesToExport = sheet.Header.AvailableLanguages;
                }
                else
                {
                    languagesToExport = ExdHelper.ConvertLanguages(options.LanguagesToExport);
                }

                foreach (var lang in languagesToExport)
                {
                    var code = lang.GetCode();
                    if (code.Length > 0)
                        code = "." + code;

                    var target =
                        new FileInfo(Path.Combine(_realm.GameVersion, string.Format(csvFileFormat, name, code)));

                    try
                    {
                        if (!target.Directory.Exists)
                            target.Directory.Create();

                        ExdHelper.SaveAsCsv(sheet, lang, target.FullName, options.Raw);

                        ++successCount;
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine($"Export of {name} failed: {e.Message}");
                        try
                        {
                            if (target.Exists)
                            {
                                target.Delete();
                            }
                        }
                        catch
                        {
                            //ignored
                        }

                        ++failCount;
                    }
                }
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine(e.Message);
                break;
            }
        }

        Console.WriteLine($"{successCount} files exported, {failCount} failed");
    }
}