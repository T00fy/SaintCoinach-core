using SaintCoinach.Cmd_Xplatform.Options;
using SaintCoinach.Imaging;
using SixLabors.ImageSharp;

namespace SaintCoinach.Cmd_Xplatform.Runner;

public class ImageRunner
{
    private readonly ARealmReversed _realm;

    public ImageRunner(ARealmReversed realm)
    {
        _realm = realm;
    }

    public void InvokeRunner(ImageOptions options)
    {
        try
        {
            if (_realm.Packs.TryGetFile(options.Path.Trim(), out var file))
            {
                if (file is ImageFile imgFile)
                {
                    var img = imgFile.GetImage();

                    var target = new FileInfo(Path.Combine(_realm.GameVersion, file.Path));
                    if (!target.Directory.Exists)
                        target.Directory.Create();
                    var pngPath = target.FullName[..^target.Extension.Length] + ".png";
                    img.SaveAsPng(pngPath);
                }
                else
                    Console.Error.WriteLine($"File is not an image (actual: {file.CommonHeader.FileType}).");
            }
            else
                Console.Error.WriteLine("File not found.");
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
        }
    }
}