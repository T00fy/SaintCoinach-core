using SaintCoinach.Ex;
using Tharga.Toolkit.Console.Commands.Base;

#pragma warning disable CS1998

namespace SaintCoinach.Cmd.Commands {
    public class LanguageCommand : AsyncActionCommandBase {
        private ARealmReversed _Realm;

        public LanguageCommand(ARealmReversed realm)
            : base("lang", "Change the language.") {
            _Realm = realm;
        }

        public override Task InvokeAsync(string[] param) {
            string paramList = string.Join(" ", param);
            if (string.IsNullOrWhiteSpace(paramList)) {
                OutputInformation($"Current language: {_Realm.GameData.ActiveLanguage}");
                return Task.CompletedTask;
            }
            paramList = paramList.Trim();
            if (!Enum.TryParse<Language>(paramList, out var newLang)) {
                newLang = LanguageExtensions.GetFromCode(paramList);
                if (newLang == Language.Unsupported) {
                    OutputError("Unknown language.");
                    return Task.FromException(new Exception("Unknown language."));
                }
            }
            _Realm.GameData.ActiveLanguage = newLang;
            return Task.CompletedTask;
        }
    }
}
