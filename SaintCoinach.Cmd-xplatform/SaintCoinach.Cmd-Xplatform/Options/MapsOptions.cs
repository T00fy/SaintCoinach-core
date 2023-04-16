using CommandLine;

namespace SaintCoinach.Cmd_Xplatform.Options
{
    [Verb("maps", HelpText = "Export all map images.")]
    public class MapsOptions : CommandOption
    {
        [Option('f', "format", Default = "png", Required = false, HelpText = "The image format for the exported map (jpg, png).")]
        public string Format { get; set; }
    }
}
