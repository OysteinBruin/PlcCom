using PlcComLibrary.Common;
using PlcComLibrary.Config;
using PlcComLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static PlcComLibrary.Common.Enums;
using log4net;




/*

    if (dataType == DataType::Bool) {

            if (m_sim.enabled) {
                m_boolValue = m_sim.simSignals.at(i)->randBool();
            }
            else {
                m_dataBlocks[dbNr].readSingleBit(dbOffset, m_plcVariables.at(i).bit(), m_boolValue);
            }
            m_data[index++] = static_cast<double>(m_boolValue);
        }
        else if (dataType == DataType::Real) {

            if (m_sim.enabled) {
                //if (i % 5 == 0) m_doubleValue = m_sim.simSignals.at(i)->sine();
                //else
                m_doubleValue = m_sim.simSignals.at(i)->randReal();
            }
            else {
                m_dataBlocks[dbNr].readSingleDouble(dbOffset, m_doubleValue);
            }
            m_data[index++] = m_doubleValue;
        }
    }




    

    SimulatedSignal::SimulatedSignal(int index, QObject *parent)
        : QObject(parent), m_index(index)
    {
        (index == 0) ? m_startValue =  0.5 : m_startValue = index;
        m_highLimit = static_cast<int>(m_startValue) * 2;
    }

    bool SimulatedSignal::randBool()
    {
        if (QTime::currentTime() > m_boolTogleTime) {
            int mSecs = QRandomGenerator::global()->bounded(10,10000);
            m_boolTogleTime = QTime::currentTime().addMSecs(mSecs);

            m_boolValue = !m_boolValue;
        }

        return  m_boolValue;
    }

    qreal SimulatedSignal::randReal()
    {
        int randomMode = QRandomGenerator::global()->bounded(1,100);

        if (m_isRampingPos || m_isRampingNeg) {
            if (QTime::currentTime() > m_rampDownTime) {
                m_rampDownTime = QTime::currentTime().addMSecs(10);
                if (m_isRampingPos) m_value -= 0.01;
                if (m_isRampingNeg) m_value += 0.01;
            }
        }
        else {
            if (QTime::currentTime() > m_skipChangingTime) {
                if (randomMode == Mode::Decr) {
                    m_value -= 0.1;
                }
                else if (randomMode == Mode::Inc) {
                    m_value += 0.1;
                }
            }
        }

        if (qAbs(m_value) > m_highLimit && !m_isRampingPos && !m_isRampingNeg) {
            m_rampDownTime = QTime::currentTime().addMSecs(100);
            if (m_value > 0) m_isRampingPos = true;
            else m_isRampingNeg = true;
        }

        if (qAbs(m_value) < m_startValue) {
            m_isRampingPos = false;
            m_isRampingNeg = false;
        }

        return m_value;
    }

    qreal SimulatedSignal::sine()
    {
        ++m_sineCounter;
        if (m_sineCounter > 1000) m_sineCounter = 0;
        double amplitude = 0.1 + m_index;
        double frequency = 1/amplitude;
        return qSin(m_sineCounter * frequency + M_PI) * amplitude / 2 + amplitude / 2;
    }

*/












namespace PlcComLibrary.PlcCom
{
    public class SimulatedPlcService : PlcService
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
  

        public SimulatedPlcService(int index, ICpuConfig config, List<IDatablockModel> datablocks)
            : base(index, config, datablocks)
        {
        }

        public string LastError { get; private set; }

        public override async Task ConnectAsync(ICpuConfig config)
        {
            Config = config;
            await ConnectAsync();
        }

        public override async Task ConnectAsync()
        {
            ComState = ComState.Connecting;
            await DelayAsync(1000);
            ComState = ComState.Connected;
        }

        public override void DisConnect()
        {
            ComState = ComState.DisConnected;
        }

        public override async Task ReadSingleAsync(string address)
        {
            await DelayAsync(500);
            VerifyConnectedAndValidateAddress(address);

            (int dbIndex, int signalIndex) = GetIndexFromAddress(address, Datablocks);

            if (dbIndex >= 0 && signalIndex >= 0)
            {
                Datablocks[dbIndex].Signals[signalIndex].Value = 0;
                PlcReadResultEventArgs args = new PlcReadResultEventArgs(new PlcComIndexModel(this.Index, dbIndex, signalIndex), 0.0f);
                RaiseHasNewData(args);
            }
            else
            {
                throw new Exception($"Plc Read Error - Unknown error occured while attempting to read from: {address}");
            }
        }

        protected override async Task ReadDbAsync(IDatablockModel db)
        {
            VerifyConnected();

            await DelayAsync(10);
        }



        public override async Task WriteSingleAsync(string address, object value)
        {
            VerifyConnected();
            await DelayAsync(10);

            (int dbIndex, int signalIndex) = GetIndexFromAddress(address, Datablocks);

            if (dbIndex >= 0 && signalIndex >= 0)
            {
                Datablocks[dbIndex].Signals[signalIndex].Value = (double)value;
                Console.WriteLine($"write value: {Datablocks[dbIndex].Signals[signalIndex].Value}");
                log.Debug($"write value: {value}");

                PlcReadResultEventArgs args = new PlcReadResultEventArgs(new PlcComIndexModel(this.Index, dbIndex, signalIndex), (double)value);
                RaiseHasNewData(args);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public override async Task PulseBitAsync(string address)
        {
            VerifyConnected();

            if (!AddressIsBoolType(address))
            {
                throw new Exception($"Plc Write Error - Attempting to pulse a non boolean signal: {address}");
            }

            (int dbIndex, int signalIndex) = GetIndexFromAddress(address, Datablocks);

            if (dbIndex >= 0 && signalIndex >= 0)
            {
                
                var writeHighArgs = new PlcReadResultEventArgs(new PlcComIndexModel(this.Index, dbIndex, signalIndex), 1.0f);
                await DelayAsync(100);
                Datablocks[dbIndex].Signals[signalIndex].Value = 1.0f;
                RaiseHasNewData(writeHighArgs);
                await DelayAsync(500);
                //args.Value = 0.0f;

                var writeLowArgs = new PlcReadResultEventArgs(new PlcComIndexModel(this.Index, dbIndex, signalIndex), 0.0f);
                Datablocks[dbIndex].Signals[signalIndex].Value = 0;
                RaiseHasNewData(writeLowArgs);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public override async Task ToggleBitAsync(string address)
        {
            VerifyConnected();
            if (!AddressIsBoolType(address))
            {
                throw new Exception($"Plc Write Error - Attempting to pulse a non boolean signal: {address}");
            }

            (int dbIndex, int signalIndex) = GetIndexFromAddress(address, Datablocks);

            if (dbIndex >= 0 && signalIndex >= 0)
            {
                await DelayAsync(100);
                if (Datablocks[dbIndex].Signals[signalIndex].Value > 0.0f)
                {
                   
                    var args = new PlcReadResultEventArgs(new PlcComIndexModel(this.Index, dbIndex, signalIndex), 0.0f);
                    Datablocks[dbIndex].Signals[signalIndex].Value = 0.0f;
                    RaiseHasNewData(args);
                }
                else
                {
                    var args = new PlcReadResultEventArgs(new PlcComIndexModel(this.Index, dbIndex, signalIndex), 1.0f);
                    Datablocks[dbIndex].Signals[signalIndex].Value = 1.0f;
                    RaiseHasNewData(args);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
