using Tharga.Toolkit.Console.Commands.Base;

#pragma warning disable CS1998

namespace SaintCoinach.Cmd.Commands {
    public class RawCommand : AsyncActionCommandBase {
        private ARealmReversed _Realm;

        public RawCommand(ARealmReversed realm)
            : base("raw", "Save raw contents of a file.") {
            _Realm = realm;
        }

        public override Task InvokeAsync(string[] param) {
            if (param == null)
                return Task.FromException(new Exception("param was null"));
            try {
                string paramList = string.Join(" ", param);
                if (_Realm.Packs.TryGetFile(paramList.Trim(), out var file)) {
                    var target = new FileInfo(Path.Combine(_Realm.GameVersion, file.Path));
                    if (!target.Directory.Exists)
                        target.Directory.Create();

                    var data = file.GetData();
                    File.WriteAllBytes(target.FullName, data);
                } else
                    OutputError("File not found.");
            } catch (Exception e) {
                OutputError(e.Message);
            }

            return Task.CompletedTask;
        }
    }
}
