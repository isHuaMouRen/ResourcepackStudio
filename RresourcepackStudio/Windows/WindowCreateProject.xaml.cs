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
        public JsonProjectFileConfig.Index? ProjectIndex { get; set; } = null;



        public WindowCreateProject()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (var c in e.Text)
            {
                if (c is '\\' or '/' or ':' or '*' or '?' or '<' or '>' or '|' || char.IsControl(c))
                    e.Handled = true; return;
            }
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
                        Tag = new JsonProjectFileConfig.LanguageInfo { Code = tbCode.Text, Name = tbName.Text, Region = tbRegion.Text }
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
            bool canCreate = true;


            if (textBox_ProjectName.Text == null)
                canCreate = false;
            else
                foreach (var c in textBox_ProjectName.Text)
                    if (c is '/' or '\\' or '<' or '>' or '|' or '*' or '?')
                        canCreate = false;

            if (string.IsNullOrEmpty(textBox_Path.Text))
                canCreate = false;




            if (canCreate)
            {
                ProjectPath = textBox_Path.Text;


                var langList = new List<JsonProjectFileConfig.LanguageInfo>();

                foreach (var item in listBox_Lang.Items)
                    if (item is ListBoxItem lbItem)
                        langList.Add((JsonProjectFileConfig.LanguageInfo)lbItem.Tag);

                ProjectIndex = new JsonProjectFileConfig.Index
                {
                    Name = textBox_ProjectName.Text!,
                    PackInfo = new JsonProjectFileConfig.PackInfo
                    {
                        BuildName = $"{textBox_ProjectName.Text}.zip",
                        Description = textBox_Description.Text,
                        Version = new JsonProjectFileConfig.VersionInfo
                        {
                            MinMain = (int)numBox_MinMain.Value,
                            MinSub = (int)numBox_MinSub.Value,
                            Neutral = (int)numBox_Neutral.Value,
                            MaxMain = (int)numBox_MaxMain.Value,
                            MaxSub = (int)numBox_MaxSub.Value
                        },
                        Language = (langList.Count > 0) ? langList.ToArray() : new JsonProjectFileConfig.LanguageInfo[] { }
                    },
                    Files = new JsonProjectFileConfig.FileInfo[] { }
                };


                DialogResult = true;
                this.Close();
            }
            else
            {
                await DialogManager.ShowDialogAsync(new ContentDialog
                {
                    Title = "无法创建",
                    Content = "内容校验不通过，请检查是否按照要求填写内容",
                    PrimaryButtonText = "确定",
                    DefaultButton = ContentDialogButton.Primary
                });
            }
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
