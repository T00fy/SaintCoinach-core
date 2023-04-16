// using SaintCoinach.Imaging;
// using Tharga.Toolkit.Console.Commands.Base;
//
// #pragma warning disable CS1998
//
// namespace SaintCoinach.Cmd.Commands {
//     public class ImageCommand : AsyncActionCommandBase {
//         private ARealmReversed _Realm;
//
//         public ImageCommand(ARealmReversed realm)
//             : base("image", "Export an image file.") {
//             _Realm = realm;
//         }
//
//         public override Task InvokeAsync(string[] param) {
//             string paramList = string.Join(" ", param);
//             try {
//                 if (_Realm.Packs.TryGetFile(paramList.Trim(), out var file)) {
//                     if (file is ImageFile imgFile) {
//                         var img = imgFile.GetImage();
//
//                         var target = new FileInfo(Path.Combine(_Realm.GameVersion, file.Path));
//                         if (!target.Directory.Exists)
//                             target.Directory.Create();
//                         var pngPath = target.FullName.Substring(0, target.FullName.Length - target.Extension.Length) + ".png";
//                         img.Save(pngPath);
//                     } else
//                         OutputError(new Exception($"File is not an image (actual: {file.CommonHeader.FileType})."));
//                 } else
//                     OutputError("File not found.");
//             } catch (Exception e) {
//                 OutputError(e.Message);
//             }
//
//             return Task.CompletedTask;
//         }
//     }
// }
