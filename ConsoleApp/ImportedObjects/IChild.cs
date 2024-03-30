using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.ImportedObjects
{
    public interface IChild
    {
        string Schema { get; }

        string ParentName { get; }
        string ParentType
        {
            get; set;
        }
        ImportedObject Parent { get; set; }
    }
}
