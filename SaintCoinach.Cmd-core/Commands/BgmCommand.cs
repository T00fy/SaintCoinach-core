using SaintCoinach.Sound;
using SaintCoinach.Xiv;
using Tharga.Toolkit.Console.Commands.Base;

#pragma warning disable CS1998

namespace SaintCoinach.Cmd.Commands {
    public class BgmCommand : AsyncActionCommandBase {
        private ARealmReversed _Realm;

        public BgmCommand(ARealmReversed realm)
            : base("bgm", "Export all BGM files (default), or only those matching specific strings, separated by spaces (e.g. bgm_ride bgm_orch)") {
            _Realm = realm;
        }

        public override Task InvokeAsync(string[] param) {
            string paramList = string.Join(" ", param);
            var bgms = _Realm.GameData.GetSheet("BGM");
            String[] searchStrings;

            if (string.IsNullOrWhiteSpace(paramList))
                searchStrings = Array.Empty<String>();
            else
                searchStrings = paramList.Split(' ');

            var successCount = 0;
            var failCount = 0;
            foreach (IXivRow bgm in bgms) {
                var filePath = bgm["File"].ToString();

                try {
                    if (string.IsNullOrWhiteSpace(filePath) || !IsMatch(searchStrings, filePath))
                        continue;

                    if (ExportFile(filePath, null)) {
                        ++successCount;
                    } else {
                        OutputError(new Exception($"File {filePath} not found."));
                        ++failCount;
                    }
                } catch(Exception e) {
                    OutputError(new Exception($"Export of {filePath} failed: {e.Message}"));
                    ++failCount;
                }
            }

            var orchestrion = _Realm.GameData.GetSheet("Orchestrion");
            var orchestrionPath = _Realm.GameData.GetSheet("OrchestrionPath");
            foreach (IXivRow orchestrionInfo in orchestrion) {
                var path = orchestrionPath[orchestrionInfo.Key];
                var name = orchestrionInfo["Name"].ToString();
                var filePath = path["File"].ToString();

                if (string.IsNullOrWhiteSpace(filePath) || !IsMatch(searchStrings, filePath))
                    continue;

                try {
                    if (ExportFile(filePath, name)) {
                        ++successCount;
                    } else {
                        OutputError(new Exception($"File {filePath} not found."));
                        ++failCount;
                    }
                }
                catch (Exception e) {
                    OutputError(new Exception($"Export of {filePath} failed: {e.Message}"));
                    ++failCount;
                }
            }

            OutputInformation($"{successCount} files exported, {failCount} failed");

            return Task.CompletedTask;
        }

        private bool ExportFile(string filePath, string suffix) {
            if (!_Realm.Packs.TryGetFile(filePath, out var file))
                return false;

            var scdFile = new ScdFile(file);
            var count = 0;
            for (var i = 0; i < scdFile.ScdHeader.EntryCount; ++i) {
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

                var targetPath = Path.Combine(_Realm.GameVersion, Path.GetDirectoryName(filePath), fileNameWithoutExtension);

                switch (e.Header.Codec) {
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

        private bool IsMatch(String[] searchStrings, string filePath) {
            return searchStrings.Length == 0 || searchStrings.Any(searchString => filePath.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) > 0);
        }

        public override void Invoke(string[] param)
        {
            throw new NotImplementedException();
        }
    }
}