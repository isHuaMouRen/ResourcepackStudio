using RresourcepackStudio.Classes;
using System;
using System.Collections.Generic;
using System.Text;
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
    /// WindowAbout.xaml 的交互逻辑
    /// </summary>
    public partial class WindowAbout : Window
    {
        public WindowAbout()
        {
            InitializeComponent();
            textBlock_Version.Text = Globals.Version;
        }
    }
}
