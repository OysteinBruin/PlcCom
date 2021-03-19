using PlcComLibrary.Config;
using PlcComLibrary.Models;
using PlcComLibrary.PlcCom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlcComLibrary.Tests.PlcCom
{
    public class TestablePlcService : PlcService
    {
        public TestablePlcService(int index, ICpuConfig config, List<IDatablockModel> datablocks)
            : base(index, config, datablocks)
        {

        }
        public override Task ConnectAsync()
        {
            throw new NotImplementedException();
        }

        public override Task ConnectAsync(ICpuConfig config)
        {
            return base.ConnectAsync(config);
        }

        public override void DisConnect()
        {
            throw new NotImplementedException();
        }

        public override Task PulseBitAsync(string address)
        {
            throw new NotImplementedException();
        }

        public override Task ReadSingleAsync(string address)
        {
            throw new NotImplementedException();
        }

        public override Task ToggleBitAsync(string address)
        {
            throw new NotImplementedException();
        }

        public override Task WriteSingleAsync(string address, object value)
        {
            throw new NotImplementedException();
        }

        public new bool AddressIsBoolType(string address)
        {
            return base.AddressIsBoolType(address);
        }

        public new (int dbIndex, int signalIndex) GetIndexFromAddress(string address, List<IDatablockModel> datablocks)
        {
            return base.GetIndexFromAddress(address, datablocks);
        }

        public new void VerifyConnected()
        {
            base.VerifyConnected();
        }

        public new bool VerifyPlcAddressStr(string address, List<IDatablockModel> datablocks)
        {
            return VerifyPlcAddressStr(address, datablocks);
        }

        public new void VerifyConnectedAndValidateAddress(string address)
        {
            base.VerifyConnectedAndValidateAddress(address);
        }

        protected override Task ReadDbAsync(IDatablockModel db)
        {
            throw new NotImplementedException();
        }

        protected override void PlcReadWriteCallback(object state)
        {
            throw new NotImplementedException();
        }
    }
}
