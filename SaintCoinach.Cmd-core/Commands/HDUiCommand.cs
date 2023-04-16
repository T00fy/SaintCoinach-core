// using SaintCoinach.Imaging;
// using Tharga.Toolkit.Console.Commands.Base;
//
// #pragma warning disable CS1998
//
// namespace SaintCoinach.Cmd.Commands
// {
//     public class HDUiCommand : AsyncActionCommandBase
//     {
//         const string UiImagePathFormat = "ui/icon/{0:D3}000{1}/{2:D6}_hr1.tex";
//
//         static readonly string[] UiVersions = new string[]
//         {
//             "",
//             "/en",
//             "/ja",
//             "/fr",
//             "/de",
//             "/hq",
//             "/chs"
//         };
//
//         private ARealmReversed _Realm;
//
//         public HDUiCommand(ARealmReversed realm)
//             : base("uiHD", "Export all, a single, or a range of HD UI icons.")
//         {
//             _Realm = realm;
//         }
//
//         public override Task InvokeAsync(string[] param)
//         {
//             string paramList = string.Join(" ", param);
//             var min = 0;
//             var max = 999999;
//
//             if (!string.IsNullOrWhiteSpace(paramList))
//             {
//                 var splitParam = paramList.Split(' ');
//
//                 if (splitParam.Length == 1)
//                 {
//                     if (int.TryParse(splitParam[0], out var parsed))
//                         min = max = parsed;
//                     else
//                     {
//                         OutputError("Failed to parse parameters.");
//                         return Task.FromException(new Exception("Failed to parse parameters."));
//                     }
//                 }
//                 else if (splitParam.Length == 2)
//                 {
//                     if (!int.TryParse(splitParam[0], out min) || !int.TryParse(splitParam[1], out max))
//                     {
//                         OutputError("Failed to parse parameters.");
//                         return Task.FromException(new Exception("Failed to parse parameters."));
//                     }
//
//                     if (max < min)
//                     {
//                         OutputError("Invalid parameters.");
//                         return Task.FromException(new Exception("Invalid parameters."));
//                     }
//                 }
//                 else
//                 {
//                     OutputError("Failed to parse parameters.");
//                     return Task.FromException(new Exception("Failed to parse parameters."));
//                 }
//             }
//
//             var count = 0;
//             for (int i = min; i <= max; ++i)
//             {
//                 try
//                 {
//                     count += Process(i);
//                 }
//                 catch (Exception e)
//                 {
//                     OutputError(new Exception($"{i:D6}: {e.Message}"));
//                 }
//             }
//
//             OutputInformation($"{count} images processed");
//
//             return Task.CompletedTask;
//         }
//
//         private int Process(int i)
//         {
//             var count = 0;
//             foreach (var v in UiVersions)
//             {
//                 if (Process(i, v))
//                     ++count;
//             }
//
//             return count;
//         }
//
//         private bool Process(int i, string version)
//         {
//             var filePath = string.Format(UiImagePathFormat, i / 1000, version, i);
//
//             if (_Realm.Packs.TryGetFile(filePath, out var file))
//             {
//                 if (file is ImageFile imgFile)
//                 {
//                     var img = imgFile.GetImage();
//
//                     var target = new FileInfo(Path.Combine(_Realm.GameVersion, file.Path));
//                     if (!target.Directory.Exists)
//                         target.Directory.Create();
//                     var pngPath = target.FullName.Substring(0, target.FullName.Length - target.Extension.Length) +
//                                   ".png";
//                     img.Save(pngPath);
//
//                     return true;
//                 }
//                 else
//                 {
//                     OutputError($"{filePath} is not an image.");
//                 }
//             }
//
//             return false;
//         }
//     }
// }