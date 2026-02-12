using RresourcepackStudio.Classes.Configs;
using RresourcepackStudio.Utils.IO;
using System.Windows;
using System.Windows.Controls;

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
        }

        #region MenuItem 事件
        
        private void MenuItem_New_Click(object sender, RoutedEventArgs e) => ProjectManager.CreateProject();
        private void MenuItem_Open_Click(object sender, RoutedEventArgs e) => ProjectManager.OpenProject();
        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown(0);
        
        #endregion

        public void LoadTreeView(JsonProjectConfig.FileInfo root)
        {
            var item = new TreeViewItem
            {
                Header = root.Name,
                Tag = root,
                IsExpanded = root.Type == FileType.Folder
            };

            treeView_Main.Items.Add(item);

            if (root.Children != null && root.Children.Length > 0)
            {
                foreach (var child in root.Children)
                {
                    LoadTreeView(child);
                }
            }
        }

        public void 
    }
}
