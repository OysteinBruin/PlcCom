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
        private int _previousByteSize;
        private bool _previousItemWasBool;
        private int _bytesRemaining = 0;
        private int _previousNewSectionByteCount = 0;
        private bool _previousIterationWasNewSection = false;

        public BitByteIndexControl()
        {
            Reset();
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

        public void Update(int byteSize, bool isBoolType)
        {
            if (isBoolType)
            {
                if (!_previousIterationWasNewSection && _previousItemWasBool/* && _previousByteCountValue == ByteCounter*/)
                {
                    BitCounter++;
                }

                if (BitCounter > Constants.LastBitInByte)
                {
                    BitCounter = 0;
                    ByteCounter++;
                }

                if (!_previousItemWasBool)
                {
                    ByteCounter += _previousByteSize;
                }
            }
            else
            {
                if (_previousItemWasBool)
                {
                    BitCounter = 0;

                    if (!_previousIterationWasNewSection)
                    {
                        if (ByteCounter % 2 == 0)
                        {
                            ByteCounter += 2;
                            _bytesRemaining = byteSize - 2;

                            //_bytesRemaining = byteSize;
                        }
                        else
                        {
                            ByteCounter += 1;
                            _bytesRemaining = byteSize - 1;

                            //_bytesRemaining = byteSize + 1;
                        }
                    }
                }
                else
                {
                    if (!_previousIterationWasNewSection)
                    {
                        if (byteSize > 2)
                        {
                            ByteCounter += 2;
                        }
                        else ByteCounter += byteSize;

                        ByteCounter += _bytesRemaining;
                        _bytesRemaining = 0;

                        //ByteCounter += _bytesRemaining;
                        //_bytesRemaining = byteSize;
                    }
                }
            }

            _previousByteSize = byteSize;
            _previousItemWasBool = isBoolType;
            _previousByteCountValue = ByteCounter;
            _previousIterationWasNewSection = false;
        }

        public void NewSectionCorrection()
        {
            BitCounter = 0;

            if (_previousItemWasBool && _previousNewSectionByteCount != ByteCounter)
            {
                if (ByteCounter % 2 == 0)
                {
                    ByteCounter += 2;
                }
                else
                {
                    ByteCounter += 1;
                }
            }

            _previousIterationWasNewSection = true;
            _previousNewSectionByteCount = ByteCounter;
        }

        public void Reset()
        {
            ByteCounter = 0;
            BitCounter = 0;
            _previousByteCountValue = 0;
            _previousItemWasBool = false;
            _bytesRemaining = 0;
            _previousIterationWasNewSection = false;

        }
    }
}
