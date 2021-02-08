using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace JsonReadWriteTest
{
    public class Data
    {
        public IList<Signal> Signals { get; set; }
    }
    public class JsonParser
    {
        public void Parse(string fileName)
        {
            //var output = new ICollection

            var item = new Data();

            using (StreamReader reader = new StreamReader(fileName))
            {
                string json = reader.ReadToEnd();
                item = JsonConvert.DeserializeObject<Data>(json);
            }

            item.Signals.Add(new Signal { Name = $"New @ {DateTime.Now}", Address = $"DB240.DBD{DateTime.Now.Second}", Description = "Genearated value 1"});
            item.Signals.Add(new Signal { Name = $"Another new @ {DateTime.Now}", Address = $"DB1630.DBD{DateTime.Now.Millisecond}", Description = "Genearated value 2" });
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                string json = JsonConvert.SerializeObject(item, Formatting.Indented);
                writer.WriteLine(json);
            }
        }
    }
}
