using CommandLine;

namespace SaintCoinach.Cmd_Xplatform.Options
{
    [Verb("ui", HelpText = "Export all, a single, or a range of UI icons. By default if nothing is passed into min/max this will export ALL. If you need a specific icon please set min/max to the same integer")]
    public class UiOptions : CommandOption
    {
        // Define the options for the "UiOptions" command
        // ...
        [Option("hd", Default = false, HelpText = "Export HD files.")]
        public bool Hd { get; set; } //TODO I think this does jack shit
        
        [Option('m', "min", Default = 0, HelpText = "The minimum icon ID to export.")]
        public int Min { get; set; }

        [Option('M', "max", Default = 999999, HelpText = "The maximum icon ID to export.")]
        public int Max { get; set; }

        [Option('l', "languages", Default = new[]{"","en","ja","fr","de","hq","chs"}, HelpText = "The game language(s) to export icons from (comma-separated). Supports en,ja,fr,de,hq,,chs (empty comma is not a typo)")]
        public IEnumerable<string> Versions { get; set; }
    }
}