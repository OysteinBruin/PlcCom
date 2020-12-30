using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public interface ISignalModel
    {
        int Index { get; set; }
        string Address { get; set; }
        int Db { get; set; }
        int Byte { get; set; }
        int Bit { get; set; }
        string DataTypeStr { get; set; }
        string Description { get; set; }
        string Name { get; set; }
        double Value { get; set; }

        bool IsValid { get; }
    }

    public class SignalModel : ISignalModel
    {
        public SignalModel()
        {
        }
        public int Index { get; set; }
        public int Db { get; set; }
        public int Byte { get; set; }
        public int Bit { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DataTypeStr { get; set; }
        public string Address { get; set; }

        public double Value { get; set; }

        public bool IsValid
        {
            get
            {
                return (Db > 0 && Db < 10000 && Name.Length > 0 && Address.Length > 7);
            }
        }
    }

    public interface IDatablockModel
    {
        string Name { get; set; }
        int Index { get; set; }
        int Number { get; set; }

        int FirstByte { get; set; }
        int ByteCount { get; set; }

        List<ISignalModel> Signals { get; set; }
    }

    public class DatablockModel //: IDatablockModel
    {
        public DatablockModel()
        {
            Index = -1;
            Signals = new List<SignalModel>();
            Name = String.Empty;
            Number = -1;
        }
        public DatablockModel(int index, List<SignalModel> signals, string name = "", int number = -1)
        {
            Index = index;
            Signals = signals;
            Name = name;
            Number = number;
        }
        public int Index { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        //public IJsonFileConfig Config { get; set; }

        //public List<ISignalModel> Signals { get; set; }
        public List<SignalModel> Signals { get; set; }

        public bool IsValid
        {
            get
            {
                return Index >= 0 && Signals.Count > 0 &&
                    Name.Length > 0 && Number > 0;
            }

        }
        public int FirstByte { get; set; }
        public int ByteCount { get; set; }

    }

    public class SignalGenerator
    {
        //private List<ISignalModel> _signals1 = new List<ISignalModel> {
        //     new SignalModel{ Index = 0}
        //};


        // public static List<ISignalModel> Signals(int index)
        public static IList<SignalModel> Signals(int index)
        {
            //List<ISignalModel> signals = new List<ISignalModel>();
            IList<SignalModel> signals = new List<SignalModel>();

            if (index == 0)
            {
                for (int i = 50; i < 70; i++)
                {
                    signals.Add(new SignalModel { Index = i, Address = $"DB{i + 1}", Name = $"Signal {i}" });
                }
            }
            else
            {
                for (int i = 1000; i < 1010; i++)
                {
                    signals.Add(new SignalModel { Index = i, Address = $"DB{i + 1}", Name = $"Signal {i}" });
                }
            }


            return signals;
        }
    }



    class Program
    {
        public static void LoadConfigs()
        {
            //List<ISignalModel> signals = new List<ISignalModel>();
            //List<IDatablockModel> datablocks = new List<IDatablockModel>();
            List<SignalModel> signals = new List<SignalModel>();
            List<DatablockModel> datablocks = new List<DatablockModel>();

            for (int i = 0; i < 3; i++)
            {
                datablocks.Clear();

                for (int j = 0; j < 2; j++)
                {
                    //IDatablockModel datablock = new DatablockModel();
                    DatablockModel datablock = new DatablockModel();
                    signals.Clear();



                    signals = SignalGenerator.Signals(j);
                    //Console.WriteLine($"\n\nAfter _dbParser.ParseDb filePath  signals: {signals.Count}");
                    //foreach (var signal in signals)
                    //{
                    //    Console.WriteLine($"\tsignal {signal.Name} address: {signal.Address}");
                    //}
                    //Console.WriteLine("\n\n");


                    datablock.Index = signals.Count;
                    datablock.Signals = signals;
                    datablock.Name = $"Datablock {j}";
                    datablock.Number = j + 3050;
                    datablocks.Add(datablock);
                }

                foreach (var db in datablocks)
                {
                    Console.WriteLine($"\n\n\n-----Cpu {i}------------Datablock {db.Index} [{db.Name}]------------------------------");
                    foreach (var signal in db.Signals)
                    {
                        Console.WriteLine($"\tSignal {signal.Name}");
                    }
                }
            }
        }
        static void Main(string[] args)
        {
            Program.LoadConfigs();
            Console.ReadLine();
        }
    }
}



//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PlcComUI
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Text;
//    using System.Threading.Tasks;

//    namespace ConsoleApp1
//    {
//        public interface ISignalModel
//        {
//            int Index { get; set; }
//            string Address { get; set; }
//            int Db { get; set; }
//            int Byte { get; set; }
//            int Bit { get; set; }
//            string DataTypeStr { get; set; }
//            string Description { get; set; }
//            string Name { get; set; }
//            double Value { get; set; }

//            bool IsValid { get; }
//        }

//        public class SignalModel : ISignalModel
//        {
//            public SignalModel()
//            {
//            }
//            public int Index { get; set; }
//            public int Db { get; set; }
//            public int Byte { get; set; }
//            public int Bit { get; set; }
//            public string Name { get; set; }
//            public string Description { get; set; }
//            public string DataTypeStr { get; set; }
//            public string Address { get; set; }

//            public double Value { get; set; }

//            public bool IsValid
//            {
//                get
//                {
//                    return (Db > 0 && Db < 10000 && Name.Length > 0 && Address.Length > 7);
//                }
//            }
//        }

//        public interface IDatablockModel
//        {
//            string Name { get; set; }
//            int Index { get; set; }
//            int Number { get; set; }

//            int FirstByte { get; set; }
//            int ByteCount { get; set; }

//            List<ISignalModel> Signals { get; set; }
//        }

//        public class DatablockModel : IDatablockModel
//        {
//            public DatablockModel()
//            {
//                Index = -1;
//                Signals = new List<ISignalModel>();
//                Name = String.Empty;
//                Number = -1;
//            }
//            public DatablockModel(int index, List<ISignalModel> signals, string name = "", int number = -1)
//            {
//                Index = index;
//                Signals = signals;
//                Name = name;
//                Number = number;
//            }
//            public int Index { get; set; }
//            public string Name { get; set; }
//            public int Number { get; set; }
//            //public IJsonFileConfig Config { get; set; }

//            public List<ISignalModel> Signals { get; set; }

//            public bool IsValid
//            {
//                get
//                {
//                    return Index >= 0 && Signals.Count > 0 &&
//                        Name.Length > 0 && Number > 0;
//                }

//            }
//            public int FirstByte { get; set; }
//            public int ByteCount { get; set; }

//        }

//        public class SignalGenerator
//        {
//            //private List<ISignalModel> _signals1 = new List<ISignalModel> {
//            //     new SignalModel{ Index = 0}
//            //};


//            public static List<ISignalModel> Signals(int index)
//            {
//                //List<ISignalModel> signals = new List<ISignalModel>();
//                List<ISignalModel> signals = new List<ISignalModel>();

//                if (index == 0)
//                {
//                    for (int i = 0; i < 20; i++)
//                    {
//                        signals.Add(new SignalModel { Index = i, Address = "DB + i+1", Name = "Signal + i" });
//                    }
//                }
//                else
//                {
//                    for (int i = 1000; i < 1010; i++)
//                    {
//                        signals.Add(new SignalModel { Index = i, Address = "DB + i+1", Name = "Signal + i" });
//                    }
//                }


//                return signals;
//            }
//        }



//        class Program
//        {
//            public static void LoadConfigs()
//            {
//                List<ISignalModel> signals = new List<ISignalModel>();
//                List<IDatablockModel> datablocks = new List<IDatablockModel>();

//                for (int i = 0; i < 1; i++)
//                {


//                    for (int j = 0; j < 2; j++)
//                    {
//                        IDatablockModel datablock = new DatablockModel();
//                        signals.Clear();



//                        signals = SignalGenerator.Signals(j);
//                        Console.WriteLine($"\n\nAfter _dbParser.ParseDb filePath  signals: {signals.Count}");
//                        foreach (var signal in signals)
//                        {
//                            Console.WriteLine($"\tsignal {signal.Name} address: {signal.Address}");
//                        }
//                        Console.WriteLine("\n\n");


//                        datablock.Index = signals.Count;
//                        datablock.Signals = signals;
//                        datablock.Name = "Datablock + j";
//                        datablock.Number = j + 3050;
//                        datablocks.Add(datablock);
//                    }

//                    foreach (var db in datablocks)
//                    {
//                        Console.WriteLine($"\n\n\n-----------------Datablock {db.Index} [{db.Name}]------------------------------");
//                        foreach (var signal in db.Signals)
//                        {
//                            Console.WriteLine($"\tSignal {signal.Name}");
//                        }
//                    }
//                }
//            }
//            static void Main(string[] args)
//            {
//                Program.LoadConfigs();
//                Console.ReadLine();
//            }
//        }
//    }
//}
