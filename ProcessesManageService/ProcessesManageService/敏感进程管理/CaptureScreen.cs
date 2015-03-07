using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ProcessesManageService.SensitiveProcessesManage
{
    //图像管理
    class GDI32
    {
        [DllImport("GDI32.dll")]
        public static extern bool BitBlt(int hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, int hdcSrc, int nXSrc, int nYSrc, int dwRop);
        [DllImport("GDI32.dll")]
        public static extern int CreateCompatibleBitmap(int hdc, int nWidth, int nHeight);
        [DllImport("GDI32.dll")]
        public static extern int CreateCompatibleDC(int hdc);
        [DllImport("GDI32.dll")]
        public static extern bool DeleteDC(int hdc);
        [DllImport("GDI32.dll")]
        public static extern bool DeleteObject(int hObject);
        [DllImport("GDI32.dll")]
        public static extern int GetDeviceCaps(int hdc, int nIndex);
        [DllImport("GDI32.dll")]
        public static extern int SelectObject(int hdc, int hgdiobj);
    }
    //桌面信息API
    class User32
    {
        [DllImport("User32.dll")]
        public static extern int GetDesktopWindow();
        [DllImport("User32.dll")]
        public static extern int GetWindowDC(int hWnd);
        [DllImport("User32.dll")]
        public static extern int ReleaseDC(int hWnd, int hDC);
    }
    //截屏类
    class CaptureScreen
    {
        //抓屏
        public static void CaptureScreenNow(string filePath)
        {
            int hdcSrc = User32.GetWindowDC(User32.GetDesktopWindow()),
            hdcDest = GDI32.CreateCompatibleDC(hdcSrc),
            hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc,
            GDI32.GetDeviceCaps(hdcSrc, 8), GDI32.GetDeviceCaps(hdcSrc, 10)); GDI32.SelectObject(hdcDest, hBitmap);
            GDI32.BitBlt(hdcDest, 0, 0, GDI32.GetDeviceCaps(hdcSrc, 8),
            GDI32.GetDeviceCaps(hdcSrc, 10), hdcSrc, 0, 0, 0x00CC0020);
            SaveImageAs(hBitmap,filePath, ImageFormat.Jpeg);
            Cleanup(hBitmap, hdcSrc, hdcDest);
        }
        //清屏
        private static void Cleanup(int hBitmap, int hdcSrc, int hdcDest)
        {
            User32.ReleaseDC(User32.GetDesktopWindow(), hdcSrc);
            GDI32.DeleteDC(hdcDest);
            GDI32.DeleteObject(hBitmap);
        }
        //保存图片文件
        private static void SaveImageAs(int hBitmap, string filePath, ImageFormat imageFormat)
        {
            Bitmap image =
            new Bitmap(Image.FromHbitmap(new IntPtr(hBitmap)),
            Image.FromHbitmap(new IntPtr(hBitmap)).Width,
            Image.FromHbitmap(new IntPtr(hBitmap)).Height);
            image.Save(filePath, imageFormat);
        }
    }
}
