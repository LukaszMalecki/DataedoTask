namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Data.Common;

    public class DataReader
    {
        List<ImportedObject> ImportedObjects;
        List<ImportedObject> Databases;

        private void ImportData(string fileToImport, char separator = ';', bool hasHeader = true)
        {
            ImportedObjects = new List<ImportedObject>();
            Databases = new List<ImportedObject>();
            var streamReader = new StreamReader(fileToImport);

            var importedLines = new List<string>();
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();
                importedLines.Add(line);
            }

            int startingIndex = 0;
            if (hasHeader)
                startingIndex++;

            for (int i = startingIndex; i < importedLines.Count; i++)
            {
                var importedLine = importedLines[i];
                var values = importedLine.Split(separator);
                if (values.Length != ImportedObject.ExpectedFieldCount)
                    continue;
                var importedObject = new ImportedObject(values[0], values[1], values[2],
                    values[3], values[4], values[5], values[6]);
                ImportedObjects.Add(importedObject);
            }
        }
        private void PrepareData()
        {
            // clear and correct imported data
            foreach (var importedObject in ImportedObjects)
            {
                importedObject.AdjustAttributes();
                if (importedObject.IsDatabase())
                    Databases.Add(importedObject);
            }

            // assign number of children
            foreach (var importedObject in ImportedObjects)
            {
                foreach (var childObj in ImportedObjects)
                {
                    if (childObj.ParentType.Equals(importedObject.Type) && childObj.ParentName.Equals(importedObject.Name))
                    {
                        importedObject.AddChild(childObj);
                    }
                }
            }
        }
        public void PrintData()
        {
            foreach (var database in Databases)
            {
                database.PrintData(shouldPrintChildren: true);
            }
        }
        public void ImportAndPrintData(string fileToImport, bool printData = true, char separator = ';', bool hasHeader = true)
        {
            ImportData(fileToImport, separator, hasHeader);
            PrepareData();
            if(printData) 
            {
                PrintData();
            }
            Console.ReadLine();
        }
    }

    class ImportedObject : ImportedObjectBaseClass
    {
        public const int ExpectedFieldCount = 7;
        public const int DefaultNumberOfChildren = 0;
        public const string DefaultDatabaseType = "DATABASE";

        public string Schema { get; private set; }

        public string ParentName { get; private set; }
        public string ParentType
        {
            get; set;
        }

        public string DataType { get; private set; }
        public bool? IsNullable { get; private set; }

        public int NumberOfChildren { get; private set; }

        public ImportedObject Parent { get; private set; } = null;

        public List<ImportedObject> Children { get; private set; }

        public DatabaseLevel DatabaseLevel { get; private set; } = DatabaseLevel.None;

        public ImportedObject(string name, string type, string schema, string parentName, string parentType, 
            string dataType, string isNullable, int numberOfChildren=DefaultNumberOfChildren) :base(type, name)
        {
            Schema = schema;
            ParentName = parentName;
            ParentType = parentType;
            DataType = dataType;
            SetIsNullable(isNullable);
            NumberOfChildren = numberOfChildren;
            Children = new List<ImportedObject>();
        }

        private void SetIsNullable(string isNullable)
        {
            if (String.IsNullOrEmpty(isNullable))
            {
                IsNullable = null;
                return;
            }
            IsNullable = isNullable == "1" ? true : false;
        }

        public void AdjustAttributes()
        {
            Type = AdjustedString(Type, shouldUpper: true);
            Name = AdjustedString(Name);
            Schema = AdjustedString(Schema);
            ParentName = AdjustedString(ParentName);
            ParentType = AdjustedString(ParentType, shouldUpper: true);

            if(IsDatabase())
            {
                SetDatabaseLevel(DatabaseLevel.Database);
            }
        }

        public string AdjustedString(string toAdjust, bool shouldUpper = false)
        {
            toAdjust = toAdjust.Replace(" ", "").Replace(Environment.NewLine, "");
            if(shouldUpper)
                toAdjust = toAdjust.ToUpper();
            return toAdjust;
        }
        public bool IsDatabase()
        {
            return  DatabaseLevel == DatabaseLevel.Database || Type.Equals(DefaultDatabaseType);
        }
        public void AddChild(ImportedObject child)
        {
            Children.Add(child);
            NumberOfChildren++;
            child.AddParent(this);
        }
        public void AddParent(ImportedObject parent)
        {
            Parent = parent;
            SetDatabaseLevel(parent.DatabaseLevel.Child());
        }

        public void SetDatabaseLevel(DatabaseLevel databaseLevel)
        {

            this.DatabaseLevel = databaseLevel;

            foreach(var child in Children)
            {
                child.SetDatabaseLevel(databaseLevel.Child());
            }
        }

        public void PrintData(bool shouldPrintChildren=true)
        {
            switch (DatabaseLevel)
            {
                case DatabaseLevel.None:
                    break;
                case DatabaseLevel.Database:
                    Console.WriteLine($"Database '{Name}' ({NumberOfChildren} tables)");
                    break;
                case DatabaseLevel.Table:
                    Console.WriteLine($"\tTable '{Schema}.{Name}' ({NumberOfChildren} columns)");
                    break;
                case DatabaseLevel.Row:
                    Console.WriteLine($"\t\tColumn '{Name}' with {DataType} data type {(IsNullable.Value ? "accepts nulls" : "with no nulls")}");
                    break;
                default:
                    break;
            }
            if (!shouldPrintChildren)
                return;

            foreach(var child in Children)
            {
                child.PrintData(true);
            }
        }



    }
    public enum DatabaseLevel
    {
        None = 0,
        Database,
        Table,
        Row
    }

    public static class DatabaseLevelExtensions
    {
        public static DatabaseLevel Parent(this DatabaseLevel databaseLevel)
        {
            if (databaseLevel == DatabaseLevel.None)
                return DatabaseLevel.None;
            return databaseLevel - 1;
        }
        public static DatabaseLevel Child(this DatabaseLevel databaseLevel)
        {
            if (databaseLevel == DatabaseLevel.None)
                return DatabaseLevel.None;
            return databaseLevel + 1;
        }
    }


    class ImportedObjectBaseClass
    {
        public string Name { get; protected set; }
        public string Type { get; protected set; }

        public ImportedObjectBaseClass(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}
