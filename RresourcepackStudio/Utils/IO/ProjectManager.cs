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
                if (window.ShowDialog() != true)
                    return;


                if (!Directory.Exists(window.ProjectPath))
                    Directory.CreateDirectory(window.ProjectPath!);

                JsonHelper.WriteJson(Path.Combine(window.ProjectPath!, $"{window.ProjectConfig!.Name}.rpsp"), window.ProjectConfig);

                //创建基础文件夹
                /*
                 assets
                    minecraft
                        lang
                        models
                        texts
                        textures
                 */




                new NotificationManager().Show(new NotificationContent
                {
                    Title = "项目创建成功",
                    Message = $"已在 \"{window.ProjectPath}\" 处创建项目 \"{window.ProjectConfig.Name}\"",
                    Type = NotificationType.Success
                });

                Globals.CurrentProject = window.ProjectConfig;
                Globals.CurrentProjectDirectory = window.ProjectPath;

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
                var window = (WindowMain)Application.Current.MainWindow;

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
                

            }
            catch (Exception ex)
            {
                ErrorReportDialog.Show(ex);
            }
        }

    }
}
