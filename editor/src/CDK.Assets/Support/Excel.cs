using System;
using System.Linq;
using System.IO;

using ClosedXML.Excel;

namespace CDK.Assets.Support
{
    public class Excel : IDisposable
    {
        private XLWorkbook _workbook;
        private IXLWorksheet _worksheet;
        private string _path;
        private bool _isNew;

        public Excel(string path)
        {
            _path = path;

            if (File.Exists(path))
            {
                _workbook = new XLWorkbook(path);
            }
            else
            {
                _workbook = new XLWorkbook();

                _isNew = true;
            }
        }

        public string[] GetSheets()
        {
            return _workbook.Worksheets.Select(ws => ws.Name).ToArray();
        }

        public bool OpenSheet(string name, bool isNew)
        {
            foreach (var ws in _workbook.Worksheets)
            {
                if (ws.Name == name)
                {
                    _worksheet = ws;

                    return true;
                }
            }
            if (isNew)
            {
                _worksheet = _workbook.Worksheets.Add(name);

                return true;
            }
            return false;
        }

        public bool RenameSheet(string name)
        {
            if (_worksheet == null) return false;

            foreach (var ws in _workbook.Worksheets)
            {
                if (ws.Name == name)
                {
                    _worksheet = ws;

                    return false;
                }
            }
            _worksheet.Name = name;

            return true;
        }

        public bool RemoveSheet()
        {
            if (_worksheet == null) return false;

            _worksheet.Delete();

            _worksheet = null;

            return true;
        }

        public string[][] GetCells()
        {
            if (_worksheet == null) return new string[0][];

            var column = 0;

            for (; ; )
            {
                var cell = _worksheet.Cell(1, column + 1);
                var value = cell.Value.ToString();
                if (value == string.Empty) column++;
                else break;
            }
            if (column == 0) return new string[0][];
            var row = 1;
            for (; ; )
            {
                var flag = false;
                for (var c = 0; c < column; c++)
                {
                    var cell = _worksheet.Cell(row + 1, c + 1);
                    var value = cell.Value.ToString();
                    if (value == string.Empty)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag) row++;
                else break;
            }

            var values = new string[row][];

            for (var r = 0; r < row; r++)
            {
                values[r] = new string[column];
                for (var c = 0; c < column; c++)
                {
                    var cell = _worksheet.Cell(r + 1, c + 1);
                    var value = cell.Value.ToString();
                    values[r][c] = value;
                }
            }
            return values;
        }

        public string GetCell(int row, int column)
        {
            if (_worksheet == null) return string.Empty;

            var cell = _worksheet.Cell(row + 1, column + 1);
            var value = cell.Value.ToString();
            return value;
        }

        public bool SetCell(int row, int column, string value)
        {
            if (_worksheet == null) return false;
            _worksheet.Cell(row + 1, column + 1).Value = value;
            return true;
        }

        public void Save()
        {
            if (_isNew) _workbook.SaveAs(_path);
            else _workbook.Save();
        }

        public void Dispose() => _workbook.Dispose();
    }
}
