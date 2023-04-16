using CommandLine;

namespace SaintCoinach.Cmd_Xplatform.Options
{
    [Verb("raw", HelpText = "Save contents of a file as raw bytes")]
    public class RawOptions : CommandOption
    {
        
        [Value(0, MetaName = "file", Required = true, HelpText = "The path to the file to export")]
        public string Path { get; set; }
        // TODO: original code does not have any parameters
    }
}
