using ModernWpf.Controls;
using Notifications.Wpf;
using RresourcepackStudio.Classes;
using RresourcepackStudio.Classes.Configs;
using RresourcepackStudio.Controls.Icons;
using RresourcepackStudio.Utils.IO;
using RresourcepackStudio.Utils.UI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace RresourcepackStudio.Windows
{
    /// <summary>
    /// WindowMain.xaml 的交互逻辑
    /// </summary>
    public partial class WindowMain : Window
    {
        public WindowMain()
        {
            InitializeComponent();
            Loaded += ((s, e) =>
            {
                SetUIEnabled(false);
                UpdateMenuItem();
            });
        }

        #region MenuItem 事件

        //文件
        private void MenuItem_New_Click(object sender, RoutedEventArgs e) => ProjectManager.CreateProject();
        private void MenuItem_Open_Click(object sender, RoutedEventArgs e) => ProjectManager.OpenProject();
        private void MenuItem_Save_Click(object sender, RoutedEventArgs e) => ProjectManager.SaveProject();
        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown(0);

        //帮助
        private void MenuItem_About_Click(object sender, RoutedEventArgs e) => new WindowAbout().ShowDialog();

        #endregion

        #region UI操作

        public void SetUIEnabled(bool enabled)
        {
            grid_Main.IsEnabled = enabled;
            grid_Main.Visibility = enabled ? Visibility.Visible : Visibility.Hidden;
        }

        public void UpdateMenuItem()
        {
            if (Globals.CurrentProject == null)
            {
                menuItem_Save.IsEnabled = false;
            }
            else
            {
                menuItem_Save.IsEnabled = true;
            }
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.Q:
                        if (menuItem_Exit.IsEnabled)
                            MenuItem_Exit_Click(null!, null!);
                        break;
                    case Key.S:
                        if (menuItem_Save.IsEnabled)
                            MenuItem_Save_Click(null!, null!);
                        break;
                }
            }
        }

        #region TreeView操作

        //Treeview选择项更改
        private void treeView_Main_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeView_Main.SelectedItem is not TreeViewItem treeViewItem)
                return;

            bool isEnabled = false;
            if (treeView_Main.SelectedItem != null && treeViewItem.Tag != null) 
            {
                isEnabled = true;
            }


            button_NewFile.IsEnabled = isEnabled;
            button_NewFolder.IsEnabled = isEnabled;


        }

        //加载TreeView
        public void LoadTreeView()
        {
            void AddItemFromFileInfo(JsonProjectConfig.FileInfo fileInfo, TreeViewItem root)
            {
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
                        AddItemFromFileInfo(fileChild, item);
                    }
                }
            }

            try
            {
                treeView_Main.Items.Clear();
                button_NewFile.IsEnabled = false; button_NewFolder.IsEnabled = false;

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

                treeView_Main.Items.Add(item);

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

        //创建文件
        private async void button_NewFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (treeView_Main.SelectedItem is not TreeViewItem treeViewItem || treeViewItem.Tag is not JsonProjectConfig.FileInfo fileInfo)
                    return;

                //文件名
                var textBox = new TextBox
                {
                    Text = "新文件"
                };
                bool isContinue = false;
                await DialogManager.ShowDialogAsync(new ContentDialog
                {
                    Title = "创建文件",
                    Content = textBox,
                    PrimaryButtonText = "确定",
                    CloseButtonText = "取消",
                    DefaultButton = ContentDialogButton.Primary
                }, (() => isContinue = true));

                if (!isContinue)
                    return;

                //无效名检测
                if (!CharChecker.Check(textBox.Text))
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
                if (fileInfo.Type == FileType.File)
                    tempCollection = ((TreeViewItem)treeViewItem.Parent).Items;
                else
                    tempCollection = treeViewItem.Items;
                foreach (TreeViewItem item in tempCollection)
                {
                    if (((JsonProjectConfig.FileInfo)item.Tag).Name == textBox.Text)
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
                                Text=textBox.Text,
                                Margin=new Thickness(5,0,0,0),
                                VerticalAlignment=VerticalAlignment.Center
                            }
                        }
                    },
                    Tag = new JsonProjectConfig.FileInfo
                    {
                        Name = textBox.Text,
                        Type = FileType.File,
                        Children = null
                    }
                };

                if (fileInfo.Type == FileType.File)
                    ((TreeViewItem)treeViewItem.Parent).Items.Add(addItem);
                else
                    treeViewItem.Items.Add(addItem);

            }
            catch (Exception ex)
            {
                ErrorReportDialog.Show(ex);
            }
        }

        //创建文件夹
        private async void button_NewFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (treeView_Main.SelectedItem is not TreeViewItem treeViewItem || treeViewItem.Tag is not JsonProjectConfig.FileInfo fileInfo)
                    return;

                //文件名
                var textBox = new TextBox
                {
                    Text = "新文件夹"
                };
                bool isContinue = false;
                await DialogManager.ShowDialogAsync(new ContentDialog
                {
                    Title = "创建文件夹",
                    Content = textBox,
                    PrimaryButtonText = "确定",
                    CloseButtonText = "取消",
                    DefaultButton = ContentDialogButton.Primary
                }, (() => isContinue = true));

                if (!isContinue)
                    return;

                //无效名检测
                if (!CharChecker.Check(textBox.Text))
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
                if (fileInfo.Type == FileType.File)
                    tempCollection = ((TreeViewItem)treeViewItem.Parent).Items;
                else
                    tempCollection = treeViewItem.Items;
                foreach (TreeViewItem item in tempCollection)
                {
                    if (((JsonProjectConfig.FileInfo)item.Tag).Name == textBox.Text)
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
                            new IconFolder(),
                            new TextBlock
                            {
                                Text=textBox.Text,
                                Margin=new Thickness(5,0,0,0),
                                VerticalAlignment=VerticalAlignment.Center
                            }
                        }
                    },
                    Tag = new JsonProjectConfig.FileInfo
                    {
                        Name = textBox.Text,
                        Type = FileType.Folder,
                        Children = null
                    }
                };

                if (fileInfo.Type == FileType.File)
                    ((TreeViewItem)treeViewItem.Parent).Items.Add(addItem);
                else
                    treeViewItem.Items.Add(addItem);
            }
            catch (Exception ex)
            {
                ErrorReportDialog.Show(ex);
            }
        }

        #endregion

        #endregion

        
    }
}
