using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ImTools;
using ScriptGraphicHelper.Models;
using ScriptGraphicHelper.ViewModels;
using Vanara.PInvoke;
using static System.Environment;

namespace ScriptGraphicHelper.Views
{
    /// <summary>
    /// GetHwd.xaml 的交互逻辑
    /// </summary>
    public partial class GetHwnd : Window
    {

        public GetHwnd()
        {
            InitializeComponent();
        }
        ObservableCollection<MovieCategory> MovieCategories = new ObservableCollection<MovieCategory>();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Resources/selectHwd.cur"))
            {
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Resources"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Resources");
                }
                var data = (byte[])Properties.Resources.ResourceManager.GetObject("selectHwd");
                var stream = new MemoryStream(data);
                var fileStream = File.Create(AppDomain.CurrentDomain.BaseDirectory + @"Resources/selectHwd.cur");
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
                stream.Close();
                fileStream.Close();
                stream.Dispose();
                fileStream.Dispose();
            }
            HwndTree.ItemsSource = MovieCategories;
        }

        private void HwndTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            MovieCategory movie = (MovieCategory)HwndTree.SelectedItem;
            if (movie != null)
            {
                BindHwnd.Text = movie.Hwnd.ToString();
                BindTitle.Text = movie.Title.ToString();
                BindClassName.Text = movie.ClassName.ToString();
                ResultHwndTitle = movie.Title ?? movie.ClassName ?? movie.Hwnd.ToString();
            }
        }
        public int ResultHwnd { get; set; } = -1;
        public string ResultHwndTitle { get; set; }
        public int ResultGraphicMode { get; set; } = -1;
        public int ResultAttribute { get; set; } = -1;
        public int ResultMode { get; set; } = -1;
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (BindHwnd.Text != "")
            {
                ResultHwnd = int.Parse(BindHwnd.Text);
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("请先选择绑定句柄!");
            }
        }

        private void StackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Size size = e.NewSize;
            //Height = SystemParameters.CaptionHeight + size.Height + 25;
        }

        private class NativeMethods
        {
            [DllImport("user32", CharSet = CharSet.Unicode)]
            internal static extern IntPtr LoadCursorFromFile(string fileName);

            [DllImport("User32.DLL")]
            internal static extern bool SetSystemCursor(IntPtr hcur, uint id);
            internal const uint OCR_NORMAL = 32512;

            [DllImport("User32.DLL")]
            internal static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

            internal const uint SPI_SETCURSORS = 87;
            internal const uint SPIF_SENDWININICHANGE = 2;

        }



        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IntPtr cur = NativeMethods.LoadCursorFromFile(AppDomain.CurrentDomain.BaseDirectory + @"Resources/selectHwd.cur");
            NativeMethods.SetSystemCursor(cur, NativeMethods.OCR_NORMAL);
        }

        private void EnumWindows(nint parentHwd, MovieCategory movieCategory)
        {
            List<nint> hwnds = WindApi.EnumWindow(parentHwd);
            for (int i = 0; i < hwnds.Count; i++)
            {
                var hwnd = hwnds[i];
                if (hwnd != 0)
                {
                    movieCategory.Movies.Add(new MovieCategory(hwnd, WindApi.GetWindowTitle(hwnd), WindApi.GetWindowClass(hwnd)));
                    EnumWindows(hwnd, movieCategory.Movies[i]);
                }
            }
        }

        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            MovieCategories.Clear();
            NativeMethods.SystemParametersInfo(NativeMethods.SPI_SETCURSORS, 0, IntPtr.Zero, NativeMethods.SPIF_SENDWININICHANGE);
            nint parentHwnd = WindApi.GetMousePointWindow();
            MovieCategories.Add(new MovieCategory(parentHwnd, WindApi.GetWindowTitle(parentHwnd), WindApi.GetWindowClass(parentHwnd)));
            EnumWindows(parentHwnd, MovieCategories[0]);
            var movie = MovieCategories.FirstOrDefault();
            if (movie != null)
            {
                BindHwnd.Text = movie.Hwnd.ToString();
                BindTitle.Text = movie.Title.ToString();
                BindClassName.Text = movie.ClassName.ToString();
                ResultHwndTitle = movie.Title ?? movie.ClassName ?? movie.Hwnd.ToString();
            }
        }
    }
}
