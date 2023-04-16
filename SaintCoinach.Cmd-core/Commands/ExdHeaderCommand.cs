using Newtonsoft.Json;
using Tharga.Toolkit.Console.Commands.Base;

namespace SaintCoinach.Cmd.Commands {
    public class ExdHeaderCommand : AsyncActionCommandBase {
        private ARealmReversed _Realm;

        public ExdHeaderCommand(ARealmReversed realm)
            :base("exdheader", "Export all data headers.") {
            _Realm = realm;
        }

        #pragma warning disable CS1998
        public override Task InvokeAsync(string[] param) {
            string paramList = string.Join(" ", param);
            var headers = new List<SheetHeader>();
            foreach (var name in _Realm.GameData.AvailableSheets) {
                var header = new SheetHeader() { Name = name, Columns = new List<SheetColumn>() };
                headers.Add(header);

                var sheet = _Realm.GameData.GetSheet(name);
                foreach (var relationalColumn in sheet.Header.Columns) {
                    header.Columns.Add(new SheetColumn() {
                        Name = relationalColumn.Name,
                        Type = relationalColumn.Reader.Type.Name
                    });
                }
            }

            Directory.CreateDirectory(_Realm.GameVersion);

            var settings = new JsonSerializerSettings() {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(headers, settings);
            File.WriteAllText(Path.Combine(_Realm.GameVersion, "exd-header.json"), json);
            return Task.CompletedTask;
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
}
