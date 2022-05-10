using System.Collections.Generic;

namespace WpfApp2.Model
{
    //public class BaseProperty
    //{
    //    public string Name;
    //    public int Type;
    //    public string Description;
    //}

    public class Root
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ProjectItem> project { get; set; }

        public Root()
        {
            this.project = new List<ProjectItem>();
        }
    }

    //public class TreeViewItemNode : ICloneable
    //{
    //    public TreeViewItemNode()
    //    {
    //        this.Children = new List<TreeViewItemNode>();
    //    }

    //    public string Name { get; set; }

    //    public List<TreeViewItemNode> Children { get; set; }

    //    //public BaseProperty Self { get; set; }

    //    public object Clone()
    //    {
    //        return new TreeViewItemNode() { Name = this.Name, Self = this.Self };
    //    }
    //}

}
