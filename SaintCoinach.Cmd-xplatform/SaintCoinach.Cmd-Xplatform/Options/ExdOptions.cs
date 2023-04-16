using CommandLine;

namespace SaintCoinach.Cmd_Xplatform.Options
{
    [Verb("exd", HelpText = "Export all data (default), or only specific data files, separated by spaces.")]
    public class ExdOptions : CommandOption
    {
        [Option('f', "file", Required = false, HelpText = "The name of a specific data file to export. If not passed in, will export all exd files.")]
        public IEnumerable<string> FilesToExport { get; set; }

        [Option('l', "languages", Required = false, HelpText = "The languages to export data for. ")] //TODO test how this even passes in
        public IEnumerable<string> LanguagesToExport { get; set; }
        
        [Option('r', "raw", Default = false,
            HelpText = "Export the exd as a 'raw' string. This retrieves the value of each cell as a string, without any formatting or type conversion")]
        public bool Raw { get; set; }

    }
}
