using PlcComLibrary.Config;
using PlcComLibrary.Factories;
using PlcComLibrary.Models;
using PlcComLibrary.Models.Signal;
using PlcComLibrary.Tests.PlcCom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PlcComLibrary.Tests
{
    // https://stackoverflow.com/questions/13908106/unit-testing-abstract-classes-and-protected-methods-inside-them/13910393

    public class PlcServiceTests
    {

        [Theory]
        [InlineData("DB1.DBX1.7")]
        [InlineData("DB2.DBX100.7")]
        [InlineData("DB59999.DBX0.0")]
        [InlineData("DB99.DBX1024.4")]
        public void AddressIsBoolType_ValidAddress_ReturnsTrue(string address)
        {
            TestablePlcService tps = new TestablePlcService(0, new TestableCpuConfig(), new List<Models.IDatablockModel>());

            Assert.True(tps.AddressIsBoolType(address));
        }

        [Theory]
        [InlineData("DB1.DBD1.2")]
        [InlineData("DB60000.DBD100")]
        [InlineData("db0.DBX100.5")]
        [InlineData("DB100.DBW.100")]
        public void AddressIsBoolType_InvalidAddress_ReturnsFalse(string address)
        {
            TestablePlcService tps = new TestablePlcService(0, new TestableCpuConfig(), new List<Models.IDatablockModel>());

            Assert.True(!tps.AddressIsBoolType(address));
        }

        [Fact]
        public void GetIndexFromAddress_ExistingAddress_ReturnsValidIndex()
        {
            TestablePlcService tps = new TestablePlcService(0, new TestableCpuConfig(), new List<Models.IDatablockModel>());

            List<IDatablockModel> datablocks = GetFakeDatablockModels();

            for (int i = 0; i < datablocks.Count; i++)
            {
                for (int j = 0; j < datablocks[i].Signals.Count; j++)
                {
                    (int, int) result = tps.GetIndexFromAddress(datablocks[i].Signals[j].Address, datablocks);
                    Assert.Equal((i, j), result);
                }
            } 
        }

        [Theory]
        [InlineData("DB100.DBD2")]
        [InlineData("DB4001.DBD100")]
        [InlineData("DB4002.DBD0")]
        [InlineData("DB4003.DBW100")]
       public void GetIndexFromAddress_NonExistingAddress_ReturnsInvalidIndex(string address)
        {
            TestablePlcService tps = new TestablePlcService(0, new TestableCpuConfig(), new List<Models.IDatablockModel>());

            (int, int) result = tps.GetIndexFromAddress(address, GetFakeDatablockModels());
            Assert.Equal((-1, -1), result);
        }

        private List<IDatablockModel> GetFakeDatablockModels()
        {
            var output = new List<IDatablockModel>();

            for (int i = 0; i < 10; i++)
            {
                var signalModels = new List<SignalModel>();

                for (int j = 0; j < 10; j++)
                {

                    var ctx = new SignalModelContext
                    {
                        Index = j,
                        DbIndex = i,
                        CpuIndex = 0,
                        Name = $"Signal {i}-{j}",
                        DbNumber = 4000 + i,
                        DataTypeStr = "Real"
                    };

                    var signal = SignalFactory.Create(ctx);
                    signalModels.Add(signal);
                }
                
                
                IDatablockModel datablockModel = new DatablockModel { };
                output.Add(datablockModel);
            }

            return output;
        }
    }
}
