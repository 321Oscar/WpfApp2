using System;

namespace WpfApp2.Model
{
    public class FormItem : IComparable<FormItem>
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int FormType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CanChannel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Singals Singals { get; set; }

        public int CompareTo(FormItem other)
        {
            return this.FormType.CompareTo(other.FormType);
        }

        public void Copy(FormItem newItem)
        {
            this.Name = newItem.Name;
            FormType = newItem.FormType;
            CanChannel = newItem.CanChannel;
            Singals = newItem.Singals;
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
