using PlcComLibrary.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.DbParser
{
    public class BitByteIndexControl
    {
        //private int _byteCouter;
        //private int _previousByteCountValue;
        //private int _previousByteSize;
        //private bool _previousItemWasBool;
        //private int _bytesRemaining = 0;
        //private int _previousNewSectionByteCount = 0;
        //private bool _previousIterationWasNewSection = false;

        private DbFileLineItem _previousLineItem;
        private DbFileLineItem _previousDataTypeLineItem;

        public BitByteIndexControl()
        {
            // Reset();
            _previousDataTypeLineItem = new DbFileLineItem();
        }
        
        public int ByteCounter { get; private set; }
    
        public int BitCounter { get; private set; }


        public void Update(DbFileLineItem lineItem)
        {
            if (lineItem.Name.Contains("ParkedPos") )
            {

            }
            if (_previousLineItem != null && _previousLineItem.Name.Contains("SpinO"))
            {

            }
            if (_previousLineItem != null && _previousLineItem.IsBoolType && lineItem.DataTypeStr == "Real")
            {

            }

            if (_previousLineItem == null)
            {
                BitCounter = 0;
                ByteCounter = 0;
            }
            else
            {
                if (lineItem.IsDataType)
                {
                    if (lineItem.IsBoolType)
                    {
                        HandleBooleanType();
                    }
                    else
                    {
                        HandleNumericType();
                    }
                }
            }

           
             _previousLineItem = lineItem;

            if (lineItem.IsDataType)
            {
                _previousDataTypeLineItem = lineItem;
            }
        }

        private void HandleBooleanType()
        {



            if (_previousLineItem.IsDataType)
            {
                if (_previousLineItem.IsBoolType)
                {
                    BitCounter++;
                    if (BitCounter > Constants.LastBitInByte)
                    {
                        BitCounter = 0;
                        ByteCounter++;
                    }
                }
                else
                {
                    BitCounter = 0;
                    ByteCounter += Constants.S7DataTypesByteSize[_previousLineItem.DataTypeStr];
                }
            }
            else
            {
                BitCounter = 0;
                if (_previousDataTypeLineItem.IsDataType)
                {
                    int previousDataTypeLineItemBytesize = Constants.S7DataTypesByteSize[_previousDataTypeLineItem.DataTypeStr];

                    ByteCounter += previousDataTypeLineItemBytesize;

                    if (ByteCounter % 2 != 0)
                    {
                        ByteCounter += 1;
                    }
                }
                else
                {
                    ByteCounter += (ByteCounter % 2 == 0 ? 2 : 1);
                }
                
            }
        }

        private void HandleNumericType()
        {
            BitCounter = 0;

            if (_previousLineItem.IsDataType)
            {
                if (_previousLineItem.IsBoolType)
                {
                    ByteCounter += (ByteCounter % 2 == 0 ? 2 : 1);
                }
                else
                {
                    ByteCounter += Constants.S7DataTypesByteSize[_previousLineItem.DataTypeStr];
                }
            }
            else
            {
                // Add 
                if (_previousDataTypeLineItem.IsDataType)
                {
                    int previousDataTypeLineItemBytesize = Constants.S7DataTypesByteSize[_previousDataTypeLineItem.DataTypeStr];

                    ByteCounter += previousDataTypeLineItemBytesize;

                    if (ByteCounter % 2 != 0)
                    {
                        ByteCounter += 1;
                    }
                }
                else
                {
                    ByteCounter += (ByteCounter % 2 == 0 ? 2 : 1);
                }
            }
        }

        //public void Update(int byteSize, bool isBoolType)
        //{
        //    if (isBoolType)
        //    {
        //        if (!_previousIterationWasNewSection && _previousItemWasBool/* && _previousByteCountValue == ByteCounter*/)
        //        {
        //            BitCounter++;
        //        }

        //        if (BitCounter > Constants.LastBitInByte)
        //        {
        //            BitCounter = 0;
        //            ByteCounter++;
        //        }

        //        if (!_previousItemWasBool)
        //        {
        //            ByteCounter += _previousByteSize;
        //        }
        //    }
        //    else
        //    {
        //        if (_previousItemWasBool)
        //        {
        //            BitCounter = 0;

        //            if (!_previousIterationWasNewSection)
        //            {
        //                if (ByteCounter % 2 == 0)
        //                {
        //                    ByteCounter += 2;
        //                    _bytesRemaining = byteSize - 2;

        //                    //_bytesRemaining = byteSize;
        //                }
        //                else
        //                {
        //                    ByteCounter += 1;
        //                    _bytesRemaining = byteSize - 1;

        //                    //_bytesRemaining = byteSize + 1;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (!_previousIterationWasNewSection)
        //            {
        //                if (byteSize > 2)
        //                {
        //                    ByteCounter += 2;
        //                }
        //                else ByteCounter += byteSize;

        //                ByteCounter += _bytesRemaining;
        //                _bytesRemaining = 0;

        //                //ByteCounter += _bytesRemaining;
        //                //_bytesRemaining = byteSize;
        //            }
        //        }
        //    }

        //    _previousByteSize = byteSize;
        //    _previousItemWasBool = isBoolType;
        //    _previousByteCountValue = ByteCounter;
        //    _previousIterationWasNewSection = false;
        //}

        //public void NewSectionCorrection()
        //{
        //    BitCounter = 0;

        //    if (_previousItemWasBool && _previousNewSectionByteCount != ByteCounter)
        //    {
        //        if (ByteCounter % 2 == 0)
        //        {
        //            ByteCounter += 2;
        //        }
        //        else
        //        {
        //            ByteCounter += 1;
        //        }
        //    }

        //    _previousIterationWasNewSection = true;
        //    _previousNewSectionByteCount = ByteCounter;
        //}

        //public void Reset()
        //{
        //    ByteCounter = 0;
        //    BitCounter = 0;
        //    _previousByteCountValue = 0;
        //    _previousItemWasBool = false;
        //    _bytesRemaining = 0;
        //    _previousIterationWasNewSection = false;

        //}
    }
}
