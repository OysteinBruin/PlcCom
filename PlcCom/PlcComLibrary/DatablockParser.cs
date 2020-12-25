using PlcComLibrary.Common;
using PlcComLibrary.Config;
using PlcComLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace PlcComLibrary
{
    public class DatablockParser : IDatablockParser
    {
        private string[] _separatingStrings = { "//", ";", ":" };
        private readonly IList<string> _endOfHeaderKeywords = new List<string> { "STRUCT", "VAR" };
        private readonly IList<string> _dbSectionKeywords = new List<string> { "STRUCT", "Struct", "VAR", "Var", "TYPE"};
        private List<ISignalModel> _signals = new List<ISignalModel>();
        private string _path;
        private int _dbNumber;
        private List<string> _discardKeywords;
        private int _bitCounter = 0;
        private int _byteCounter = 0;

        public int FirstByte { get; } = 0;
        public int ByteCount { get; } = 0;

        public DatablockParser()
        {
        }
         
        public List<ISignalModel> ParseDb(string path, int dbNumber, List<string> discardKeywords)
        {
            _path = path;
            _dbNumber = dbNumber;
            _discardKeywords = discardKeywords;
            _signals.Clear();
            List<string> fileLines = ReadS7DbFile(_path);

            // Return if no data from db file
            if (fileLines.Count == 0)
            {
                return _signals;
            }

            _bitCounter = 0;
            _byteCounter = 0;

            bool result = RemoveHeaderData(fileLines, _endOfHeaderKeywords);
            Console.WriteLine($"DatablockParser.ParseDb path {_path}");

            // TODO: HANDLE
            if (!result)
            {
                throw new Exception("Invalid file. No STRUCT or VAR keyword found.");
            }

            foreach (var line in fileLines)
            {
                (List<string> splittedLines, bool signalDiscarded) = SplitAndValidateLine(line, _discardKeywords);

                _signals.AddRange(CheckforUDT(splittedLines));

                splittedLines = FilterOnKeywords(splittedLines);

                if (splittedLines.Count > 1)
                {
                    (Enums.DataType dataType, string dataTypeStr, int byteSize) datatypeAndSize = GetDataTypeAndByteSizeFromLine(splittedLines[1]);

                    if (datatypeAndSize.dataType != Enums.DataType.Array)
                    {
                        if (!signalDiscarded)
                        {
                            int signalIndex = _signals.Count;
                            _signals.Add(CreateSignal(signalIndex, splittedLines, datatypeAndSize));
                        }                        
                    }

                    UpdateByteAndBitIndex(datatypeAndSize);

                }
            }

            return _signals;
        }




        /// <summary>
        /// TODOTODOTODOTODOTODOTODOTODO   TODO: Handle File exceptions
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<string> ReadS7DbFile(string path)
        {
            Console.WriteLine($"S7DbParser.ReadS7DbFile: {path}");
            
            List<string> fileLines = new List<string>();
            // TODO: Handle File exceptions instead of:
            FileInfo fi = new FileInfo(path);
            if (!fi.Exists)
            {
                Console.WriteLine($"\n\n\t\t{path} NOT found!\n\n ");
                return fileLines;
            }

            fileLines = File.ReadAllLines(path).ToList();
            return fileLines;
        }

        private bool RemoveHeaderData(List<string> fileLines, IList<string> endOfHeaderKeywords)
        {
            int contentBeignIndex = -1;
            for (int i = 0; i < fileLines.Count; i++)
            {
                if (contentBeignIndex == -1)
                {
                    foreach (var item in endOfHeaderKeywords)
                    {
                        if (fileLines[i].Contains(item))
                        {
                            contentBeignIndex = i;
                        }
                    }
                }

                fileLines[i].Trim();
            }

            if (contentBeignIndex >= 0 && contentBeignIndex < fileLines.Count)
            {
                fileLines.RemoveRange(0, contentBeignIndex);
                return true;
            }

            return false;
        }

        (List<string> splittedLines, bool discarded) SplitAndValidateLine(string line, List<string> discardKeywords)
        {
            List<string> splittedLineStrings = new List<string>();
            splittedLineStrings = StringExtensions.Split_RemoveWhiteTokens(line, _separatingStrings).ToList();

            bool doDiscardSignal = false;
            // Console.WriteLine($"-------------------\nSplitAndValidateLine() ");
            foreach (var discardKeyword in discardKeywords)
            {
                List<string> containsDiscardKeywordList = splittedLineStrings.Where(str => str.Contains(discardKeyword)).ToList();

                if (containsDiscardKeywordList.Count > 0)
                {
                    doDiscardSignal = true;
                }

                //Console.Write($"discardKeyword {discardKeyword}, line to check: ");
                //foreach (var str in splittedLineStrings)
                //{
                //    Console.Write(str);
                //    Console.Write(" - ");
                //}
                //Console.WriteLine($"{discardKeyword} result: {doDiscardSignal}");
            }
            //Console.WriteLine($"Return value: {doDiscardSignal}");
            return (splittedLineStrings, doDiscardSignal);
        }

        List<ISignalModel> CheckforUDT(List<string> splittedLines)
        {
            List<ISignalModel> signals = new List<ISignalModel>();

            foreach (var word in splittedLines)
            {
                List<string> splittedWords = word.Split('"').ToList();
                if (splittedWords.Count > 1)
                {
                    char[] charsToTrim = new char[] { '\\', '/', '\"', ' '};
                    string udtFileName = word.Trim(charsToTrim);
                    string filePath = AppDomain.CurrentDomain.BaseDirectory + Constants.BaseDirectorySubDirs + udtFileName + Constants.S7UdtExtension;
                    return ParseDb(filePath, _dbNumber, _discardKeywords);
                }
            }

            return signals;
        }
        List<string> FilterOnKeywords(List<string> splittedLines)
        {
            // Check for _dbSectionKeywords
            foreach (var keyword in _dbSectionKeywords)
            {
                if (splittedLines.Contains(keyword))
                {
                    splittedLines.Clear();
                    return splittedLines;
                }
            }
            return splittedLines;
        }

        private (Enums.DataType dataType, string dataTypeStr, int byteSize) GetDataTypeAndByteSizeFromLine(string dataTypeStr)
        {
            foreach (var dataTypeLookupItem in Constants.DataTypeLookup)
            {
                if (dataTypeStr.Contains(Constants.S7DbArrayKeyword))
                {
                    int arraySize = HandleDatatpeArray(dataTypeStr);
                    return (Enums.DataType.Array, "", arraySize);
                }
                else if (dataTypeStr.Contains(dataTypeLookupItem.dataTypeStr))
                {
                    return dataTypeLookupItem;
                }
            }
            return (Enums.DataType.Bit, "", -1);
        }

        int HandleDatatpeArray(string dataTypeStr)
        {
            // Excepected input string if array is Int type and size is 3: Array[0..2] of Int

            int arrayByteSize = 0;

            List<string> splittedstrings = dataTypeStr.Split(' ').ToList<string>();
            
            if (splittedstrings.Count == 3)
            {
                // 1. Get the from and to values to get the size of the array
                string arraySizeStr = splittedstrings.First();

                //  - remove the first chars: Array[ and last: ]
                arraySizeStr = arraySizeStr.Remove(0, 6);
                arraySizeStr = arraySizeStr.Remove(arraySizeStr.Length - 1);

                //  - split  on .. to get the 2 numbers
                List<string> splittedArraySizeStr = arraySizeStr.Split(new string[] { ".." }, StringSplitOptions.None).ToList<string>();

                bool isNumeric;
                int arrayBegin;
                int arrayEnd;
                isNumeric = Int32.TryParse(splittedArraySizeStr.First(), out arrayBegin);
                isNumeric = Int32.TryParse(splittedArraySizeStr.Last(), out arrayEnd);

                // 2. Get the datatype and its bytesize
                int byteMultiplier = 0;
                foreach (var dataTypeLookupItem in Constants.DataTypeLookup)
                {
                    if (splittedstrings.Last().Contains(dataTypeLookupItem.dataTypeStr))
                    {
                        byteMultiplier = dataTypeLookupItem.byteSize;
                    }
                }

                if (isNumeric && byteMultiplier > 0)
                {
                    int arraySize = arrayEnd - arrayBegin + 1;
                    arrayByteSize = arraySize * byteMultiplier;
                }
                else
                {
                    throw new Exception("Invalid file. Array parse error");
                }
            }
            else
            {
                throw new Exception("Invalid file. Array parse error");
            }

            return arrayByteSize;
        }

        private string CreateDbAdressString(int dbNumber, Enums.DataType dataType, int byteIndex, int bit = -1)
        {
            string dbAddress = "DB";
            dbAddress += dbNumber;

            switch (dataType)
            {
                case Enums.DataType.Bit:
                    dbAddress += ".DBX" + byteIndex + '.' + bit;
                    break;
                case Enums.DataType.Byte:
                    dbAddress += ".DBB" + byteIndex;
                    break;
                case Enums.DataType.Word:
                    dbAddress += ".DBW" + byteIndex;
                    break;
                case Enums.DataType.DWord:
                    dbAddress += ".DBD" + byteIndex;
                    break;
                case Enums.DataType.Int:
                    dbAddress += ".DBW" + byteIndex;
                    break;
                case Enums.DataType.DInt:
                    dbAddress += ".DBW" + byteIndex;
                    break;
                case Enums.DataType.Real:
                    dbAddress += ".DBD" + byteIndex;
                    break;
                case Enums.DataType.Array:
                    break;
                default:
                    break;
            }
            return dbAddress;
        }

        private void UpdateByteAndBitIndex((Enums.DataType dataType, string dataTypeStr, int byteSize) datatypeAndSize)
        {
            if (datatypeAndSize.dataType != Enums.DataType.Array)
            {

                if (datatypeAndSize.dataType == Enums.DataType.Bit)
                {
                    if (_signals.Any())
                    {
                        _bitCounter++;
                    }

                    if (_bitCounter > Constants.LastBitInByte)
                    {
                        _bitCounter = 0;
                        _byteCounter++;
                    }
                }
                else
                {
                    _byteCounter += datatypeAndSize.byteSize;
                }
            }
            else
            {
                _byteCounter += datatypeAndSize.byteSize;
            }
        }

        ISignalModel CreateSignal(int index, List<string> splittedLines, (Enums.DataType dataType, string dataTypeStr, int byteSize) datatypeAndSize)
        {
            ISignalModel s = new SignalModel(index);
            s.Name = splittedLines[0];
            s.DataType = datatypeAndSize.dataType;
            s.Address = CreateDbAdressString(_dbNumber, datatypeAndSize.dataType, _byteCounter, _bitCounter);
            s.Db = _dbNumber;
            s.Byte = ByteCount;
            if (s.DataType == Enums.DataType.Bit)
            {
                s.Bit = _bitCounter;
            }
            else
            {
                s.Bit = -1;
            }

            s.DataTypeStr = datatypeAndSize.dataTypeStr;

            if (splittedLines.Count == 3)
            {
                s.Description = splittedLines[2];
            }
            return s;
        }
    }
}