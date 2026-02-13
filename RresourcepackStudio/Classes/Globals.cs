using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using RresourcepackStudio.Classes.Configs;

namespace RresourcepackStudio.Classes
{
    public static class Globals
    {
        public static string Version = "1.0.0-beta.4";//版本号
        public static string ExecuteDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;//执行目录

        public static JsonProjectConfig.Index? CurrentProject = null;//当前项目文件
        public static string? CurrentProjectDirectory = null;//当前项目目录
    }
}
