﻿using log4net;
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
        private List<string> _fileLines;
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
            _fileLines = fileLines;

            // Return if no data from db file
            if (_fileLines.Count == 0)
            {
                log.Warn($"DatablockParser.ParseDb return - no lines in file");
                return new List<SignalModelContext>();
            }

            return ParseDb();
        }

        private List<SignalModelContext> ParseDb()
        {
            bool result = RemoveHeaderData(_fileLines, _contentStartKeywords);

            // TODO: HANDLE
            if (!result)
            {
                throw new Exception("Exception: Invalid file. No STRUCT or VAR keyword found.");
            }

            List<SignalModelContext> signalContextList = new List<SignalModelContext>();
            foreach (var line in _fileLines)
            {
                var lineItem = new DbFileLineItem(line, _discardKeywords);

                if (lineItem.IsValid)
                {
                    if (lineItem.IsStruct)
                    {
                        _structNames.Add(lineItem.Name);
                    }

                    int currentByteCount = _bitByteIndexControl.ByteCounter;
                    IList<SignalModelContext> signalsFromUdt = CheckForUDT(lineItem);
                    signalContextList.AddRange(signalsFromUdt);

                    IList<SignalModelContext> signalsFromArray = CheckForArrayType(lineItem);
                    signalContextList.AddRange(signalsFromArray);


                    if (currentByteCount == _bitByteIndexControl.ByteCounter)
                    {
                        _bitByteIndexControl.Update(lineItem);

                        if (!lineItem.IsDiscarded)
                        {
                            signalContextList.Add(CreateSignalContextItem(lineItem));
                        }
                    }

                }
            }
            log.Info($"DatablockParser.ParseDb - parse completed - signal count {signalContextList.Count}");
            return signalContextList;
        }

        
        private SignalModelContext CreateSignalContextItem(DbFileLineItem lineItem)
        {
            if (!lineItem.IsDataType)
                return null;

            string name = String.Empty;
            foreach (var structName in _structNames)
            {
                name += structName + '.';
            }
            name += lineItem.Name;

            SignalModelContext ctx = new SignalModelContext
            {
                Name = name,
                Description = lineItem.Description,
                DataTypeStr = lineItem.DataTypeStr,
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

        /// <summary>
        /// Check if the current line is a UDT
        /// </summary>
        /// <param name="splittedLines"></param>
        /// <returns></returns>
        private List<SignalModelContext> CheckForUDT(DbFileLineItem lineItem)
        {
            List<string> splittedUdtName = lineItem.DataTypeStr.Split('"').ToList();
            if (splittedUdtName.Count > 1)
            {
                char[] charsToTrim = new char[] { '\\', '/', '\"', ' ' };
                string udtFileName = lineItem.DataTypeStr.Trim(charsToTrim);

                string filePath = AppDomain.CurrentDomain.BaseDirectory + Constants.BaseDirectorySubDirs +
                        udtFileName + Constants.S7UdtExtension;


                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    _fileLines = ReadS7DbFile(filePath);
                    if (_fileLines?.Count == 0)
                    {
                        return new List<SignalModelContext>();
                    }

                    _structNames.Add(lineItem.Name);
                    var signals = ParseDb();
                    if (_structNames.Count > 0)
                    {
                        _structNames.RemoveAt(_structNames.Count - 1);
                    }

                    return signals;
                }
            }
            return new List<SignalModelContext>();
        }

        private List<SignalModelContext> CheckForArrayType(DbFileLineItem lineItem)
        {
            var output = new List<SignalModelContext>();
           

            // Excepected value of dataTypeStr if array datatype is Int  and size is 3: Array[0..2] of Int

            // Array[0..2] of Int   
            // 1. split .. and remove 0, 6 => splitted[0] = lower value
            // splitt on ']' + ' '  =>  splitted.First = upper value,  splitted.First

            // Example input string: "Array[0..2] of Int" At least 18 chars and always begins with: Array[


            if (String.IsNullOrEmpty(lineItem.DataTypeStr) || lineItem.DataTypeStr.Count() < 18 || !lineItem.DataTypeStr.Contains("Array["))
            {
                return output;
            }


            string arrayBeginStr, arrayEndStr, arrayDataTypeStr;

            // 1. split on .. and remove 0, 6 => splitted[0] == lower value
            List<string> dataTypeStrSplitted = lineItem.DataTypeStr.Split(new string[] { ".." } , StringSplitOptions.None).ToList();
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
                string name = lineItem.Name;
                lineItem.DataTypeStr = arrayDataTypeStr;
                for (int i = 0; i < arraySize; i++)
                {
                    _bitByteIndexControl.Update(lineItem);
                    if (!lineItem.IsDiscarded)
                    {
                        lineItem.Name = name + $"[{i}]";
                        output.Add(CreateSignalContextItem(lineItem));
                    }
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