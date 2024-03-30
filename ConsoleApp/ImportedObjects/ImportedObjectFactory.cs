using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.ImportedObjects
{
    public static class ImportedObjectFactory
    {
        public static ImportedObject GetImportedObject(string name, string type, string schema, string parentName, string parentType,
            string dataType, string isNullable)
        {
            var adjustedType = ImportedObject.AdjustedString(type, shouldUpper: true);
            switch (adjustedType)
            {
                case Constants.DefaultColumnType:
                    return new Column(name, type, schema, parentName, parentType, dataType, isNullable);
                case Constants.DefaultTableType:
                    return new Table(name, type, schema, parentName, parentType);
                case Constants.DefaultDatabaseType:
                    return new Database(name, type);
                case Constants.DefaultMissingType:
                case null: 
                    return null;
                default:
                    throw new ArgumentException($"Unknown object type {adjustedType}, new class may be needed");
            }
        }
    }
}
