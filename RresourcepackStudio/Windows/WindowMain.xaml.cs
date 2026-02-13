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
        public ContextMenu treeViewItemContextMenu = new ContextMenu();



        public WindowMain()
        {
            InitializeComponent();
            Loaded += ((s, e) =>
            {
                this.Title = $"Resourcepack Studio {Globals.Version}";
                SetUIEnabled(false);
                UpdateMenuItem();



                //创建ContextMenu
                treeViewItemContextMenu.Items.Add(new MenuItem
                {
                    Header = "新建",
                    Items =
                    {
                        CreateMenuItem("新建文件",MenuItem_CM_NewFile_Click),
                        CreateMenuItem("新建文件夹",MenuItem_CM_NewFolder_Click)
                    }
                });
                treeViewItemContextMenu.Items.Add(CreateMenuItem("重命名", MenuItem_CM_Rename_Click));
                treeViewItemContextMenu.Items.Add(CreateMenuItem("删除", MenuItem_CM_Delete_Click));
            });
        }

        private MenuItem CreateMenuItem(string header,RoutedEventHandler handler)
        {
            var item = new MenuItem
            {
                Header = header
            };
            if (handler != null)
                item.Click += handler;
            return item;
        }


        #region MenuItem 事件

        //文件
        private void MenuItem_New_Click(object sender, RoutedEventArgs e) => ProjectManager.CreateProject();
        private void MenuItem_Open_Click(object sender, RoutedEventArgs e) => ProjectManager.OpenProject();
        private void MenuItem_Save_Click(object sender, RoutedEventArgs e) => ProjectManager.SaveProject();
        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown(0);

        //帮助
        private void MenuItem_About_Click(object sender, RoutedEventArgs e) => new WindowAbout().ShowDialog();


        //右键菜单
        private void MenuItem_CM_NewFile_Click(object sender, RoutedEventArgs e) => button_NewFile_Click(button_NewFile, null!);
        private void MenuItem_CM_NewFolder_Click(object sender, RoutedEventArgs e) => button_NewFolder_Click(button_NewFolder, null!);
        private async void MenuItem_CM_Rename_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private async void MenuItem_CM_Delete_Click(object sender, RoutedEventArgs e)
        {
            
        }
        
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
            
        }

        //创建文件
        private async void button_NewFile_Click(object sender, RoutedEventArgs e)
        {
            
        }

        //创建文件夹
        private async void button_NewFolder_Click(object sender, RoutedEventArgs e)
        {
            
        }

        #endregion

        #endregion

        
    }
}
