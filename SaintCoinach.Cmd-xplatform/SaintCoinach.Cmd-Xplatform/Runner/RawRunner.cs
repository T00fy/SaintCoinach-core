using SaintCoinach.Cmd_Xplatform.Options;

namespace SaintCoinach.Cmd_Xplatform.Runner;

public class RawRunner
{
    private readonly ARealmReversed _realm;

    public RawRunner(ARealmReversed realm)
    {
        _realm = realm;
    }

    public void InvokeRunner(RawOptions options)
    {
        try {
            if (_realm.Packs.TryGetFile(options.Path.Trim(), out var file)) {
                var target = new FileInfo(Path.Combine(_realm.GameVersion, file.Path));
                if (!target.Directory.Exists)
                    target.Directory.Create();

                var data = file.GetData();
                File.WriteAllBytes(target.FullName, data);
            }
            else
            {
                Console.WriteLine($"File not found: {options.Path.Trim()}.");
            }
        } catch (Exception e) {
            Console.Error.WriteLine(e.Message);
        }
    }
}