using Notifications.Wpf;
using RresourcepackStudio.Classes;
using RresourcepackStudio.Classes.Configs;
using RresourcepackStudio.Controls.Icons;
using System;
using System.Collections.Generic;
using System.Text;
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
            //遍历添加
            void AddItemFromFileInfo(JsonProjectConfig.FileInfo fileInfo, TreeViewItem root)
            {
                //定义item
                var item = new TreeViewItem
                {
                    Header = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Children =
                        {
                            fileInfo.Type==FileType.Folder?new IconFolder():new IconFile(),
                            new TextBlock
                            {
                                Text=fileInfo.Name,
                                Margin=new Thickness(5,0,0,0),
                                VerticalAlignment=VerticalAlignment.Center
                            }
                        }
                    },
                    Tag = fileInfo
                };

                root.Items.Add(item);

                if (fileInfo.Children != null && fileInfo.Children.Length > 0)
                {
                    foreach (var fileChild in fileInfo.Children)
                    {
                        //继续遍历
                        AddItemFromFileInfo(fileChild, item);
                    }
                }
            }

            try
            {
                treeView.Items.Clear();

                //添加顶级元素
                var item = new TreeViewItem
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

                treeView.Items.Add(item);

                //添加普通文件
                if (Globals.CurrentProject.Files != null)
                    foreach (var file in Globals.CurrentProject.Files!)
                        AddItemFromFileInfo(file, item);

            }
            catch (Exception ex)
            {
                ErrorReportDialog.Show(ex);
            }
        }

        /// <summary>
        /// 创建文件，带无效/同名检测，无需手动检测
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="rootItem">要创建的对象</param>
        public static void NewFile(string fileName, TreeViewItem rootItem)
        {
            var rootFileInfo = (JsonProjectConfig.FileInfo)rootItem.Tag;

            //无效名检测
            if (!CharChecker.Check(fileName))
            {
                new NotificationManager().Show(new NotificationContent
                {
                    Title = "文件无法创建",
                    Message = "文件名包含了无效字符",
                    Type = NotificationType.Error
                });
                return;
            }


            //同名检测
            ItemCollection tempCollection;
            if (rootFileInfo.Type == FileType.File)
                tempCollection = ((TreeViewItem)rootItem.Parent).Items;
            else
                tempCollection = rootItem.Items;
            foreach (TreeViewItem item in tempCollection)
            {
                if (((JsonProjectConfig.FileInfo)item.Tag).Name == fileName)
                {
                    new NotificationManager().Show(new NotificationContent
                    {
                        Title = "文件无法创建",
                        Message = "已有同名文件",
                        Type = NotificationType.Error
                    });
                    return;
                }
            }

            var addItem = new TreeViewItem
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
                Tag = new JsonProjectConfig.FileInfo
                {
                    Name = fileName,
                    Type = FileType.File,
                    Children = null
                }
            };

            if (rootFileInfo.Type == FileType.File)
                ((TreeViewItem)rootItem.Parent).Items.Add(addItem);
            else
                rootItem.Items.Add(addItem);
        }

        /// <summary>
        /// 创建文件夹，带无效/同名检测，无需手动检测
        /// </summary>
        /// <param name="fileName">文件夹名</param>
        /// <param name="rootItem">要创建的对象</param>
        public static void NewFolder(string folderName, TreeViewItem rootItem)
        {
            var rootFileInfo = (JsonProjectConfig.FileInfo)rootItem.Tag;

            //无效名检测
            if (!CharChecker.Check(folderName))
            {
                new NotificationManager().Show(new NotificationContent
                {
                    Title = "文件夹无法创建",
                    Message = "文件夹名包含了无效字符",
                    Type = NotificationType.Error
                });
                return;
            }


            //同名检测
            ItemCollection tempCollection;
            if (rootFileInfo.Type == FileType.File)
                tempCollection = ((TreeViewItem)rootItem.Parent).Items;
            else
                tempCollection = rootItem.Items;
            foreach (TreeViewItem item in tempCollection)
            {
                if (((JsonProjectConfig.FileInfo)item.Tag).Name == folderName)
                {
                    new NotificationManager().Show(new NotificationContent
                    {
                        Title = "文件夹无法创建",
                        Message = "已有同名文件夹",
                        Type = NotificationType.Error
                    });
                    return;
                }
            }

            var addItem = new TreeViewItem
            {
                Header = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                        {
                            new IconFile(),
                            new TextBlock
                            {
                                Text=folderName,
                                Margin=new Thickness(5,0,0,0),
                                VerticalAlignment=VerticalAlignment.Center
                            }
                        }
                },
                Tag = new JsonProjectConfig.FileInfo
                {
                    Name = folderName,
                    Type = FileType.File,
                    Children = null
                }
            };

            if (rootFileInfo.Type == FileType.File)
                ((TreeViewItem)rootItem.Parent).Items.Add(addItem);
            else
                rootItem.Items.Add(addItem);
        }

    }
}
