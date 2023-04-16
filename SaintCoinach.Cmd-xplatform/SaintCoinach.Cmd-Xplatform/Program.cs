using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using CommandLine;
using Microsoft.Extensions.Configuration;
using SaintCoinach.Cmd_Xplatform.Options;
using SaintCoinach.Cmd_Xplatform.Runner;
using SaintCoinach.Ex;
using SaintCoinach.IO;
using Directory = System.IO.Directory;

namespace SaintCoinach.Cmd_Xplatform
{
    class Program
    {
        private static IConfigurationRoot Configuration { get; set; } = null!;
        private static string DataPath { get; set; } = null!;

        private static void InitConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            Configuration = configuration;
        }

        [SuppressMessage("ReSharper.DPA", "DPA0000: DPA issues")]
        static void Main(string[] args)
        {
            InitConfiguration();

            DataPath = InitDataPath() ?? throw new InvalidOperationException(
                "Need data! The DataPath doesn't exist. Either override appsettings.json OR use ENV variable FFXIV_GAME_DIR");
            Console.WriteLine($"Found game path {DataPath}");

            Parser.Default.ParseArguments<ExdOptions, BgmOptions, ImageOptions, MapsOptions, ExdHeaderOptions, UiOptions, FurnitureOptions, SqlOptions, RawOptions>(args)
                .WithParsed((ExdOptions opts) => new ExdRunner(GetRealm(opts.Verbose)).InvokeRunner(opts))
                .WithParsed((BgmOptions opts) => new BgmRunner(GetRealm(opts.Verbose)).InvokeRunner(opts))
                .WithParsed((ImageOptions opts) => new ImageRunner(GetRealm(opts.Verbose)).InvokeRunner(opts))
                .WithParsed((MapsOptions opts) => new MapRunner(GetRealm(opts.Verbose)).InvokeRunner(opts))
                .WithParsed((ExdHeaderOptions opts) => new ExdHeaderRunner(GetRealm(opts.Verbose)).InvokeRunner(opts))
                .WithParsed((UiOptions opts) => new UiRunner(GetRealm(opts.Verbose)).InvokeRunner(opts))
                .WithParsed((FurnitureOptions opts) => new FurnitureRunner(GetRealm(opts.Verbose)).InvokeRunner(opts))
                .WithParsed((SqlOptions opts) => new SqlRunner(GetRealm(opts.Verbose)).InvokeRunner(opts))
                .WithParsed((RawOptions opts) => new RawRunner(GetRealm(opts.Verbose)).InvokeRunner(opts));
        }

        private static ARealmReversed GetRealm(bool verbose)
        {
            if (verbose)
                return new ARealmReversed(DataPath, @"SaintCoinach.History.zip", Language.English, @"app_data.sqlite");
            var stdOut = Console.Out;
            Console.SetOut(TextWriter.Null);
            var realm = new ARealmReversed(DataPath, @"SaintCoinach.History.zip", Language.English, @"app_data.sqlite");
            Console.SetOut(stdOut);
            return realm;
        }

        private static string InitDataPath()
        {
            var dataPath = Environment.GetEnvironmentVariable("FFXIV_GAME_DIR") ??
                           Configuration.GetSection("AppSettings:DataPath").Value;

            if (string.IsNullOrWhiteSpace(dataPath))
                dataPath = SearchForDataPaths().FirstOrDefault(Directory.Exists);

            // if (string.IsNullOrWhiteSpace(dataPath))
            // {
            //     dataPath = CheckArgsForDataPath(ref args, dataPath);
            // }

            return dataPath;
        }

        private static string? CheckArgsForDataPath(ref string[] args, string? dataPath)
        {
            throw new NotImplementedException();
        }

        static string[] SearchForDataPaths()
        {
            const string gameFolder = "FINAL FANTASY XIV - A Realm Reborn";
#if WINDOWS
            string programDir;
            if (Environment.Is64BitProcess)
                programDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            else
                programDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            return new[] {
                Path.Combine(programDir, "SquareEnix", gameFolder),
                Path.Combine(@"D:\Games\SteamApps\common", gameFolder)
            };
#else
            string homeDir = Environment.GetEnvironmentVariable("HOME");
            var searchPaths =
                Directory.GetDirectories(Path.Combine(homeDir, "Games"), "*", SearchOption.TopDirectoryOnly);
            var searchForDataPaths = searchPaths.Select(p => Path.Combine(p, gameFolder)).ToArray();
            Console.WriteLine($"Detected game path");
            return searchForDataPaths;
#endif
        }
    }
}