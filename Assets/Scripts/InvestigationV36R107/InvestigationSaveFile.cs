using System;
using System.IO;
using System.Text;

namespace ConRoi.Investigation.V36.R107
{
    // One writer per profile. Load reads only the committed destination, never .tmp.
    internal static class InvestigationSaveFile
    {
        internal static void Write(string path, string json)
        {
            Write(path, json, (stream, bytes) => stream.Write(bytes, 0, bytes.Length));
        }

        // Private write seam permits a controlled partial-write IOException in Editor checks.
        // Production always uses the overload above; there is no configurable fault hook.
        static void Write(string path, string json, Action<FileStream, byte[]> write)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            string temp = path + ".tmp";
            byte[] bytes = new UTF8Encoding(false).GetBytes(json);
            using (var stream = new FileStream(temp, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                write(stream, bytes);
                stream.Flush(true);
            }
            // Same directory/filesystem. No delete/copy fallback if replacement fails.
            // A failed attempt may leave .tmp; next explicit Save rewrites it from live state.
            if (File.Exists(path)) File.Replace(temp, path, null);
            else File.Move(temp, path);
        }
    }
}
