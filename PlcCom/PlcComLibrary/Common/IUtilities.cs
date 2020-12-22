using System.Collections.Generic;

namespace PlcComLibrary.Common
{
    public interface IUtilities
    {
        List<string> LoadAppConfigFiles();
        bool AddressIsBoolType(string address);
        (int dbIndex, int signalIndex) GetSignalIndexFromAddress(string address, List<IDatablock> datablocks);
        bool VerifyPlcAddressStr(string address, List<IDatablock> datablocks);
    }
}