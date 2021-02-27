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
        private int _byteCouter;
        private int _previousByteCountValue;
        private bool _previousItemWasBool;

        public BitByteIndexControl()
        {
            ByteCounter = 0;
            BitCounter = 0;
            _previousByteCountValue = 0;
            _previousItemWasBool = false;
        }
        
        public int ByteCounter 
        { 
            get => _byteCouter;
            private set
            {
                _byteCouter = value;
            }
        }
        public int BitCounter { get; private set; }

        public void Update(int byteSize, bool isBoolType, bool isFirstItem)
        {
            if (isBoolType)
            {
                if (!isFirstItem/* && _previousByteCountValue == ByteCounter*/)
                {
                    BitCounter++;
                }

                if (BitCounter > Constants.LastBitInByte)
                {
                    BitCounter = 0;
                    ByteCounter++;
                }
            }
            else
            {
                if (_previousItemWasBool)
                {
                    AddedStructCorrection();
                }
                else ByteCounter += byteSize;
            }

            _previousItemWasBool = isBoolType;
            _previousByteCountValue = ByteCounter;
        }

        public void AddedStructCorrection()
        {
            if (ByteCounter == 31)
            {

            }


            BitCounter = 0;

            if (_previousItemWasBool)
            {
                if (ByteCounter % 2 == 0)
                {
                    ByteCounter += 2;
                }
                else ByteCounter += 1;
            }
        }
    }
}
