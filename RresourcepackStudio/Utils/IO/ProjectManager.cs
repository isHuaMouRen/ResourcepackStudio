using RresourcepackStudio.Classes.Configs;
using System.IO;
using RresourcepackStudio.Utils.UI;
using RresourcepackStudio.Windows;
using Notifications.Wpf;
using Microsoft.Win32;
using RresourcepackStudio.Classes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
                
                //写Json
                JsonHelper.WriteJson(Path.Combine(window.ProjectPath!, $"{window.ProjectIndex!.Name}.rpsp"), window.ProjectIndex!);

                //初始化文件系统
                SetupFileSystem(window.ProjectPath!, window.ProjectIndex.Files!);


                new NotificationManager().Show(new NotificationContent
                {
                    Title = "项目创建成功",
                    Message = $"项目 {window.ProjectIndex.Name} 成功创建！",
                    Type = NotificationType.Success
                });

                Globals.CurrentProject = window.ProjectIndex;
                Globals.CurrentProjectDirectory = window.ProjectPath;
                LoadProject();
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


                FileManager.LoadTreeView(window.treeView_Main);
                window.button_NewFile.IsEnabled = false;window.button_NewFolder.IsEnabled = false;
                window.SetUIEnabled(true);
                window.UpdateMenuItem();
            }
            catch (Exception ex)
            {
                ErrorReportDialog.Show(ex);
            }
        }

        public static void SaveProject()
        {
            try
            {
                var treeView = ((WindowMain)Application.Current.MainWindow).treeView_Main;

                //根据TreeView反序列化FIleINfo
                Globals.CurrentProject!.Files = FileManager.ParseTreeviewToFileInfoArray(treeView);
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


        public static void SetupFileSystem(string rootPath, JsonProjectConfig.FileInfo[] rootInfo)
        {
            void AddFileFromFileInfo(string path, JsonProjectConfig.FileInfo fileInfo)
            {
                if (fileInfo.Type != FileType.Folder)
                    return;
                
                var targetPath = Path.Combine(path, fileInfo.Name);
                Directory.CreateDirectory(targetPath);

                if(fileInfo.Children!=null && fileInfo.Children.Length > 0)
                {
                    foreach (var chilren in fileInfo.Children)
                    {
                        AddFileFromFileInfo(targetPath, chilren);
                    }
                }
            }



            try
            {
                var rootDirectory = Path.Combine(rootPath, "files");

                foreach (var item in rootInfo)
                {
                    AddFileFromFileInfo(rootDirectory, item);
                }


                return;
            }
            catch (Exception ex)
            {
                ErrorReportDialog.Show(ex);
            }
        }
    }
}
