using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.PlcCom
{
    /*
    include <QObject>
    #include <QTime>

    class SimulatedSignal : public QObject
    {
        Q_OBJECT
    public:
        SimulatedSignal(int index, QObject *parent = nullptr);

    public:
        qreal randReal();
        qreal sine();
        bool randBool();

    private:
        int m_index {-1};
        qreal m_value {0.0};
        qreal m_startValue;
        int m_highLimit {3};
        bool m_isRampingPos {false};
        bool m_isRampingNeg {false};
        //bool m_isRampingDown {false};
        //bool m_wasHighPos {false};
        //bool m_wasLow {false};
        QTime m_rampDownTime;
        QTime m_boolTogleTime;
        QTime m_skipChangingTime;
        bool m_boolValue {false};

        int m_sineCounter{0};

        enum Mode {
            Decr = 20,
            Inc = 40,
            Wait = 94
        } m_mode;

    };



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
    public class SimulatedSignal
    {
        int _index;
        private readonly Random _random = new Random();
        private DateTime _nextBooleanToggleTime = new DateTime();

        private bool _booleanValue = false;
        private float _floatValue = 0;
        private float _startValue = 0.0f;
        private int _highLimit = 0;
        private int _lowLimit = 0;
        private DateTime modeChangeWaitTime;
        private int _sineCounter = 0;
        int _randIncDecWaitValue;





        public SimulatedSignal(int index)
        {
            _index = index;
            

            _highLimit = index + 1 * _random.Next(1, 100);
            _lowLimit =  _random.Next(_highLimit - 100, _highLimit - 50);
            _startValue =  (_highLimit + _lowLimit)/2;
            _randIncDecWaitValue = _random.Next(1, 100);
        }


        public bool RandomBool()
        {
            if (DateTime.Now > _nextBooleanToggleTime)
            {
                int mSecs = _random.Next(100, 3000);
                _nextBooleanToggleTime = DateTime.Now.AddMilliseconds(mSecs);

                _booleanValue = !_booleanValue;
            }

            return _booleanValue;
        }

        public float RandomFloat()
        {
            if (Math.Abs(_floatValue) >= _highLimit)
            {
                _randIncDecWaitValue = 0;
                modeChangeWaitTime = DateTime.Now.AddMilliseconds(_random.Next(800, 3000));
            }
            else if (Math.Abs(_floatValue) <= _lowLimit)
            {
                _randIncDecWaitValue = 100;
                modeChangeWaitTime = DateTime.Now.AddMilliseconds(_random.Next(800, 3000));
            }

            if (_randIncDecWaitValue < 33)
            {
                _floatValue -= 0.001f * _random.Next(1, 100);
            }
            else if (_randIncDecWaitValue > 66)
            {
                _floatValue += 0.001f * _random.Next(1, 100);
            }

            if (DateTime.Now > modeChangeWaitTime)
            {
                modeChangeWaitTime = DateTime.Now.AddMilliseconds(_random.Next(10, 3000));
                _randIncDecWaitValue = _random.Next(1, 100);

            }


            //if (_isRampingPosDir || _isRampingNegDir)
            //{
            //    if (DateTime.Now > _rampDownTime)
            //    {
            //        _rampDownTime = DateTime.Now.AddMilliseconds(10);
            //        if (_isRampingPosDir) _floatValue -= 0.01f;
            //        if (_isRampingNegDir) _floatValue += 0.01f;
            //    }
            //}
            //else
            //{
            //    if (DateTime.Now > modeChangeWaitTime)
            //    {
            //        if (randomMode == _mode.Decrease)
            //        {
            //            _floatValue -= 0.1f;
            //        }
            //        else if (randomMode == _mode.Increase)
            //        {
            //            _floatValue += 0.1f;
            //        }
            //    }
            //}

            

            //if (Math.Abs(_floatValue) < _startValue)
            //{
            //    _isRampingPosDir = false;
            //    _isRampingNegDir = false;
            //}

            return _floatValue;
        }

        public double Sine()
        {
            ++_sineCounter;
            if (_sineCounter > 1000) _sineCounter = 0;
            double amplitude = 0.1 + _index*20;
            double frequency = 1 / amplitude;
            return Math.Sin(_sineCounter * frequency + Math.PI) * amplitude / 2 + amplitude / 2;
        }
    }
}
