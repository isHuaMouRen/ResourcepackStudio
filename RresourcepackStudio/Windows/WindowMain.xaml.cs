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
                this.Title = $"Resourcepack Studio {Globals.Version}";
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

                FileManager.NewFile(textBox.Text, treeViewItem);
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

                FileManager.NewFolder(textBox.Text, treeViewItem);
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
