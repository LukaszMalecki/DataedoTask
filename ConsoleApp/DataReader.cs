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
    using ConsoleApp.ImportedObjects;

    public class DataReader
    {
        List<ImportedObject> importedObjects;
        List<Database> databases;
        List<Table> tables;
        List<Column> columns;

        private void ImportData(string fileToImport, char separator = ';', bool hasHeader = true)
        {
            importedObjects = new List<ImportedObject>();
            databases = new List<Database>();
            tables = new List<Table>();
            columns = new List<Column>();
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
                if (values.Length != Constants.ExpectedFieldCount)
                    continue;
                var importedObject = ImportedObjectFactory.GetImportedObject(values[1], values[0], values[2],
                    values[3], values[4], values[5], values[6]);
                if (importedObject == null)
                    continue;
                bool wasAdded = true;
                switch (importedObject)
                {
                    case Database database:
                        databases.Add(database);
                        break;
                    case Table table:
                        tables.Add(table);
                        break;
                    case Column column:
                        columns.Add(column);
                        break;
                    default:
                        wasAdded = false;
                        break;
                }
                if (wasAdded)
                    importedObjects.Add(importedObject);
            }
        }
        private void PrepareData()
        {
            // clear and correct imported data
            foreach (var importedObject in importedObjects)
            {
                importedObject.AdjustAttributes();
            }

            // assign number of children
            foreach(var database in databases)
            {
                foreach(var table in tables)
                {
                    if(database.IsParentOf(table))
                    {
                        database.AddChild(table);
                    }
                }
            }
            foreach (var table in tables)
            {
                foreach(var column in columns)
                {
                    if (table.IsParentOf(column))
                    {
                        table.AddChild(column);
                    }
                }
            }
        }
        public void PrintData()
        {
            foreach (var database in databases)
            {
                database.PrintData(shouldPrintChildren: true);
            }
        }
        public void ImportAndPrintData(string fileToImport, bool printData = true, char separator = ';', bool hasHeader = true)
        {
            ImportData(fileToImport, separator, hasHeader);
            PrepareData();
            if (printData)
            {
                PrintData();
            }
            Console.ReadLine();
        }
    }
}
