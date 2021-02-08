using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonReadWriteTest
{
    class Program
    {
        static void Main(string[] args)
        {
            JsonParser parser = new JsonParser();

            parser.Parse("test.json");
        }
    }
}
