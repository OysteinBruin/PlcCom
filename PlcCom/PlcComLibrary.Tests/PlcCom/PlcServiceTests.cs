using PlcComLibrary.Config;
using PlcComLibrary.Models;
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
        [InlineData("DB1.DBD1")]
        //[InlineData("DB59999.DBX100.1")]
        [InlineData("db99.DBx100.1")]
        [InlineData("DB100.DBW100")]
        public void AddressIsBoolType_ValidAddress_ReturnsTrue(string address)
        {
            TestablePlcService tps = new TestablePlcService(0, new TestableCpuConfig(), new List<Models.IDatablockModel>());

            Assert.True(tps.AddressIsBoolType(address));
        }

        [Theory]
        [InlineData("DB1.DBD1.2")]
        [InlineData("DB60000.DBD100")]
        [InlineData("db0.DBx100.1")]
        [InlineData("DB100.DBW.100")]
        public void AddressIsBoolType_InvalidAddress_ReturnsFalse(string address)
        {
            TestablePlcService tps = new TestablePlcService(0, new TestableCpuConfig(), new List<Models.IDatablockModel>());

            Assert.True(tps.AddressIsBoolType(address));
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
                int dbNumber = 4000 + i;
                var signalModels = new List<ISignalModel>();

                for (int j = 0; j < 10; j++)
                {
                    var signal = new SignalModel(j);
                    signal.Index = j;
                    signal.Name = $"Signal {i}-{j}";
                    signal.Address = (i%2 == 0) ? $"DB{dbNumber}.DBX{i}.{Math.Min(7, j)}" : $"DB{dbNumber}.DBD{j}";
                    signal.Db = dbNumber;
                    signalModels.Add(signal);
                }

                DatablockModel datablockModel = new DatablockModel(i, signalModels, $"Datablock {0}", dbNumber);
                output.Add(datablockModel);
            }

            return output;
        }
    }
}
