using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.ImportedObjects
{
    public class Column:ImportedObject, IChild, IData
    {
        public string Schema { get; protected set; }

        public string ParentName { get; protected set; }
        public string ParentType
        {
            get; set;
        }
        public ImportedObject Parent { get; protected set; }

        public string DataType { get; protected set; }
        public bool IsNullable { get; protected set; }
        public Column(string name, string type, string schema, string parentName, string parentType,
            string dataType, bool isNullable) :base(name, type) 
        {
            Schema = schema;
            ParentName = parentName;
            ParentType = parentType;
            DataType = dataType;
            IsNullable = isNullable;
            Parent = null;
        }
        public Column(string name, string type, string schema, string parentName, string parentType,
            string dataType, string isNullable) : this(name, type, schema, parentName, parentType, 
                dataType, TranslateIsNullable(isNullable))
        {

        }

        protected static bool TranslateIsNullable(string isNullable)
        {
            if (String.IsNullOrEmpty(isNullable))
            {
                return false;
            }
            return isNullable == "1";
        }

        public override void AdjustAttributes()
        {
            base.AdjustAttributes();
            Schema = AdjustedString(Schema);
            ParentName = AdjustedString(ParentName);
            ParentType = AdjustedString(ParentType, shouldUpper: true);
        }
        public void AddParent(ImportedObject parent, bool isCalledFirst = true)
        {
            if(!isCalledFirst)
            {
                Parent = parent;
            }
            if(Parent == parent)
                return;
            Parent = parent;
            parent.AddChild(this, false);
        }
        public override void PrintData(bool shouldPrintChildren = true)
        {
            Console.WriteLine($"\t\tColumn '{Name}' with {DataType} data type {(IsNullable ? "accepts nulls" : "with no nulls")}");
            base.PrintData(shouldPrintChildren);
        }
    }
}
