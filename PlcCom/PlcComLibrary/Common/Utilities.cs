using PlcComLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PlcComLibrary.Common
{
    public class Utilities : IUtilities
    {
        public List<string> LoadAppConfigFiles(string path = "")
        {
            if (String.IsNullOrEmpty(path))
            {
                path = AppDomain.CurrentDomain.BaseDirectory + Constants.BaseDirectorySubDirs;
            }
            
            /// TODOTODOTODOTODOTODOTODOTODO   TODO: Handle File exceptions
            FileInfo fi = new FileInfo(path);
            List<string> appFiles = new List<string>();

            if (!fi.Directory.Exists)
            {
                //TODO - Directory.CreateDirectory -  Maybe create the dirs
                return appFiles;
            }
            appFiles = Directory.GetFiles(path).ToList();
            return appFiles;
        }

        public bool AddressIsBoolType(string address)
        {
            // Validate address with regular expression
            var regex = new Regex(Constants.SignalAddressBoolRegExp, RegexOptions.IgnoreCase);

            if (regex.IsMatch(address))
            {
                return true;
            }
            return false;
        }

        public (int dbIndex, int signalIndex) GetSignalIndexFromAddress(string address, List<IDatablockModel> datablocks)
        {
            // Validate address with regular expression
            var regex = new Regex(Constants.SignalAddressRegExp, RegexOptions.IgnoreCase);

            if (!regex.IsMatch(address))
            {
                return (-1, -1);
            }

            // Get db number from adddress
            int dbNumber;
            address = address.Remove(2); // Remove "db" 
            List<string> strList =  address.Split('.').ToList();

            bool successfullyParsed = int.TryParse(strList[0], out dbNumber);
            if (successfullyParsed)
            {
                // Find the signal in Datablocks list
                for (int i = 0; i < datablocks.Count; i++)
                {
                    if (datablocks[i].Number == dbNumber)
                    {
                        for (int j = 0; j < datablocks[i].Signals.Count; j++)
                        {
                            if (datablocks[i].Signals[j].Address == address)
                            {
                                return (i, j);
                            }
                        }
                    }
                }
            }

            return (-1, -1);
        }

        public bool VerifyPlcAddressStr(string address, List<IDatablockModel> datablocks)
        {
            (int dbIndex, int signalIndex) = GetSignalIndexFromAddress(address, datablocks);

            if (dbIndex >= 0 && signalIndex >= 0)
            {
                return true;
            }
            return false;
        }

        public Int32 BytesToInt(byte[] bytes)
        {
            Int32 d = 0;

            if (bytes.Length == sizeof(Int32))
            {
                for (int i = 0; i < sizeof(Int32); ++i)
                {
                    d |= (bytes[i] << (i * 8));
                }
            }

            return d;
        }

        public float BytesToFloat(byte[] bytes)
        {
            Int32 d = BytesToInt(bytes);

            // TODO: To be verified
            return  (float)d;
        }

        public bool ByteToBool(byte[] boolByte)
        {
            if (boolByte?[0] == 0)
                return false;

            return true;
        }


        /*
       
        Bytes to/from functions reference: From my previous C++ Qt version:


       int32_t ComUtils::bytesToInt(unsigned char bytes[])
        {
            int32_t d = 0;
            for (size_t i = 0; i < sizeof(int32_t); ++i) {
                d |= (bytes[i] << (i * 8) );
            }
            return d;
        }

        float ComUtils::bytesToFloat(unsigned char bytes[])
        {
            int32_t intVal = bytesToInt(bytes);
            return *(float *)&intVal;
        }

        bool ComUtils::byteToBool(unsigned char byte[])
        {
            if (byte[0] == 0)
                return false;

            return true;
        }

        int32_t ComUtils::bytesToInt(const std::vector<unsigned char> bytesVec)
        {
            if (bytesVec.size() < sizeof (int32_t))
                return 0;

            int32_t d = 0;
            for (size_t i = 0; i < sizeof(int32_t); ++i) {
                d |= (bytesVec.at(i) << (i * 8) );
            }
            return d;
        }

        float ComUtils::bytesToFloat(const std::vector<unsigned char> bytesVec)
        {
            if (bytesVec.size() != sizeof (int32_t))
                return 0.0;

            int32_t intVal = bytesToInt(bytesVec);
            return *(float *)&intVal;
        }

        bool ComUtils::byteToBool(const std::vector<unsigned char> bytesVec)
        {
            if (bytesVec.size() != 1) // FIX ME - should implement some error reporting
                return false;

            if (bytesVec.at(0) == 0)
                return false;

            return true;
        }


        std::vector<unsigned char> ComUtils::intToBytes(const int32_t val)
        {
            std::vector<unsigned char> bytes;
            bytes.push_back(val & 0x00FF);
            bytes.push_back((val & 0xFF00) >> 8);
            bytes.push_back((val & 0xFF0000) >> 16);
            bytes.push_back((val & 0xFF000000) >> 24);
            return bytes;
        }

        std::vector<unsigned char> ComUtils::floatToBytes(const float val)
        {
            int32_t d = *(int32_t *)&val;
            return intToBytes(d);
        }

        unsigned char ComUtils::boolToByte(const bool val)
        {
            unsigned char boolVal = 0x0;
            if (val)
                boolVal = 0xFF;

            return boolVal;
        }

        std::vector<unsigned char> ComUtils::valueToBytes(const QVariant val, const QString &dataType)
        {
            bool ok = false;

            if (dataType == QString{"Float"}
                    || dataType == QString("float")
                    || dataType == QString{"Real"}
                    || dataType == QString{"real"}) {
                float floatVal = val.toFloat(&ok);
                if (!ok) return std::vector<unsigned char>{};
                return ComUtils::floatToBytes(floatVal);
            }
            else if (dataType == QString{"uint32_t"}
                    || dataType == QString("int")
                    || dataType == QString{"Int"}) {
                int32_t intVal = val.toInt(&ok);
                if (!ok) return std::vector<unsigned char>{};
                return  ComUtils::intToBytes(intVal);
            }
            else if (dataType == QString{"Bool"}
                    || dataType == QString("bool")
                    || dataType == QString{"Boolean"}
                    || dataType == QString{"boolean"}) {
                return std::vector<unsigned char>{ComUtils::boolToByte(val.toBool())};
            }
            else return std::vector<unsigned char>{};
        }

        QVariant ComUtils::bytesToValue(const std::vector<unsigned char> bytesVec, const QString &dataType)
        {
            if (dataType == QString{"Float"}
                    || dataType == QString("float")
                    || dataType == QString{"Real"}
                    || dataType == QString{"real"}) {
                float floatVal = bytesToFloat(bytesVec);
                return QVariant{floatVal};
            }
            else if (dataType == QString{"uint32_t"}
                    || dataType == QString("int")
                    || dataType == QString{"Int"}) {
                int32_t intVal = bytesToInt(bytesVec);
                return QVariant{intVal};
            }
            else if (dataType == QString{"Bool"}
                    || dataType == QString("bool")
                    || dataType == QString{"Boolean"}
                    || dataType == QString{"boolean"}) {
                bool boolVal = byteToBool(bytesVec);
                return QVariant{boolVal};
            }
            else return QVariant{};
        } 
     */
    }
}
