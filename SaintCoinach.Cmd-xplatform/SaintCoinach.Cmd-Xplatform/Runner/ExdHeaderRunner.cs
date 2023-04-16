using Newtonsoft.Json;
using SaintCoinach.Cmd_Xplatform.Options;
using static System.IO.Directory;

namespace SaintCoinach.Cmd_Xplatform.Runner;

public class ExdHeaderRunner
{
    private readonly ARealmReversed _realm;

    public ExdHeaderRunner(ARealmReversed realm)
    {
        _realm = realm;
    }

    public void InvokeRunner(ExdHeaderOptions options)
    {
        var headers = new List<SheetHeader>();
        foreach (var name in _realm.GameData.AvailableSheets) {
            var header = new SheetHeader() { Name = name, Columns = new List<SheetColumn>() };
            headers.Add(header);

            var sheet = _realm.GameData.GetSheet(name);
            foreach (var relationalColumn in sheet.Header.Columns) {
                header.Columns.Add(new SheetColumn() {
                    Name = relationalColumn.Name,
                    Type = relationalColumn.Reader.Type.Name
                });
            }
        }

        CreateDirectory(_realm.GameVersion);

        var settings = new JsonSerializerSettings() {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };

        var json = JsonConvert.SerializeObject(headers, settings);
        var path = Path.Combine(_realm.GameVersion, "exd-header.json");
        File.WriteAllText(path, json);
        Console.WriteLine($"Finished exporting to {path}");
    }
    
    private class SheetHeader {
        public string Name;
        public List<SheetColumn> Columns;
    }

    private class SheetColumn {
        public string Name;
        public string Type;
    }
}