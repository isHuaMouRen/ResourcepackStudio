using RresourcepackStudio.Classes.Configs;
using System.IO;
using System;
using RresourcepackStudio.Utils.UI;
using RresourcepackStudio.Windows;
using Notifications.Wpf;
using System.Runtime.CompilerServices;
using System.Windows;

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
    }
}
