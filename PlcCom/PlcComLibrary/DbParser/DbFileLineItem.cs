using PlcComLibrary.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.DbParser
{
    public class DbFileLineItem
    {
        private string[] _separatingStrings = { "//", ";", ":" };

        public DbFileLineItem()
        {
        }

        public DbFileLineItem(string line, IList<string> _discardKeywords)
        {
            if (line == null || _discardKeywords == null)
                return;

            List<string> lineStrings = new List<string>();

            // Remove curly brace content from line
            // e.g: { ExternalAccessible := 'False'; ExternalVisible := 'False'; ExternalWritable := 'False'}
            int beginCurlyIdx = line.IndexOf('{');
            int endCurlyIdx = line.IndexOf('}');
            if (endCurlyIdx > beginCurlyIdx && beginCurlyIdx >= 0)
            {
                string[] separators = { "{", "}" };
                lineStrings = line.Split_RemoveWhiteTokens(separators).ToList();

                if (lineStrings.Count == 3)
                {
                    line = lineStrings.First() + lineStrings.Last();
                }
            }

            // Separate line into name, datatype and optional description 
            lineStrings = line.Split_RemoveWhiteTokens(_separatingStrings).ToList();

            if (lineStrings.Count == 1)
            {
                if (lineStrings[0] == "END_STRUCT")
                {
                    IsEndOfStruct = true;
                }
            }
            else if (lineStrings.Count >= 2 && lineStrings.Count <= 3)
            {
                Name = lineStrings[0];
                DataTypeStr = lineStrings[1];
                
                if (lineStrings.Count == 3)
                    Description = lineStrings[2];

                if (Constants.S7DataTypes.Contains(DataTypeStr))
                {
                    IsDataType = true;
                    IsBoolType = (DataTypeStr == "Bool");
                }
                else if (DataTypeStr.Contains(Constants.S7DbArrayKeyword))
                {
                    IsDataType = true;
                    IsArrayType = true;
                }
                else if (DataTypeStr.Contains(Constants.S7DbStructKeyword))
                {
                    IsStruct = true;
                }
                else if(DataTypeStr.Split('"').ToList().Count > 1)
                {
                    IsUdp = true;
                }
            }

            // Check if signal is to be disacrded
            foreach (var discardKeyword in _discardKeywords)
            {
                List<string> containsDiscardKeywordList = lineStrings.Where(str => str.Contains(discardKeyword)).ToList();

                if (containsDiscardKeywordList.Count > 0)
                {
                    IsDiscarded = true;
                }
            }

            if (!IsDataType)
            {
                IsDiscarded = true;
            }

            if (IsDataType || IsEndOfStruct || IsArrayType || IsBoolType || IsUdp)
            {
                IsValid = true;
            }
        }

        public string Name { get; set; } = String.Empty;
        public string DataTypeStr { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public bool IsDataType { get; set; } = false;
        public bool IsStruct { get; set; } = false;
        public bool IsEndOfStruct { get; set; } = false;
        public bool IsArrayType { get; set; } = false;
        public bool IsBoolType { get; set; } = false;
        public bool IsUdp { get; set; } = false;
        public bool IsDiscarded { get; set; } = false;
        public bool IsValid { get; set; } = false;
    }
}
