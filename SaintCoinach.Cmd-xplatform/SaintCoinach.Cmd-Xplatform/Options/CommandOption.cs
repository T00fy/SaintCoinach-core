using CommandLine;

namespace SaintCoinach.Cmd_Xplatform.Options
{
    public class CommandOption
    {
        [Option('v', "verbose", Default = false,
            HelpText = "Show warnings from SaintCoinach core about missing sheets.")]
        public bool Verbose { get; set; }
        
        // [Option("DataPath", HelpText = "Override the path to FFXIV data files.")]
        // public string DataPath { get; set; }
    }   
}