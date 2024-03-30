using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.ImportedObjects
{
    public class Table : ImportedObject, IChild
    {
        public string Schema { get; protected set; }

        public string ParentName { get; protected set; }
        public string ParentType
        {
            get; set;
        }
        public ImportedObject Parent { get; set; }
        public Table(string name, string type, string schema, string parentName, string parentType):base(name,type)
        {
            Schema = schema;
            ParentName = parentName;
            ParentType = parentType;
            Parent = null;
            //Children = new List<ImportedObject>();
        }
        public override void AdjustAttributes()
        {
            base.AdjustAttributes();
            Schema = AdjustedString(Schema);
            ParentName = AdjustedString(ParentName);
            ParentType = AdjustedString(ParentType, shouldUpper: true);
        }

        public override void PrintData(bool shouldPrintChildren = true)
        {
            Console.WriteLine($"\tTable '{Schema}.{Name}' ({NumberOfChildren} columns)");
            base.PrintData(shouldPrintChildren);
        }
    }
}
