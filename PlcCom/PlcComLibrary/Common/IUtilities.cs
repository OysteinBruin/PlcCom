using PlcComLibrary.Models;
using System;
using System.Collections.Generic;

namespace PlcComLibrary.Common
{
    public interface IUtilities
    {
        List<string> LoadAppConfigFiles(string path = "");
        bool AddressIsBoolType(string address);
        (int dbIndex, int signalIndex) GetSignalIndexFromAddress(string address, List<IDatablockModel> datablocks);
        bool VerifyPlcAddressStr(string address, List<IDatablockModel> datablocks);

        Int32 BytesToInt(byte[] bytes);

        float BytesToFloat(byte[] bytes);

        bool ByteToBool(byte[] boolByte);
    }
}