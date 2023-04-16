using CommandLine;

namespace SaintCoinach.Cmd_Xplatform.Options
{
    [Verb("bgm", HelpText = "Export all BGM files (default), or only those matching specific strings, separated by spaces (e.g. bgm_ride bgm_orch).")]
    public class BgmOptions : CommandOption
    {
        // Define the options for the "BgmOptions" command
        // ...
        [Option('f', "file", Required = false, HelpText = "The name of a specific data file to export. If not passed in, will export all files.")]
        public IEnumerable<string> FilesToExport { get; set; }
    }
}
