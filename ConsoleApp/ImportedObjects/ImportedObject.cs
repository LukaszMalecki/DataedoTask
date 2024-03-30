using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.ImportedObjects
{
    public abstract class ImportedObject
    {
        public string Name { get; protected set; }
        //consider refactoring Type variable to be omitted and only have ParentType as a Type variable for better performance
        public string Type { get; protected set; }
        public List<ImportedObject> Children { get; protected set; } = new List<ImportedObject>();

        public ImportedObject(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public int NumberOfChildren
        {
            get => Children?.Count ?? 0;
        }
        public static string AdjustedString(string toAdjust, bool shouldUpper = false)
        {
            toAdjust = toAdjust.Replace(" ", "").Replace(Environment.NewLine, "");
            if (shouldUpper)
                toAdjust = toAdjust.ToUpper();
            return toAdjust;
        }
        public virtual void AdjustAttributes()
        {
            Type = AdjustedString(Type, shouldUpper: true);
            Name = AdjustedString(Name);
        }
        public bool IsParentOf<T>(T child) where T : ImportedObject, IChild
        {
            if (child.Parent == this)
                return true;
            return child.ParentType.Equals(this.Type) && child.ParentName.Equals(this.Name);
        }
        public void AddChild<T>(T child) where T : ImportedObject, IChild
        {
            if (child == null)
                return;
            if (child.Parent == this)
                return;
            Children.Add(child);
            child.Parent = this;
        }
        public void RemoveChild<T>(T child) where T : ImportedObject, IChild
        {
            if (child == null)
                return;
            if (Children.Remove(child))
                child.Parent = null;
        }

        public virtual void PrintData(bool shouldPrintChildren = true)
        {
            if (!shouldPrintChildren)
                return;
            if (NumberOfChildren == 0)
                return;
            foreach (var child in Children)
            {
                child.PrintData(true);
            }
        }
    }
}
