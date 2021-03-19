using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlcComLibrary.Common;
using System.Text.RegularExpressions;
using Xunit;
using Assert = Xunit.Assert;

namespace PlcComLibrary.Tests
{

    public class ConstantsTests
    {
        [Theory]
        [InlineData("DB1.DBD1")]
        [InlineData("DB59999.DBX100.1")]
        [InlineData("db99.DBx100.1")]
        [InlineData("DB100.DBW100")]
        public void SignalAddressRegExp_ShouldMatch(string input)
        {
            var regex = new Regex(Constants.SignalAddressRegExp, RegexOptions.IgnoreCase);
            Assert.Matches(regex, input);
        }

        [Theory]
        [InlineData("DB1.DBD1.2")]
        [InlineData("DB100000.DBD100")]
        [InlineData("db0.DBx100.1")]
        [InlineData("DB100.DBW.100")]
        public void SignalAddressRegExp_ShouldNotMatch(string input)
        {
            var regex = new Regex(Constants.SignalAddressRegExp, RegexOptions.IgnoreCase);
            Assert.DoesNotMatch(regex, input);
        }
    }
}
