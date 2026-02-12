using RresourcepackStudio.Classes;
using RresourcepackStudio.Classes.Configs;
using RresourcepackStudio.Controls.Icons;
using RresourcepackStudio.Utils.IO;
using RresourcepackStudio.Utils.UI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            Loaded += ((s,e) =>
            {
                SetUIEnabled(false);
            });
        }

        #region MenuItem 事件
        
        //文件
        private void MenuItem_New_Click(object sender, RoutedEventArgs e) => ProjectManager.CreateProject();
        private void MenuItem_Open_Click(object sender, RoutedEventArgs e) => ProjectManager.OpenProject();
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

        #region TreeView操作

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
                button_NewFile.IsEnabled = false;button_NewFolder.IsEnabled = false;

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

        #endregion

        #endregion






        #region UI事件

        //Treeview选择项更改
        private void treeView_Main_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeView_Main.SelectedItem is not TreeViewItem treeViewItem)
                return;

            bool isEnabled = false;
            if (treeView_Main.SelectedItem != null)
            {
                if (treeViewItem.Tag != null && treeViewItem.Tag is JsonProjectConfig.FileInfo fileInfo && fileInfo.Type != FileType.File) 
                {
                    isEnabled = true;
                }
            }


            button_NewFile.IsEnabled = isEnabled;
            button_NewFolder.IsEnabled = isEnabled;


        }

        #endregion
    }
}
