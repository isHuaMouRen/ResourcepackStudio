using Notifications.Wpf;
using RresourcepackStudio.Classes;
using RresourcepackStudio.Classes.Configs;
using RresourcepackStudio.Controls.Icons;
using RresourcepackStudio.Windows;
using System.Windows;
using System.Windows.Controls;

namespace RresourcepackStudio.Utils.UI
{
    public static class FileManager
    {
        /// <summary>
        /// 从全局当前项目加载TreeView
        /// </summary>
        /// <param name="treeView">要加载到的TreeView</param>
        public static void LoadTreeView(TreeView treeView)
        {
            treeView.Items.Clear();

            var headerItem = new TreeViewItem
            {
                Header = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                    {
                        new IconApplication(),
                        new TextBlock
                        {
                            Text=$"{Globals.CurrentProject!.Name}",
                            Margin=new Thickness(5,0,0,0),
                            VerticalAlignment=VerticalAlignment.Center
                        }
                    }
                }
            };
            treeView.Items.Add(headerItem);


        }

        /// <summary>
        /// 创建文件，带无效/同名检测，无需手动检测
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="rootItem">要创建的对象</param>
        public static void NewFile(string fileName, TreeViewItem rootItem)
        {
            
        }

        /// <summary>
        /// 创建文件夹，带无效/同名检测，无需手动检测
        /// </summary>
        /// <param name="fileName">文件夹名</param>
        /// <param name="rootItem">要创建的对象</param>
        public static void NewFolder(string folderName, TreeViewItem rootItem)
        {
            
        }

        /// <summary>
        /// 重命名项，带无效/同名检测
        /// </summary>
        /// <param name="newName">新名字</param>
        /// <param name="targetItem">目标名字</param>
        public static void RenameItem(string newName,TreeViewItem targetItem)
        {

        }

        /// <summary>
        /// 删除项
        /// </summary>
        /// <param name="targetItem">目标项</param>
        public static void DeleteItem(TreeViewItem targetItem)
        {
            
        }
    }
}
