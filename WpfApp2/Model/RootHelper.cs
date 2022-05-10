using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;

namespace WpfApp2.Model
{
    public class RootHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Json路径</param>
        /// <param name="oldRoot">root</param>
        /// <returns></returns>
        public static Root InitRootByJson(string path, Root oldRoot = null)
        {
            string jsonStr;

            try
            {
                FileStream fs = null;
                StreamReader sr = null;

                try
                {
                    fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    sr = new StreamReader(fs, System.Text.Encoding.UTF8);

                    jsonStr = sr.ReadToEnd();
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    if (sr != null)
                    {
                        sr.Close();
                    }
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }

                if (oldRoot == null)
                {
                    oldRoot = JsonConvert.DeserializeObject<Root>(jsonStr);
                }
                else
                {
                    Root rImport = JsonConvert.DeserializeObject<Root>(jsonStr);
                    foreach (var project in rImport.project)
                    {
                        if (oldRoot.project.Find(x => x.Name == project.Name) == null)
                        {
                            oldRoot.project.Add(project);
                        }
                    }
                }

                //LoadTreeView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return oldRoot;
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
