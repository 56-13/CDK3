using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace CDK.Assets.Support
{
    public class CSV
    {
        public static string[][] ParseAll(string csv, int column = int.MaxValue)
        {
            var rtn = new List<string[]>();

            var current = new List<string>();

            var strbuf = new StringBuilder();

            var quaotion = false;

            for (var i = 0; i < csv.Length; i++)
            {
                switch (csv[i])
                {
                    case ',':
                        if (quaotion || current.Count >= column) strbuf.Append(',');
                        else
                        {
                            current.Add(strbuf.ToString());
                            strbuf.Clear();
                        }
                        break;
                    case '"':
                        if (!quaotion)
                        {
                            if (current.Count >= column) strbuf.Append('"');
                            quaotion = true;
                        }
                        else if(i + 1 < csv.Length && csv[i + 1] == '"')
                        {
                            strbuf.Append('"');
                            if (current.Count >= column) strbuf.Append('"');
                            i++;
                        }
                        else
                        {
                            if (current.Count >= column) strbuf.Append('"');
                            quaotion = false;
                        }
                        break;
                    case '\r':
                        if (quaotion) strbuf.Append('\r');
                        break;
                    case '\n':
                        if (!quaotion)
                        {
                            if (strbuf.Length != 0) current.Add(strbuf.ToString());
                            rtn.Add(current.ToArray());
                            current.Clear();
                            strbuf.Clear();
                        }
                        else strbuf.Append('\n');
                        break;
                    default:
                        strbuf.Append(csv[i]);
                        break;
                }
            }
            if (strbuf.Length != 0) current.Add(strbuf.ToString());
            if (current.Count != 0) rtn.Add(current.ToArray());

            return rtn.ToArray();
        }

        public static string[] Parse(string csv, int column = int.MaxValue)
        {
            var current = new List<string>();

            var strbuf = new StringBuilder();

            var quaotion = false;

            for (var i = 0; i < csv.Length; i++)
            {
                switch (csv[i])
                {
                    case ',':
                        if (quaotion || current.Count >= column) strbuf.Append(',');
                        else
                        {
                            current.Add(strbuf.ToString());
                            strbuf.Clear();
                        }
                        break;
                    case '"':
                        if (!quaotion)
                        {
                            if (current.Count >= column) strbuf.Append('"');
                            quaotion = true;
                        }
                        else if (i + 1 < csv.Length && csv[i + 1] == '"')
                        {
                            strbuf.Append('"');
                            if (current.Count >= column) strbuf.Append('"');
                            i++;
                        }
                        else
                        {
                            if (current.Count >= column) strbuf.Append('"');
                            quaotion = false;
                        }
                        break;
                    case '\r':
                        if (quaotion) strbuf.Append('\r');
                        break;
                    case '\n':
                        if (!quaotion) goto rtn;
                        else strbuf.Append('\n');
                        break;
                    default:
                        strbuf.Append(csv[i]);
                        break;
                }
            }

        rtn:

            if (strbuf.Length != 0 || current.Count == 0) current.Add(strbuf.ToString());

            return current.ToArray();
        }

        public static string Make(string[] origin)
        {
            var strbuf = new StringBuilder();
            var comma = false;
            foreach (var str in origin)
            {
                if (comma) strbuf.Append(',');
                else comma = true;

                if (str.Contains('\n') || str.Contains(',') || str.Contains('"'))
                {
                    strbuf.Append('"');
                    strbuf.Append(str.Replace("\"", "\"\""));
                    strbuf.Append('"');
                }
                else strbuf.Append(str.Replace("\"", "\"\""));
            }
            return strbuf.ToString();
        }
    }
}
