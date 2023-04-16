using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SaintCoinach.Cmd_Xplatform.Options;
using SaintCoinach.Xiv;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

namespace SaintCoinach.Cmd_Xplatform.Runner
{
    public class MapRunner
    {
        private readonly ARealmReversed _realm;

        public MapRunner(ARealmReversed realm)
        {
            _realm = realm;
        }

        public void InvokeRunner(MapsOptions options)
        {
            // string format = PngFormat.Instance.FileExtensions.First();
            IImageFormat format = PngFormat.Instance;
            if (!string.IsNullOrEmpty(options.Format))
            {
                switch (options.Format)
                {
                    case "jpg":
                        format = JpegFormat.Instance;
                        break;
                    case "png":
                        break;
                    default:
                        Console.Error.WriteLine($"Invalid map format {options.Format}");
                        return;
                }
            }

            var c = 0;
            var allMaps = _realm.GameData.GetSheet<Map>()
                .Where(m => m.PlaceName != null);

            var fileSet = new Dictionary<string, int>();
            foreach (var map in allMaps)
            {
                var img = map.MediumImage;
                if (img == null)
                    continue;

                var outPathSb = new StringBuilder("ui/map/");
                var territoryName = map.TerritoryType?.Name?.ToString();
                if (!string.IsNullOrEmpty(territoryName))
                {
                    if (territoryName.Length < 3)
                    {
                        outPathSb.AppendFormat("{0}/", territoryName);
                    }
                    else
                    {
                        outPathSb.AppendFormat("{0}/", territoryName.Substring(0, 3));
                    }

                    outPathSb.AppendFormat("{0} - ", territoryName);
                }

                outPathSb.AppendFormat("{0}", ToPathSafeString(map.PlaceName.Name.ToString()));
                if (map.LocationPlaceName != null)
                {
                    if (map.LocationPlaceName.Key != 0 && !map.LocationPlaceName.Name.IsEmpty)
                        outPathSb.AppendFormat(" - {0}", ToPathSafeString(map.LocationPlaceName.Name.ToString()));
                }
                var mapKey = outPathSb.ToString();
                fileSet.TryGetValue(mapKey, out int mapIndex);
                if (mapIndex > 0)
                {
                    outPathSb.AppendFormat(" - {0}", mapIndex);
                }

                fileSet[mapKey] = mapIndex + 1;
                outPathSb.Append(FormatToExtension(format));

                var outFile = new FileInfo(Path.Combine(_realm.GameVersion, outPathSb.ToString()));
                if (!outFile.Directory.Exists)
                    outFile.Directory.Create();

                using var output = new FileStream(outFile.FullName, FileMode.Create);
                img.Save(output, format);
                ++c;
            }

            Console.Out.WriteLine($"{c} maps saved");
        }

        static string FormatToExtension(IImageFormat format)
        {
            if (Equals(format, PngFormat.Instance))
                return ".png";
            if (Equals(format, JpegFormat.Instance))
                return ".jpg";

            throw new NotImplementedException();
        }

        static string ToPathSafeString(string input, char invalidReplacement = '_')
        {
            var sb = new StringBuilder(input);
            var invalid = Path.GetInvalidFileNameChars();
            foreach (var c in invalid)
                sb.Replace(c, invalidReplacement);
            return sb.ToString();
        }
    }
}
