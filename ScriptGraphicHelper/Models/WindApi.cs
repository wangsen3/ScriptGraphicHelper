using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using ScriptGraphicHelper.Models;
using Vanara.PInvoke;

public static class WindApi
{

    public static string GetWindowTitle(nint hwnd)
    {
        StringBuilder stringBuilder = new(512);
        User32.GetWindowText(hwnd, stringBuilder, stringBuilder.Capacity);
        return stringBuilder.ToString();
    }
    public static (int x, int y, int width, int height) GetClientSize(nint hwnd)
    {
        if (User32.GetClientRect(hwnd, out RECT rect))
        {
            return (rect.X, rect.Y, rect.Width, rect.Height);
        }
        return (0, 0, 0, 0);
    }

    public static (int x, int y, int width, int height) GetWindowRect(nint hwnd)
    {
        if (User32.GetWindowRect(hwnd, out RECT rect))
        {
            return (rect.X, rect.Y, rect.Width, rect.Height);
        }
        return (0, 0, 0, 0);
    }
    public static List<nint> EnumWindow(nint parent)
    {
        List<nint> hwnds = [];
        User32.EnumWindowsProc enumWindowsProc = (hwnd, args) =>
        {
            if (hwnd.IsNull)
            {
                return false;
            }
            hwnds.Add(hwnd.DangerousGetHandle());
            return true;
        };
        User32.EnumChildWindows(parent, enumWindowsProc, 0);
        //User32.EnumWindows(enumWindowsProc, 0);
        return hwnds;
    }

    public static nint GetParentWindow(nint hwnd)
    {
        var parent = User32.GetWindow(hwnd, 3);
        return parent.DangerousGetHandle();
    }

    public static string GetWindowClass(nint hwnd)
    {
        StringBuilder stringBuilder = new(512);
        User32.GetClassName(hwnd, stringBuilder, stringBuilder.Capacity);
        return stringBuilder.ToString();
    }

    public static Bitmap ScreenShot(nint hwnd)
    {
        (int x, int y, int width, int height) = GetClientSize(hwnd);
        (x, y, width, height) = GetWindowRect(hwnd);
        var title = GetWindowTitle(hwnd);
        var className = GetWindowClass(hwnd);
        Bitmap bitmap = new Bitmap(width, height);
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(width, height));
        }
        return bitmap;
    }

    public static nint GetMousePointWindow()
    {
        User32.GetCursorPos(out POINT pt);
        var hwnd = User32.WindowFromPoint(pt);
        return hwnd.DangerousGetHandle();
    }


}
