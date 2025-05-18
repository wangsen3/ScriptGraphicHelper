using ScriptGraphicHelper.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace ScriptGraphicHelper.Models
{
    namespace ScriptGraphicHelper.Models
    {
        class HwndHelper : EmulatorHelper
        {
            public override string Path { get; set; } = "句柄";
            public override string Name { get; set; } = "句柄";

            private int Hwd = -1;
            private bool IsInit = false;
            public override void Dispose()
            {

            }
            public override async Task<Bitmap> ScreenShot(int Index)
            {
                if (Index == -1 || !IsInit)
                {
                    MessageBox.Show("请先选择窗口句柄!");
                    return new Bitmap(1, 1);
                }
                var task = Task.Run(() =>
                {
                    try
                    {
                        return WindApi.ScreenShot(Hwd);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        return new Bitmap(1, 1);
                    }
                });
                return await task;
            }
            public override async Task<List<KeyValuePair<int, string>>> ListAll()
            {
                GetHwnd getHwnd = new GetHwnd();
                if ((bool)getHwnd.ShowDialog())
                {
                    Hwd = getHwnd.ResultHwnd;
                    IsInit = true;
                }
                return [new KeyValuePair<int, string>(key: 0, value: getHwnd.ResultHwndTitle)];
            }

            public override bool IsStart(int Index)
            {
                return IsInit;
            }
        }
    }

}
