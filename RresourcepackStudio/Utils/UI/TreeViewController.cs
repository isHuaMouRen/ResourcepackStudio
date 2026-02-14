using ModernWpf.Controls;
using Notifications.Wpf;
using ResourcepackStudio.Classes;
using ResourcepackStudio.Classes.Configs;
using ResourcepackStudio.Controls.Icons;
using ResourcepackStudio.Windows;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ResourcepackStudio.Utils.UI
{
    public enum TreeItemType
    {
        File,
        Folder,
        Root
    }

    public class TreeItemInfo
    {
        public string Name { get; set; } = "Name";
        public TreeItemType Type { get; set; } = TreeItemType.File;
    }



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
        public static bool LoadTree(string path, TreeViewItem rootNode)
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
            void RecursionAddNode(string currentPath, TreeViewItem parentNode)
            {
                var mainWindow = (WindowMain)Application.Current.MainWindow;

                foreach (var dirPath in Directory.GetDirectories(currentPath))
                {
                    string folderName = Path.GetFileName(dirPath);

                    var folderItem = new TreeViewItem
                    {
                        Header = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Children =
                            {
                                new IconFolder(),
                                new TextBlock
                                {
                                    Text = folderName,
                                    Margin = new Thickness(5, 0, 0, 0),
                                    VerticalAlignment = VerticalAlignment.Center
                                }
                            }
                        },
                        Tag = new TreeItemInfo
                        {
                            Name = folderName,
                            Type = TreeItemType.Folder
                        },
                        ContextMenu = mainWindow.treeViewItemContextMenu
                    };
                    folderItem.PreviewMouseRightButtonDown += ((s, e) => folderItem.IsSelected = true);

                    parentNode.Items.Add(folderItem);

                    RecursionAddNode(dirPath, folderItem);
                }

                foreach (var filePath in Directory.GetFiles(currentPath))
                {
                    string fileName = Path.GetFileName(filePath);

                    var fileItem = new TreeViewItem
                    {
                        Header = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Children =
                            {
                                new IconFile(),
                                new TextBlock
                                {
                                    Text = fileName,
                                   Margin = new Thickness(5, 0, 0, 0),
                                    VerticalAlignment = VerticalAlignment.Center
                                }
                            }
                        },
                        Tag = new TreeItemInfo
                        {
                            Name = fileName,
                            Type = TreeItemType.File
                        },
                        ContextMenu = mainWindow.treeViewItemContextMenu
                    };
                    fileItem.PreviewMouseRightButtonDown += ((s, e) => fileItem.IsSelected = true);

                    parentNode.Items.Add(fileItem);
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
                var mainWindow = (WindowMain)Application.Current.MainWindow;


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
                    },
                    Tag = new TreeItemInfo
                    {
                        Name = project.Name,
                        Type = TreeItemType.Root
                    },
                    ContextMenu = mainWindow.treeViewRootItemContextMenu
                });
                return true;
            }
            catch (Exception ex)
            {
                ErrorReportDialog.Show(ex);
                return false;
            }
        }


        /// <summary>
        /// 新建文件
        /// </summary>
        /// <param name="rootNode">创建节点</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static bool NewFile(TreeViewItem rootNode, string fileName)
        {
            try
            {
                var mainWindow = (WindowMain)Application.Current.MainWindow;


                var newFile = new TreeViewItem
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
                    },
                    Tag = new TreeItemInfo
                    {
                        Name = fileName,
                        Type = TreeItemType.File
                    },
                    ContextMenu = mainWindow.treeViewItemContextMenu
                };

                string? filePath = null;
                if (((TreeItemInfo)rootNode.Tag).Type == TreeItemType.Folder)
                {
                    rootNode.Items.Add(newFile);

                    filePath = Path.Combine(Globals.CurrentProjectDirectory!, "files", GetItemPath(rootNode), fileName);
                }
                else if (((TreeItemInfo)rootNode.Tag).Type == TreeItemType.File)
                {
                    ((TreeViewItem)rootNode.Parent).Items.Add(newFile);

                    filePath = Path.Combine(Globals.CurrentProjectDirectory!, "files", Path.GetDirectoryName(GetItemPath(rootNode))!, fileName);
                }

                if (string.IsNullOrEmpty(filePath))
                    return false;


                if (File.Exists(filePath))
                {
                    new NotificationManager().Show(new NotificationContent
                    {
                        Title = "新文件",
                        Message = $"\"{filePath}\" 已有同名文件！",
                        Type = NotificationType.Error
                    });
                    return false;
                }

                File.Create(filePath);



                return true;
            }
            catch (Exception ex)
            {
                ErrorReportDialog.Show(ex);
                return false;
            }
        }

        public static async void NewFileEx()
        {
            try
            {
                bool isCreate = false;
                var tb = new TextBox
                {
                    Text = "新文件"
                };
                await DialogManager.ShowDialogAsync(new ContentDialog
                {
                    Title = "新文件",
                    Content = tb,
                    PrimaryButtonText = "创建",
                    CloseButtonText = "取消",
                    DefaultButton = ContentDialogButton.Primary
                }, (() => isCreate = true));

                if (!isCreate)
                    return;

                if (!CharChecker.Check(tb.Text))
                {
                    new NotificationManager().Show(new NotificationContent
                    {
                        Title = "新文件",
                        Message = $"\"{tb.Text}\" 包含无效字符",
                        Type = NotificationType.Error
                    });
                    return;
                }

                var result = NewFile((TreeViewItem)TreeViewMain.SelectedItem, tb.Text);

                if (!result)
                    new NotificationManager().Show(new NotificationContent
                    {
                        Title = "新文件",
                        Message = "无法创建文件",
                        Type = NotificationType.Error
                    });
            }
            catch (Exception ex)
            {
                ErrorReportDialog.Show(ex);
            }
        }






        public static string GetItemPath(this TreeViewItem item)
        {
            var pathParts = new List<string>();
            var current = item;

            while (current != null)
            {
                string headerText = ((TreeItemInfo)current.Tag).Name;
                if (!string.IsNullOrEmpty(headerText) && ((TreeItemInfo)current.Tag).Type != TreeItemType.Root)
                {
                    pathParts.Add(headerText);
                }

                var parent = ItemsControl.ItemsControlFromItemContainer(current) as ItemsControl;
                current = parent as TreeViewItem;
            }

            pathParts.Reverse();
            return Path.Combine(pathParts.ToArray());
        }
    }
}
