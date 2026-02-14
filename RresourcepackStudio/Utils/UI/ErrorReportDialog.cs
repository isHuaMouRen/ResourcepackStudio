using Ookii.Dialogs.Wpf;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ResourcepackStudio.Utils.UI
{
    public static class ErrorReportDialog
    {
        public static async void Show(Exception ex, bool isUnHandleException = false)
        {
            var dialog = new TaskDialog
            {
                WindowTitle = isUnHandleException ? "发生未捕获的错误" : "发生错误",
                MainIcon = TaskDialogIcon.Error,
                MainInstruction = isUnHandleException ? "程序在运行时发生了未捕获的错误" : "程序在运行时发生错误",
                Content = ex.ToString(),
                AllowDialogCancellation = false,
                ButtonStyle = TaskDialogButtonStyle.CommandLinks
            };


            var btnEnd = new TaskDialogButton
            {
                Text = "终止程序（推荐）",
                CommandLinkNote = "丢弃现在状态并退出程序"
            };
            var btnCopyAndEnd = new TaskDialogButton
            {
                Text = "复制错误信息并退出",
                CommandLinkNote = "复制错误信息并退出程序"
            };
            var btnContinue = new TaskDialogButton
            {
                Text = "继续运行（不推荐）",
                CommandLinkNote = "忽略这个错误并继续执行程序（出现错误后程序一般无法正常运行）"
            };

            var btnClose = new TaskDialogButton
            {
                Text = "关闭",
                ButtonType = ButtonType.Close
            };

            dialog.Buttons.Add(btnEnd);
            dialog.Buttons.Add(btnCopyAndEnd);

            if (!isUnHandleException)
            {
                dialog.Buttons.Add(btnContinue);
            }

            dialog.Buttons.Add(btnClose);


            var result = dialog.ShowDialog();


            if (result == btnEnd)
            {
                Environment.Exit(1);
            }
            else if (result == btnCopyAndEnd)
            {
                Clipboard.SetText($"{new string('=', 5)}[ResourcepackStudio 错误信息]{new string('=', 5)}\n\n{ex}\n\n{new string('=', 25)}");
                Environment.Exit(1);
            }
        }
    }
}
