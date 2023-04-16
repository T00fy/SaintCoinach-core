using CommandLine;

namespace SaintCoinach.Cmd_Xplatform.Options;

[Verb("furniture", HelpText = "Export all MDL Furniture/Yard files.")]
public class FurnitureOptions : CommandOption
{
    //TODO should probably include something.. original code does not really use any options and just exports everything
}