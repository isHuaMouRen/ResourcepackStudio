using ModernWpf.Controls;

namespace ResourcepackStudio.Utils.UI
{
    public static class DialogManager
    {
        private static readonly SemaphoreSlim DialogSemaphore = new(1, 1);

        public static async Task<ContentDialogResult> ShowDialogAsync(
            ContentDialog dialog,
            Action? primaryCallback = null,
            Action? secondaryCallback = null,
            Action? closeCallback = null)
        {

            await DialogSemaphore.WaitAsync();
            try
            {
                var result = await dialog.ShowAsync();

                // 执行回调
                switch (result)
                {
                    case ContentDialogResult.Primary: primaryCallback?.Invoke(); break;
                    case ContentDialogResult.Secondary: secondaryCallback?.Invoke(); break;
                    default: closeCallback?.Invoke(); break;
                }

                return result;
            }
            catch (Exception)
            {
                return ContentDialogResult.None;
            }
            finally
            {
                DialogSemaphore.Release();
            }
        }
    }
}
