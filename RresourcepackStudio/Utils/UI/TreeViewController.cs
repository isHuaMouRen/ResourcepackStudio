using ResourcepackStudio.Classes;
using ResourcepackStudio.Classes.Configs;
using ResourcepackStudio.Controls.Icons;
using ResourcepackStudio.Windows;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ResourcepackStudio.Utils.UI
{
    public static class TreeViewController
    {
        public static TreeView TreeViewMain = ((WindowMain)Application.Current.MainWindow).treeView_Main;
            

        //快捷清除方法
        public static void Clear() => TreeViewMain.Items.Clear();





        /// <summary>
        /// 从目录加载树结构到TreeView
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>true成功，false失败</returns>
        public static bool LoadTree(string path,TreeViewItem rootNode)
        {
            try
            {
                if (!Directory.Exists(path))
                    return false;

                RecursionAddNode(path, rootNode);

                return true;
            }
            catch (Exception ex) 
            {
                ErrorReportDialog.Show(ex);
                return false;
            }


            //==========CUSTOM==========
            void RecursionAddNode(string path, TreeViewItem rootNode)
            {
                foreach (var folder in Directory.GetDirectories(path))
                {
                    string folderName = Path.GetFileName(folder);

                    var tvi = new TreeViewItem
                    {
                        Header = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Children =
                            {
                                new IconFolder(),
                                new TextBlock
                                {
                                    Text=folderName,
                                    Margin=new Thickness(5,0,0,0),
                                    VerticalAlignment=VerticalAlignment.Center
                                }
                            }
                        }
                    };

                    rootNode.Items.Add(tvi);

                    foreach (var chilFolder in Directory.GetDirectories(folder))
                    {
                        RecursionAddNode(chilFolder, tvi);
                    }
                        

                }
                foreach (var file in Directory.GetFiles(path))
                {
                    string fileName = Path.GetFileName(file);

                    var tvi = new TreeViewItem
                    {
                        Header = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Children =
                            {
                                new IconFile(),
                                new TextBlock
                                {
                                    Text=fileName,
                                    Margin=new Thickness(5,0,0,0),
                                    VerticalAlignment=VerticalAlignment.Center
                                }
                            }
                        }
                    };

                    rootNode.Items.Add(tvi);
                }
            }




        }

        /// <summary>
        /// 添加根节点
        /// </summary>
        /// <param name="project">项目信息</param>
        /// <returns>true成功，false失败</returns>
        public static bool AddRootNode(JsonProjectConfig.Index project)
        {
            try
            {
                TreeViewMain.Items.Add(new TreeViewItem
                {
                    Header = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Children =
                        {
                            new IconApplication(),
                            new TextBlock
                            {
                                Text=project.Name,
                                Margin=new Thickness(5,0,0,0),
                                VerticalAlignment=VerticalAlignment.Center
                            }
                        }
                    }
                });
                return true;
            }
            catch (Exception ex)
            {
                ErrorReportDialog.Show(ex);
                return false;
            }
        }
    }
}
