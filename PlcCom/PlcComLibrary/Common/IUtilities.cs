using PlcComLibrary.Models;
using System.Collections.Generic;

namespace PlcComLibrary.Common
{
    public interface IUtilities
    {
        List<string> LoadAppConfigFiles();
        bool AddressIsBoolType(string address);
        (int dbIndex, int signalIndex) GetSignalIndexFromAddress(string address, List<IDatablockModel> datablocks);
        bool VerifyPlcAddressStr(string address, List<IDatablockModel> datablocks);
    }
}