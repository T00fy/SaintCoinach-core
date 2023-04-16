using System.Globalization;
using System.Text;
using SaintCoinach.Ex;
using SaintCoinach.Ex.Relational;
using SaintCoinach.Ex.Variant2;
using SaintCoinach.Xiv;

namespace SaintCoinach.Cmd {
    static class ExdHelper {
        private static readonly CultureInfo Culture = new("en-US", false);

        public static void SaveAsCsv(IRelationalSheet sheet, Language language, string path, bool writeRaw) {
            using (var s = new StreamWriter(path, false, Encoding.UTF8)) {
                var indexLine = new StringBuilder("key");
                var nameLine = new StringBuilder("#");
                var typeLine = new StringBuilder("int32");

                var colIndices = new List<int>();
                foreach (var col in sheet.Header.Columns) {
                    indexLine.Append($",{col.Index}");
                    nameLine.Append($",{col.Name}");
                    typeLine.Append($",{col.ValueType}");

                    colIndices.Add(col.Index);
                }

                s.WriteLine(indexLine);
                s.WriteLine(nameLine);
                s.WriteLine(typeLine);

                WriteRows(s, sheet, language, colIndices, writeRaw);
            }
        }

        private static void WriteRows(StreamWriter s, ISheet sheet, Language language, IEnumerable<int> colIndices, bool writeRaw) {
            if (sheet.Header.Variant == 1)
                WriteRowsCore(s, sheet.Cast<IRow>(), language, colIndices, writeRaw, WriteRowKey);
            else {
                var rows = sheet.Cast<XivRow>().Select(_ => (DataRow)_.SourceRow);
                foreach (var parentRow in rows.OrderBy(_ => _.Key))
                    WriteRowsCore(s, parentRow.SubRows, language, colIndices, writeRaw, WriteSubRowKey);
            }
        }

        private static void WriteRowsCore(StreamWriter s, IEnumerable<IRow> rows, Language language, IEnumerable<int> colIndices, bool writeRaw, Action<StreamWriter, IRow> writeKey) {
            foreach (var row in rows.OrderBy(_ => _.Key)) {
                var useRow = row;

                if (useRow is IXivRow)
                    useRow = ((IXivRow)row).SourceRow;
                var multiRow = useRow as IMultiRow;

                writeKey(s, useRow);
                foreach (var col in colIndices) {
                    object v;

                    if (language == Language.None || multiRow == null)
                        v = writeRaw ? useRow.GetRaw(col) : useRow[col];
                    else
                        v = writeRaw ? multiRow.GetRaw(col, language) : multiRow[col, language];

                    s.Write(",");
                    if (v == null)
                        continue;
                    else if (IsUnescaped(v))
                        s.Write(string.Format(Culture, "{0}", v));
                    else
                        s.Write("\"{0}\"", v.ToString().Replace("\"", "\"\""));
                }
                s.WriteLine();

                s.Flush();
            }
        }

        private static void WriteRowKey(StreamWriter s, IRow row) {
            s.Write(row.Key);
        }

        private static void WriteSubRowKey(StreamWriter s, IRow row) {
            var subRow = (SubRow)row;
            s.Write(subRow.FullKey);
        }

        private static bool IsUnescaped(object self) {
            return (self is Boolean
                || self is Byte
                || self is SByte
                || self is Int16
                || self is Int32
                || self is Int64
                || self is UInt16
                || self is UInt32
                || self is UInt64
                || self is Single
                || self is Double);
        }
    }
}
