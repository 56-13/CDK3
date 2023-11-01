using System;
using System.Linq;

namespace CDK.Assets
{
    public static class FileFilters
    {
        public const string Binary = "Binary|*.*";
        public const string Media = "Media|*.wav;*.ogg;*.mp3;*.mp4;*.wmv;*.avi";
        public const string String = "String|*.txt";
        public const string Image = "Image|*.png;*.bmp;*.jpg";
        public const string Mesh = "Mesh|*.fbx;*.3ds;*.dae;*.obj;*.ase";
        public const string Archive = "Archive|*.zip";
        public const string Xml = "XML|*.xml";
        public const string Csv = "CSV|*.csv";
        public const string Excel = "Excel|*.xlsx";
        public const string ExcelOrCsv = "Excel|*.xlsx|CSV|*.csv";

        public static readonly string[] ArchiveExtensions = { "zip" };
        public static readonly string[] ImageExtensions = { "png", "bmp", "jpg" };
        public static readonly string[] MeshExtensions = { "fbx", "3ds", "dae", "obj", "ase" };
        public static readonly string[] MediaExtensions = { "wav", "ogg", "mp3", "mp4", "wmv", "avi" };

        public static bool Contains(string path, string[] extensions) => extensions.Any(e => path.EndsWith(e, StringComparison.CurrentCultureIgnoreCase));
    }
}
