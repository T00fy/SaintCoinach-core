using CommandLine;

namespace SaintCoinach.Cmd_Xplatform.Options
{
    [Verb("headers", HelpText = "Export all data headers as JSON")]
    public class ExdHeaderOptions : CommandOption
    {
        //TODO should probably include something.. original code does not really use any options and just exports everything
    }
}
