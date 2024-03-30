using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.ImportedObjects
{
    public class Database:ImportedObject
    {
        public Database(string name, string type):base(name, type) 
        {

            //Children = new List<ImportedObject>();
        }

        public override void PrintData(bool shouldPrintChildren = true)
        {
            Console.WriteLine($"Database '{Name}' ({NumberOfChildren} tables)");
            base.PrintData(shouldPrintChildren);
        }
    }
}
