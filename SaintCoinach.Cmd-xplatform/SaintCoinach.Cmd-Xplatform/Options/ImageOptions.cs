using CommandLine;

namespace SaintCoinach.Cmd_Xplatform.Options
{
    [Verb("image", HelpText = "Export an image file.")]
    public class ImageOptions : CommandOption
    {
        [Value(0, MetaName = "path", Required = true, HelpText = "The path to the image file.")]
        public string Path { get; set; }
    }
}
