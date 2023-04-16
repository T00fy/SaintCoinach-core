using SaintCoinach.Cmd_Xplatform.Options;
using SaintCoinach.Xiv;

namespace SaintCoinach.Cmd_Xplatform.Runner;

public class FurnitureRunner
{
    private readonly ARealmReversed _realm;

    public FurnitureRunner(ARealmReversed realm)
    {
        _realm = realm;
    }

    public void InvokeRunner(FurnitureOptions options) //TODO can't test, don't know how to view these files in *nix
    {
        var indoor = _realm.GameData.GetSheet("HousingFurniture");
        var outdoor = _realm.GameData.GetSheet("HousingYardObject");
        var allFurniture = indoor.Cast<HousingItem>().Concat(outdoor.Cast<HousingItem>())
            .Where(_ => _.Item != null && _.Item.Key != 0 && _.Item.Name.ToString().Length > 0)
            .OrderBy(_ => _.Item.Name).ToArray();

        var successCount = 0;
        var failCount = 0;
        foreach (HousingItem outdoorItem in allFurniture)
        {
            var filePath = outdoorItem.GetScene().File.ToString();

            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    continue;

                if (ExportFile(filePath))
                {
                    ++successCount;
                }
                else
                {
                    Console.Error.WriteLine(new Exception($"File {filePath} not found."));
                    ++failCount;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(new Exception($"Export of {filePath} failed: {e.Message}"));
                ++failCount;
            }
        }

        Console.WriteLine($"{successCount} files exported, {failCount} failed");
    }

    /// <summary>
    /// Exports files to mdl and text using the function that exports any asset object type.
    /// </summary>
    /// <param name="filePath">File path to where the item's sgb file is located</param>
    /// <return name="result">A boolean indicating if the file was saved correctly.</return>
    private bool ExportFile(string filePath)
    {
        try
        {
            //Running for mdl
            bool successful = WriteFunction(filePath.Replace("asset", "bgparts").Replace("sgb", "mdl"));

            //Running for all three textures
            WriteFunction(filePath.Replace("asset", "texture").Replace(".sgb", "_1a_d.tex"));
            WriteFunction(filePath.Replace("asset", "texture").Replace(".sgb", "_1a_n.tex"));
            WriteFunction(filePath.Replace("asset", "texture").Replace(".sgb", "_1a_s.tex"));

            return successful;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return false;
        }
    }

    private bool WriteFunction(string filePath)
    {
        if (!_realm.Packs.TryGetFile(filePath, out var file)) return false;
        try
        {
            var target = new FileInfo(Path.Combine(_realm.GameVersion, file.Path));
            if (!target.Directory.Exists)
                target.Directory.Create();

            var data = file.GetData();
            File.WriteAllBytes(target.FullName, data);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return false;
        }

        return true;
    }
}