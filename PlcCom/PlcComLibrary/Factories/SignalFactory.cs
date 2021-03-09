using PlcComLibrary.Common;
using PlcComLibrary.Models.Signal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PlcComLibrary.Factories
{
    public static class SignalFactory
    {
        public static SignalModel Create(SignalModelContext context)
        {
            string typeSpecifier = Constants.S7DataTypeSpecifiers[context.DataTypeStr];
            // DB450.DBD624 
            string address = "DB" + context.DbNumber + ".DB" + Constants.S7DataTypeSpecifiers[context.DataTypeStr] + context.ByteIndex;

            if (typeSpecifier == "X")
            {
                address += '.';
                address += context.BitNumber;
            }

            if (!Constants.S7DataTypes.Contains(context.DataTypeStr) || !AddressIsValid(address))
            {
                throw new ArgumentException($"Invalid data type: {context.DataTypeStr} " +
                    $"Unable to create signal {context.Name}. Datablock creation aborted.");
            }

            context.Address = address;


            switch (context.DataTypeStr)
            {
                case "Bool":
                    return new BoolSignalModel(context);

                case "Byte":
                    return new Int32SignalModel(context);

                case "Int":
                    return new Int16SignalModel(context);
                case "UInt":
                    return new Int16SignalModel(context);

                case "DInt":
                    return new Int32SignalModel(context);
                case "UDInt":
                    return new Int32SignalModel(context);

                case "Word":
                    return new Int16SignalModel(context);

                case "DWord":

                    return new Int32SignalModel(context);

                case "Real":
                    return new FloatSignalModel(context);

                default:
                    throw new ArgumentException($"Invalid data type: {context.DataTypeStr} " +
                        $"Unable to create signal {context.Name}. Datablock creation aborted.");
            }
            
        }

        // TODO: consider moving this to a helper class if others need it
        private static bool AddressIsValid(string address)
        {
            // Validate address with regular expression
            var regex = new Regex(Constants.SignalAddressRegExp, RegexOptions.IgnoreCase);

            if (regex.IsMatch(address))
            {
                return true;
            }
            return false;
        }
    }
}
