using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole {
    class Program {
        static void Main(string[] args) {
            var list = new Person[] {
                new Person { Name = "Bob", DOB = new DateTime(1980, 1, 1)},
                new Person { Name = "Bob", DOB = new DateTime(1980, 1, 1)}

            };

            Console.WriteLine(list);

        }

        public class Person {
            public string Name { get; set; }
            public DateTime DOB { get; set; }
        }
    }
}
