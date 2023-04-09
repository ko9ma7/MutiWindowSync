﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using mutiWindowSync.config;
using mutiWindowSync.DmSoft;
using mutiWindowSync.service.IService;

namespace mutiWindowSync.service
{
    public class DmService : IDmService
    {
        private DmSoftCustomClassName dmSoft;

        public DmService()
        {
            if (Environment.Is64BitProcess)
            {
                Debug.WriteLine("这是 64 位程序");
                Debug.WriteLine("按任意键结束程序");
                // Console.ReadKey();
                return;
            }

            // 免注册调用大漠插件
            var registerDmSoftDllResult = RegisterDmSoft.RegisterDmSoftDll();
            Debug.WriteLine($"免注册调用大漠插件返回：{registerDmSoftDllResult}");
            if (!registerDmSoftDllResult)
            {
                throw new Exception("免注册调用大漠插件失败");
            }

            // 创建对象
            dmSoft = new DmSoftCustomClassName();

            // 收费注册
            // var regResult = dmSoft.Reg(DmConfig.DmRegCode, DmConfig.DmVerInfo);
            // Debug.WriteLine($"收费注册返回：{regResult}");
            // if (regResult != 1)
            // {
            //     throw new Exception("收费注册失败");
            // }

            // 判断 Resources 是否存在，不存在就创建
            if (!Directory.Exists(DmConfig.DmGlobalPath))
            {
                Directory.CreateDirectory(DmConfig.DmGlobalPath);
            }

            // 设置全局路径,设置了此路径后,所有接口调用中,相关的文件都相对于此路径. 比如图片,字库等
            dmSoft.SetPath(DmConfig.DmGlobalPath);
        }

        public async void capture()
        {
            var hwnd = 591738;
            if (dmSoft.IsBind(hwnd) == 0)
            {
                var BindWinRes = dmSoft.BindWindowEx(hwnd, "normal", "normal", "windows", "", 0);
                if (BindWinRes != 1)
                {
                    Debug.WriteLine("绑定窗口石板!!!!!");
                    return;
                }
            }

            dmSoft.KeyPress(49);

            // dmSoft.UnBindWindow();

            // 抓取指定区域(x1, y1, x2, y2)的图像,保存为file(24位位图)
            // var captureResult = dmSoft.Capture(0, 0, 2000, 2000, "screen.bmp");
            // Debug.WriteLine($"Capture 返回：{captureResult}");
            // if (captureResult != 1)
            // {
            // throw new Exception("Capture 失败");
            // }
        }
    }
}