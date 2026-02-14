using Microsoft.Win32;
using ModernWpf.Controls;
using Notifications.Wpf;
using RresourcepackStudio.Classes.Configs;
using RresourcepackStudio.Utils.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RresourcepackStudio.Windows
{
    /// <summary>
    /// WindowCreateProject.xaml 的交互逻辑
    /// </summary>
    public partial class WindowCreateProject : Window
    {
        public string? ProjectPath { get; set; } = null;
        public JsonProjectConfig.Index? ProjectConfig { get; set; } = null;



        public WindowCreateProject()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!CharChecker.Check(e.Text))
                e.Handled = true;
        }

        private async void Button_AddLang_Click(object sender, RoutedEventArgs e)
        {
            var tbCode = new TextBox
            {
                Text = "zh_cn",
                Margin = new Thickness(0, 0, 0, 10)
            };
            var tbName = new TextBox
            {
                Text = "简体中文",
                Margin = new Thickness(0, 0, 0, 10)
            };
            var tbRegion = new TextBox
            {
                Text = "中国大陆",
                Margin = new Thickness(0, 0, 0, 10)
            };

            var dialog = new ContentDialog
            {
                Title = "添加语言",
                Content = new StackPanel
                {
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "语言代码",
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(0, 0, 0, 5)
                        },
                        tbCode,
                        new TextBlock
                        {
                            Text = "语言名称",
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(0, 0, 0, 5)
                        },
                        tbName,
                        new TextBlock
                        {
                            Text = "地区名称",
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(0, 0, 0, 5)
                        },
                        tbRegion
                    }
                },
                PrimaryButtonText = "确定",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary
            };
            await DialogManager.ShowDialogAsync(dialog, (() =>
            {
                if (string.IsNullOrEmpty(tbCode.Text) || string.IsNullOrEmpty(tbName.Text) || string.IsNullOrEmpty(tbRegion.Text))
                {
                    new NotificationManager().Show(new NotificationContent
                    {
                        Title = "添加语言失败",
                        Message = "值不可为空",
                        Type = NotificationType.Error
                    });
                }
                else
                {
                    var listboxItem = new ListBoxItem
                    {
                        Content = tbName.Text,
                        Tag = new JsonProjectConfig.LanguageInfo { Code = tbCode.Text, Name = tbName.Text, Region = tbRegion.Text }
                    };
                    listBox_Lang.Items.Add(listboxItem);
                }
            }));


        }

        private void Button_RemoveLang_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_Lang.SelectedItem == null || listBox_Lang.SelectedItem is not ListBoxItem)
                return;

            listBox_Lang.Items.Remove(listBox_Lang.SelectedItem);
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private async void Button_Done_Click(object sender, RoutedEventArgs e)
        {
            


            DialogResult = true;
            this.Close();
        }

        private void Button_Broswer_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Multiselect = false,
                Title = "选择你要创建项目的文件夹"
            };
            if (dialog.ShowDialog() == true)
            {
                textBox_Path.Text = dialog.FolderName;
            }
        }
    }
}
