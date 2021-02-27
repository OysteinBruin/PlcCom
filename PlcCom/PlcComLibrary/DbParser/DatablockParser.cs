using log4net;
using PlcComLibrary.Common;
using PlcComLibrary.Factories;
using PlcComLibrary.Models.Signal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PlcComLibrary.DbParser
{
    public class DatablockParser : IDatablockParser
    {
        private BitByteIndexControl _bitByteIndexControl;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string[] _separatingStrings = { "//", ";", ":" };
        private readonly IList<string> _contentStartKeywords = new List<string> { "STRUCT", "VAR" };
        private IList<string> _discardKeywords;
        
        private IList<string> _structNames = new List<string>();
        

        public DatablockParser()
        {
            _bitByteIndexControl = new BitByteIndexControl();
        }

        /// <summary>
        /// Reads all lines of the file
        /// </summary>
        /// <param name="path"></param>
        /// <returns> A list of strings containing all lines of the files</returns>
        /// <exception>ArgumentException</exception>
        public List<string> ReadS7DbFile(string path)
        {
            List<string> fileLines = new List<string>();
            // TODO: Handle File exceptions instead of:
            FileInfo fi = new FileInfo(path);
            if (!fi.Exists)
            {
                throw new ArgumentException($"Exception: Could not find file: {path}");
            }

            fileLines = File.ReadAllLines(path).ToList();
            return fileLines;
        }

        public List<SignalModelContext> ParseDb(List<string> fileLines, IList<string> discardKeywords)
        {
            log.Info($"DatablockParser.ParseDb");
            _bitByteIndexControl.Reset();
            _discardKeywords = discardKeywords;
            List<SignalModelContext> signalContextList = new List<SignalModelContext>();

            // Return if no data from db file
            if (fileLines.Count == 0)
            {
                log.Warn($"DatablockParser.ParseDb return - no lines in file");
                return signalContextList;
            }

            bool result = RemoveHeaderData(fileLines, _contentStartKeywords);

            // TODO: HANDLE
            if (!result)
            {
                throw new Exception("Invalid file. No STRUCT or VAR keyword found.");
            }

            foreach (var line in fileLines)
            {
                (List<string> splittedLines, bool signalDiscarded) = SplitAndValidateLine(line);

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

                    if (Constants.S7DataTypes.Contains(dataTypeStr))
                    {
                        int byteSize = Constants.S7DataTypesByteSize[dataTypeStr];
                        bool isBool = (dataTypeStr == "Bool");
                        bool isFirstItem = (signalContextList.Count == 0);
                        _bitByteIndexControl.Update(byteSize, isBool, isFirstItem);

                        if (!signalDiscarded)
                        {
                            signalContextList.Add(CreateSignalContextItem(splittedLines));
                        }
                    }
                    else if (dataTypeStr.Contains(Constants.S7DbArrayKeyword))
                    {
                        signalContextList.AddRange(HandleDatatypeArray(splittedLines, signalDiscarded));
                    }
                    else if (dataTypeStr.Contains(Constants.S7DbStructKeyword))
                    {
                        _structNames.Add(splittedLines[0]);

                        // A struct always increase an odd byte value ( i.e bool
                        _bitByteIndexControl.AddedStructCorrection();
                    }
                }
            }
            log.Info($"DatablockParser.ParseDb - parse completed - signal count {signalContextList.Count}");
            return signalContextList;
        }

        
        private SignalModelContext CreateSignalContextItem(List<string> splittedLines)
        {
            if (splittedLines.Count < 2)
                return null;

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

            SignalModelContext ctx = new SignalModelContext
            {
                Name = name,
                Description = desciption,
                DataTypeStr = splittedLines[1],
                ByteIndex = _bitByteIndexControl.ByteCounter,
                BitNumber = _bitByteIndexControl.BitCounter
            };

            return ctx;
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

        (List<string> splittedLines, bool discarded) SplitAndValidateLine(string line)
        {
            List<string> splittedLineStrings = new List<string>();
            splittedLineStrings = line.Split_RemoveWhiteTokens(_separatingStrings).ToList();

            bool doDiscardSignal = false;

            foreach (var discardKeyword in _discardKeywords)
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
                    string filePath = AppDomain.CurrentDomain.BaseDirectory + Constants.BaseDirectorySubDirs + 
                        udtFileName + Constants.S7UdtExtension;


                    var lines = ReadS7DbFile(filePath);
                    if (lines?.Count == 0)
                    {
                        return signals;
                    }

                    return ParseDb(lines, _discardKeywords);
                }
            }

            return signals;
        }

        private List<SignalModelContext> HandleDatatypeArray(List<string> splittedLines, bool signalDiscarded)
        {
            Debug.Assert(splittedLines.Count == 2);

            var output = new List<SignalModelContext>();
            string dataTypeStr = splittedLines[1];

            // Excepected value of dataTypeStr if array datatype is Int  and size is 3: Array[0..2] of Int

            // Array[0..2] of Int   
            // 1. split .. and remove 0, 6 => splitted[0] = lower value
            // splitt on ']' + ' '  =>  splitted.First = upper value,  splitted.First

            // Example input string: "Array[0..2] of Int" At least 18 chars and always begins with: Array[


            if (String.IsNullOrEmpty(dataTypeStr) || dataTypeStr.Count() < 18 || !dataTypeStr.StartsWith("Array["))
            {
                throw new ArgumentException("Invalid file. Array parse error. Array string: ");
            }


            string arrayBeginStr, arrayEndStr, arrayDataTypeStr;

            // 1. split on .. and remove 0, 6 => splitted[0] == lower value
            List<string> dataTypeStrSplitted = dataTypeStr.Split(new string[] { ".." } , StringSplitOptions.None).ToList();
            arrayBeginStr = dataTypeStrSplitted[0].Remove(0, 6);
            
            List<string> splitGetUpperValueAndType = dataTypeStrSplitted[1].Split(new string[] { "]", " " } , StringSplitOptions.None).ToList();
            arrayEndStr = splitGetUpperValueAndType.First();
            arrayDataTypeStr = splitGetUpperValueAndType.Last();

            bool parseOk = false;
            int arrayBegin, arrayEnd;
            parseOk = Int32.TryParse(arrayBeginStr, out arrayBegin);
            parseOk = Int32.TryParse(arrayEndStr, out arrayEnd);

            // 2. Get the datatype and its bytesize
            int byteMultiplier = Constants.S7DataTypesByteSize[arrayDataTypeStr];

            if (parseOk && byteMultiplier > 0)
            {
                int arraySize = arrayEnd - arrayBegin + 1;
                string name = splittedLines[0];
                splittedLines[1] = arrayDataTypeStr;
                for (int i = 0; i < arraySize; i++)
                {
                    if (!signalDiscarded)
                    {
                        splittedLines[0] = name + $"[{i}]";
                        output.Add(CreateSignalContextItem(splittedLines));
                    }
                    _bitByteIndexControl.Update(byteMultiplier, arrayDataTypeStr == "Bool", false);
                }
            }
            else
            {
                throw new Exception("Invalid file. Array parse error");
            }

            return output;
        }

    }
}