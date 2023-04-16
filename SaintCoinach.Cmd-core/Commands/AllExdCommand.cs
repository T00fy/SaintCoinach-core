using SaintCoinach.Ex;
using Tharga.Toolkit.Console.Commands.Base;

namespace SaintCoinach.Cmd.Commands {
    public class AllExdCommand : AsyncActionCommandBase {
        private ARealmReversed _Realm;

        public AllExdCommand(ARealmReversed realm)
            : base("allexd", "Export all data (default), or only specific data files, seperated by spaces; including all languages.") {
            _Realm = realm;
        }

        public override Task InvokeAsync(string[] param) {
            string paramList = string.Join(" ", param);
            const string csvFileFormat = "exd-all/{0}{1}.csv";

            IEnumerable<string> filesToExport;

            if (string.IsNullOrWhiteSpace(paramList))
                filesToExport = _Realm.GameData.AvailableSheets;
            else
                filesToExport = paramList.Split(' ').Select(_ => _Realm.GameData.FixName(_));

            var successCount = 0;
            var failCount = 0;
            foreach (var name in filesToExport) {
                var sheet = _Realm.GameData.GetSheet(name);
                foreach(var lang in sheet.Header.AvailableLanguages) {
                    var code = lang.GetCode();
                    if (code.Length > 0)
                        code = "." + code;
                    var target = new FileInfo(Path.Combine(_Realm.GameVersion, string.Format(csvFileFormat, name, code)));
                    try {

                        if (!target.Directory.Exists)
                            target.Directory.Create();

                        ExdHelper.SaveAsCsv(sheet, lang, target.FullName, false);

                        ++successCount;
                    } catch (Exception e) {
                        OutputError(new Exception($"Export of {name} failed: {e.Message}"));
                        try { if (target.Exists) { target.Delete(); } } catch { }
                        ++failCount;
                    }
                }
            }
            OutputInformation($"{successCount} files exported, {failCount} failed");
            return Task.CompletedTask;
        }
    }
}
