using System.Collections.Generic;

namespace WpfApp2.Model
{
    public class ProjectItem 
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int DeviceType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int DeviceIndex { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<CanIndexItem> CanIndex { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<FormItem> Form { get; set; }

        public ProjectItem()
        {
            Form = new List<FormItem>();
            CanIndex = new List<CanIndexItem>();
        }

        public void Copy(ProjectItem newItem)
        {
            this.Name = newItem.Name;
            DeviceType = newItem.DeviceType;
            DeviceIndex = newItem.DeviceIndex;
            CanIndex = newItem.CanIndex;
            Form = newItem.Form;
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
