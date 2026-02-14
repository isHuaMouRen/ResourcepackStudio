using ResourcepackStudio.Classes.Configs;
using System.IO;
using ResourcepackStudio.Utils.UI;
using ResourcepackStudio.Windows;
using Notifications.Wpf;
using ModernWpf.Controls;
using ResourcepackStudio.Classes;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace ResourcepackStudio.Utils.IO
{
    public static class ProjectManager
    {
        /// <summary>
        /// 创建项目，无任何保护，直接操作
        /// </summary>
        /// <param name="path">项目路径</param>
        /// <param name="projectIndex">项目信息</param>
        /// <returns>true为成功，false为失败</returns>
        public static bool CreateProject(string path, JsonProjectConfig.Index projectIndex)
        {
            try
            {
                string projectFile = Path.Combine(path, $"{projectIndex.Name}.rpsp");

                JsonHelper.WriteJson(projectFile, projectIndex);

                //创建基础目录结构
                /*
                 assets
                    minecraft
                        lang
                        models
                            block
                            item
                        texts
                        textures
                 */

                string filesPath = Path.Combine(path, "files");
                string[] files = {
                    Path.Combine(filesPath,"assets"),
                    Path.Combine(filesPath,"assets","minecraft"),
                    Path.Combine(filesPath,"assets","minecraft","lang"),
                    Path.Combine(filesPath,"assets","minecraft","models"),
                    Path.Combine(filesPath,"assets","minecraft","models","block"),
                    Path.Combine(filesPath,"assets","minecraft","models","item"),
                    Path.Combine(filesPath,"assets","minecraft","texts"),
                    Path.Combine(filesPath,"assets","minecraft","textures")
                };

                foreach (var file in files)
                    Directory.CreateDirectory(file);

                return true;
            }
            catch (Exception ex)
            {
                ErrorReportDialog.Show(ex);
                return false;
            }
        }

        public static async void CreateProjectEx()
        {
            try
            {
                var window = new WindowCreateProject();
                if (window.ShowDialog() != true)
                    return;

                if (File.Exists(Path.Combine(window.ProjectPath!, $"{window.ProjectConfig!.Name}.rpsp"))) 
                {
                    bool isReturn = false;
                    await DialogManager.ShowDialogAsync(new ContentDialog
                    {
                        Title = "发现重名文件",
                        Content = $"我们在 \"{window.ProjectPath}\" 处发现了与 \"{window.ProjectConfig.Name}.rpsp\" 同名的文件",
                        PrimaryButtonText = "覆盖",
                        CloseButtonText = "取消",
                        DefaultButton = ContentDialogButton.Primary
                    }, null, (() => isReturn = true), (() => isReturn = true));

                    if (isReturn)
                        return;
                }



                var result = CreateProject(window.ProjectPath!, window.ProjectConfig!);
                if (result)
                    new NotificationManager().Show(new NotificationContent
                    {
                        Title = "创建项目",
                        Message = $"成功在 \"{window.ProjectPath}\" 处创建项目 \"{window.ProjectConfig!.Name}\"",
                        Type = NotificationType.Success
                    });
                else
                    new NotificationManager().Show(new NotificationContent
                    {
                        Title = "创建项目",
                        Message = $"在创建项目时出现了错误，这可能是内部错误，如果确定不是您引发的请反馈至开发者",
                        Type = NotificationType.Error
                    });



                if (result)
                {
                    var mainWindow = (WindowMain)Application.Current.MainWindow;

                    OpenProject(Path.Combine(window.ProjectPath!, $"{window.ProjectConfig.Name}.rpsp"));
                    LoadProject();

                    mainWindow.UpdateMenuItem();
                    mainWindow.SetUserInterfaceEnabled(true);
                }
            }
            catch (Exception ex)
            {
                ErrorReportDialog.Show(ex);
            }
        }


        /// <summary>
        /// 打开项目到全局当前项目
        /// </summary>
        /// <param name="projectPath">项目文件路径</param>
        /// <returns>true成功，false失败</returns>
        public static bool OpenProject(string projectPath)
        {
            try
            {
                var projectInfo = JsonHelper.ReadJson<JsonProjectConfig.Index>(projectPath);

                Globals.CurrentProject = projectInfo;
                Globals.CurrentProjectDirectory = Path.GetDirectoryName(projectPath);

                return true;
            }
            catch (Exception ex)
            {
                ErrorReportDialog.Show(ex);
                return false;
            }
        }

        public static void OpenProjectEx()
        {
            try
            {
                var mainWindow = (WindowMain)Application.Current.MainWindow;


                //选择文件
                var dialog = new OpenFileDialog
                {
                    Filter = "Resourcepack Studio Project|*.rpsp",
                    Title = "选择一个有效的项目文件",
                    Multiselect = false
                };
                if (dialog.ShowDialog() != true)
                    return;

                //读到全局类
                var resultOpen = OpenProject(dialog.FileName);
                if (!resultOpen)
                    new NotificationManager().Show(new NotificationContent
                    {
                        Title = "打开项目",
                        Message = $"无法打开项目 \"{dialog.FileName}\"",
                        Type = NotificationType.Error
                    });

                //加载
                var resultLoad = LoadProject();
                if (!resultLoad)
                    new NotificationManager().Show(new NotificationContent
                    {
                        Title = "加载项目",
                        Message = $"无法加载项目 \"{Globals.CurrentProject!.Name}\"",
                        Type = NotificationType.Error
                    });


                //更新主窗口
                if (resultLoad && resultOpen)
                {
                    mainWindow.SetUserInterfaceEnabled(true);
                    mainWindow.UpdateMenuItem();
                }
            }
            catch (Exception ex)
            {
                ErrorReportDialog.Show(ex);
            }
        }


        /// <summary>
        /// 从全局当前项目加载项目
        /// </summary>
        /// <returns>true成功，false失败</returns>
        public static bool LoadProject()
        {
            try
            {
                TreeViewController.Clear();
                TreeViewController.AddRootNode(Globals.CurrentProject!);
                TreeViewController.LoadTree(Path.Combine(Globals.CurrentProjectDirectory!, "files"), (TreeViewItem)TreeViewController.TreeViewMain.Items[0]);


                return true;
            }
            catch (Exception ex)
            {
                ErrorReportDialog.Show(ex);
                return false;
            }
        }


        /// <summary>
        /// 保存项目
        /// </summary>
        /// <returns>ture成功，false失败</returns>
        public static bool SaveProject()
        {
            try
            {
                string projectFile = Path.Combine(Globals.CurrentProjectDirectory!, $"{Globals.CurrentProject!.Name}.rpsp");
                JsonHelper.WriteJson(projectFile, Globals.CurrentProject);


                return true;
            }
            catch (Exception ex)
            {
                ErrorReportDialog.Show(ex);
                return false;
            }
        }

        public static void SaveProjectEx()
        {
            try
            {
                var result = SaveProject();
                if (result)
                    new NotificationManager().Show(new NotificationContent
                    {
                        Title = "保存项目",
                        Message = $"项目 \"{Globals.CurrentProject!.Name}\" 成功保存至 \"{Path.Combine(Globals.CurrentProjectDirectory!, $"{Globals.CurrentProject.Name}.rpsp")}\"",
                        Type = NotificationType.Success
                    });
                else
                    new NotificationManager().Show(new NotificationContent
                    {
                        Title = "保存项目",
                        Message = $"无法保存项目 \"{Globals.CurrentProject!.Name}\"",
                        Type = NotificationType.Error
                    });
            }
            catch (Exception ex)
            {
                ErrorReportDialog.Show(ex);
            }
        }
    }
}
