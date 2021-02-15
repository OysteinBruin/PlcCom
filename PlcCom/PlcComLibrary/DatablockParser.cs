using log4net;
using PlcComLibrary.Common;
using PlcComLibrary.Factories;
using PlcComLibrary.Models.Signal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlcComLibrary
{
    public class DatablockParser : IDatablockParser
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string[] _separatingStrings = { "//", ";", ":" };
        private readonly IList<string> _contentStartKeywords = new List<string> { "STRUCT", "VAR" };
        private string _path;
        private int _dbNumber;
        private List<string> _discardKeywords;
        private int _bitCounter = 0;
        private int _byteCounter = 0;
        private IList<string> _structNames = new List<string>();


        public int FirstByte { get; } = 0;
        public int ByteCount { get; } = 0;

        public List<SignalModelContext> ParseDb(string path, int dbNumber, List<string> discardKeywords)
        {
            log.Info($"DatablockParser.ParseDb path {_path} db {dbNumber}");
            _path = path;
            _dbNumber = dbNumber;
            _discardKeywords = discardKeywords;
            List<SignalModelContext> signalContextList = new List<SignalModelContext>();
            List<string> fileLines = ReadS7DbFile(_path);


            // Return if no data from db file
            if (fileLines.Count == 0)
            {
                log.Warn($"DatablockParser.ParseDb return - no lines in file");
                return signalContextList;
            }

            _bitCounter = 0;
            _byteCounter = 0;

            bool result = RemoveHeaderData(fileLines, _contentStartKeywords);

            // TODO: HANDLE
            if (!result)
            {
                throw new Exception("Invalid file. No STRUCT or VAR keyword found.");
            }

            foreach (var line in fileLines)
            {
                (List<string> splittedLines, bool signalDiscarded) = SplitAndValidateLine(line, _discardKeywords);

                IList<SignalModelContext> signalsFromUdt = CheckforUDT(splittedLines);
                signalContextList.AddRange(signalsFromUdt);


                if (splittedLines.Count == 1)
                {
                    if (splittedLines[0] == "END_STRUCT")
                    {
                        if (_structNames.Count > 0)
                            _structNames.RemoveAt(_structNames.Count-1);
                    }

                }  
                else if (splittedLines.Count >= 2 && splittedLines.Count <= 3)
                {
                    // The datatype kan be struct, Array or one of the target data types (Constants.S7DataTypes)
                    string dataTypeStr = splittedLines[1];
                    int dataTypeByteCount = -1;

                    if (Constants.S7DataTypes.Contains(dataTypeStr))
                    {
                        dataTypeByteCount = Constants.S7DataTypesByteSize[dataTypeStr];

                        if (!signalDiscarded)
                        {

                            string name = String.Empty;
                            foreach (var structName in _structNames)
                            {
                                name += structName + '.';
                            }
                            name += splittedLines[0];

                            string desciption = String.Empty;
                            if (splittedLines.Count == 3)
                            {
                                desciption = splittedLines.Last();
                            }

                            SignalModelContext ctx = new SignalModelContext {
                                Name = name,
                                Description = desciption,
                                DataTypeStr = dataTypeStr,
                                ByteIndex = _byteCounter,
                                BitNumber = _bitCounter
                            };

                            signalContextList.Add(ctx);
                        }

                        UpdateByteAndBitIndex(dataTypeByteCount, false, dataTypeStr == "Bool", signalContextList.Count == 0);
                    }
                    else if (dataTypeStr.Contains(Constants.S7DbArrayKeyword))
                    {
                        UpdateByteAndBitIndex(dataTypeByteCount, true, false, signalContextList.Count == 0);
                    }
                    else if (dataTypeStr.Contains(Constants.S7DbStructKeyword))
                    {
                        _structNames.Add(splittedLines[0]);
                    }
                }
            }
            log.Info($"DatablockParser.ParseDb - parse completed - signal count {signalContextList.Count}");
            return signalContextList;
        }

        /// <summary>
        /// TODOTODOTODOTODOTODOTODOTODO   TODO: Handle File exceptions
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<string> ReadS7DbFile(string path)
        {
            // Console.WriteLine($"S7DbParser.ReadS7DbFile: {path}");
            
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
            splittedLineStrings = line.Split_RemoveWhiteTokens(_separatingStrings).ToList();

            bool doDiscardSignal = false;

            foreach (var discardKeyword in discardKeywords)
            {
                List<string> containsDiscardKeywordList = splittedLineStrings.Where(str => str.Contains(discardKeyword)).ToList();

                if (containsDiscardKeywordList.Count > 0)
                {
                    doDiscardSignal = true;
                }
            }
            return (splittedLineStrings, doDiscardSignal);
        }

        /// <summary>
        /// Check if the current line is a UDT
        /// </summary>
        /// <param name="splittedLines"></param>
        /// <returns></returns>
        private List<SignalModelContext> CheckforUDT(List<string> splittedLines)
        {
            List<SignalModelContext> signals = new List<SignalModelContext>();

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
        //private List<string> FilterOnKeywords(List<string> splittedLines)
        //{
        //    // Check for _dbSectionKeywords
        //    foreach (var keyword in _dbSectionKeywords)
        //    {
        //        if (splittedLines.Contains(keyword))
        //        {
        //            splittedLines.Clear();
        //            return splittedLines;
        //        }
        //    }
        //    return splittedLines;
        //}

        private int HandleDatatypeArray(string dataTypeStr)
        {
            // Excepected input string if array is Int type and size is 3: Array[0..2] of Int

            // Array[0..2] of Int   
            // 1. split .. and remove 0, 6 => splitted[0] = lower value
            // splitt on ']' + ' '  =>  splitted.First = upper value,  splitted.First

            // Example input string: "Array[0..2] of Int" At least 18 chars and always begins with: Array[
            if (String.IsNullOrEmpty(dataTypeStr) || dataTypeStr.Count() < 18 || !dataTypeStr.StartsWith("Array["))
            {
                throw new ArgumentException("Invalid file. Array parse error. Array string: ");
            }

            int arrayByteSize = 0;
            string arrayBeginStr, arrayEndStr, arrayDataTypeStr;


            // 1. split on .. and remove 0, 6 => splitted[0] == lower value
            List<string> dataTypeStrSplitted = dataTypeStr.Split(new string[] { ".." } , StringSplitOptions.None).ToList();
            arrayBeginStr = dataTypeStrSplitted[0].Remove(0, 6);
            
            List<string> splitGetUpperValueAndType = dataTypeStrSplitted[1].Split(new string[] { "]", " " } , StringSplitOptions.None).ToList();
            arrayEndStr = splitGetUpperValueAndType.First();
            arrayDataTypeStr = splitGetUpperValueAndType.Last();


            bool parseOk;
            int arrayBegin;
            int arrayEnd;
            parseOk = Int32.TryParse(arrayBeginStr, out arrayBegin);
            parseOk = Int32.TryParse(arrayEndStr, out arrayEnd);

            // 2. Get the datatype and its bytesize
            int byteMultiplier = Constants.S7DataTypesByteSize[arrayDataTypeStr];

            if (parseOk && byteMultiplier > 0)
            {
                int arraySize = arrayEnd - arrayBegin + 1;
                arrayByteSize = arraySize * byteMultiplier;
            }
            else
            {
                throw new Exception("Invalid file. Array parse error");
            }
          
            return arrayByteSize;
        }

        

        private void UpdateByteAndBitIndex(int byteSize, bool isArrayType, bool isBoolType, bool isFirstItem)
        {
            if (!isArrayType)
            {

                if (isBoolType)
                {
                    if (!isFirstItem)
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
                    _byteCounter += byteSize;
                }
            }
            else
            {
                _byteCounter += byteSize;
            }
        }
    }
}