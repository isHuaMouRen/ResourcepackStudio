using RresourcepackStudio.Classes.Configs;
using System.IO;
using RresourcepackStudio.Utils.UI;
using RresourcepackStudio.Windows;
using Notifications.Wpf;
using Microsoft.Win32;
using RresourcepackStudio.Classes;
using System.Windows;
using System.Windows.Controls;

namespace RresourcepackStudio.Utils.IO
{
    public static class ProjectManager
    {
        public static void CreateProject()
        {
			try
			{
                var window = new WindowCreateProject();
                window.Owner = App.Current.MainWindow;

                if (window.ShowDialog() != true)
                    return;
                

                if (!Directory.Exists(window.ProjectPath))
                    Directory.CreateDirectory(window.ProjectPath!);

                JsonHelper.WriteJson(Path.Combine(window.ProjectPath!, $"{window.ProjectIndex!.Name}.rpsp"), window.ProjectIndex!);

                new NotificationManager().Show(new NotificationContent
                {
                    Title = "项目创建成功",
                    Message = $"项目 {window.ProjectIndex.Name} 成功创建！",
                    Type = NotificationType.Success
                });
			}
			catch (Exception ex)
			{
                ErrorReportDialog.Show(ex);
			}
        }

        public static void OpenProject()
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Multiselect = false,
                    Title = "选择一个有效的项目文件 (.rpsp)",
                    Filter = "ResourcepackStudio项目文件|*.rpsp"
                };

                if (dialog.ShowDialog() != true)
                    return;

                if (!File.Exists(dialog.FileName))
                {
                    new NotificationManager().Show(new NotificationContent
                    {
                        Title = "无法打开项目",
                        Message = "项目文件不存在",
                        Type = NotificationType.Error
                    });
                    return;
                }


                Globals.CurrentProject = JsonHelper.ReadJson<JsonProjectConfig.Index>(dialog.FileName);
                Globals.CurrentProjectDirectory = Path.GetDirectoryName(dialog.FileName);

                LoadProject();
            }
            catch (Exception ex)
            {
                ErrorReportDialog.Show(ex);
            }
        }

        public static void LoadProject()
        {
            try
            {
                if (App.Current.MainWindow is not WindowMain window)
                    throw new Exception("程序主窗口无效，这通常是内部代码问题。请反馈给开发者！");


                window.LoadTreeView();
                window.SetUIEnabled(true);
            }
            catch (Exception ex)
            {
                ErrorReportDialog.Show(ex);
            }
        }

        public static void SaveProject()
        {
            JsonProjectConfig.FileInfo AddFileInfoFromTreeView(TreeViewItem treeViewItem, JsonProjectConfig.FileInfo original)
            {
                var node = new JsonProjectConfig.FileInfo
                {
                    Name = original.Name,
                    Type = original.Type
                };

                if (node.Type == FileType.Folder && treeViewItem.HasItems)
                {
                    var children = new List<JsonProjectConfig.FileInfo>();

                    foreach (TreeViewItem childItem in treeViewItem.Items)
                    {
                        if(childItem.Tag is JsonProjectConfig.FileInfo childOrigin)
                        {
                            var childNode = AddFileInfoFromTreeView(childItem, childOrigin);
                            if (childNode != null)
                            {
                                children.Add(childNode);
                            }
                        }
                    }

                    if (children.Count > 0)
                    {
                        node.Children = children.ToArray();
                    }
                    else
                    {
                        node.Children = null;
                    }

                }
                else
                {
                    node.Children = null;
                }

                return node;
            }


            try
            {
                var treeView = ((WindowMain)Application.Current.MainWindow).treeView_Main;


                var result = new List<JsonProjectConfig.FileInfo>();
                //根据TreeView反序列化FIleINfo
                foreach (TreeViewItem item in ((TreeViewItem)treeView.Items[0]).Items)
                {
                    if(item.Tag is JsonProjectConfig.FileInfo fileInfo)
                    {
                        var cloned = AddFileInfoFromTreeView(item, fileInfo);
                        if (cloned != null)
                            result.Add(cloned);
                    }
                }

                Globals.CurrentProject!.Files = result.ToArray();
                JsonHelper.WriteJson(Path.Combine(Globals.CurrentProjectDirectory!, $"{Globals.CurrentProject.Name}.rpsp"), Globals.CurrentProject);

                new NotificationManager().Show(new NotificationContent
                {
                    Title = "项目已保存",
                    Message = "项目已保存",
                    Type = NotificationType.Success
                });

            }
            catch (Exception ex)
            {
                ErrorReportDialog.Show(ex);
            }
        }
    }
}
