using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlcComLibrary.Common
{
    public static class StringExtensions
    {
        // https://stackoverflow.com/questions/10682301/stringsplitoptions-removeemptyentries-doesnt-work-as-advertised/10682341
        public static IEnumerable<string> Split_RemoveWhiteTokens(string s, params char[] separator)
        {
            return s.Split(separator, System.StringSplitOptions.RemoveEmptyEntries).
                  Select(tag => tag.Trim()).
                  Where(tag => !string.IsNullOrEmpty(tag));
        }

        public static IEnumerable<string> Split_RemoveWhiteTokens(string s, params string[] separator)
        {
            return s.Split(separator, System.StringSplitOptions.RemoveEmptyEntries).
                  Select(tag => tag.Trim()).
                  Where(tag => !string.IsNullOrEmpty(tag));
        }

    }
}
