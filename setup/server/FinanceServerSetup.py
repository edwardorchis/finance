import socket
import time
import tkinter as tk
from tkinter import *
from tkinter import filedialog
from tkinter import messagebox
import webbrowser
import winreg
import winshell
import ttkbootstrap as ttk
from ttkbootstrap.constants import *
import pymssql
import ctypes
import os,sys
import xml.dom.minidom as minidom

os.add_dll_directory(os.path.abspath("."))
def get_resource_path(relative_path):
    if getattr(sys, 'frozen', False):
        base_path = sys._MEIPASS
    else:
        base_path = os.path.abspath('.')
    return os.path.join(base_path, relative_path)

class WinMain(Frame):
    def __init__(self, parent=None):
        self.root = parent
        self.background = '#810102'
        self._ww = 600
        self._wh = 400

        self._s = ttk.Style()
        self._s.configure('Top.TFrame', background=self.background, borderwidth=0)
        self._s.configure('Close.TButton', background=self.background, borderwidth=0)
        self._s.configure('Bottom.TFrame', borderwidth=1)

        self._dst_path = tk.StringVar()
        self._lab_title = tk.StringVar()
        self._close_png = tk.PhotoImage(file= get_resource_path('./image/close.png'))

        self._frm_bottom = ttk.Frame(self.root, height=50, width=self._ww, style='Bottom.TFrame')
        self._frm_top = ttk.Frame(self.root, height=300, width=self._ww, style='Top.TFrame')
        self._frm_connect = ttk.Frame(self.root, height=100, width=self._ww, style='Bottom.TFrame')
        self._frm_serv_url = ttk.Frame(self.root, height=100, width=self._ww, style='Bottom.TFrame')

        self._dst_path.set("D:\\Program Files\\Finance")
        self._lab_title.set('欢迎使用 Finance Service')
        self._btn_setup = ttk.Button(self.root, text='立即安装', command=self._setup_click)

        self.processbar = ttk.Progressbar(self.root, orient=HORIZONTAL, length=self._ww, mode='determinate')

        self._init_windows()

    def _init_windows(self):
        # 不显示边框
        self.root.overrideredirect(True)

        # 窗口位于屏幕中间
        screenWidth = self.root.winfo_screenwidth()
        screenHeight = self.root.winfo_screenheight()
        self.root.geometry('%dx%d+%d+%d' % (self._ww, self._wh, (screenWidth - self._ww) / 2, (screenHeight - self._wh) / 2 - 200))

        # 不能调整长宽高
        self.root.resizable(width=False, height=False)

        self._frm_top.grid(row=0, columnspan=3)
        ttk.Label(self._frm_top, textvariable=self._lab_title, font=tk.font.Font(size=18, weight='bold'), foreground='white',
                  background=self.background).place(x=35, y=50)

        ttk.Button(self._frm_top, command=self.root.destroy, image=self._close_png,
                   style='Close.TButton').place(x=(self._ww - 35), y=0)

        self._btn_setup.grid(row=1, column=1, pady=10)

        self._frm_bottom.grid(row=2, columnspan=3)
        ttk.Label(self._frm_bottom, text='安装路径').grid(row=2, column=0)
        ttk.Entry(self._frm_bottom, textvariable=self._dst_path, width=60).grid(row=2, column=1)
        ttk.Button(self._frm_bottom, text='浏览', command=self._sel_path).grid(row=2, column=2)

        self._frm_top.bind("<Button-1>", self.MouseDown)
        self._frm_top.bind("<B1-Motion>", self.MouseMove)

    def _sel_path(self):
        p = filedialog.askdirectory(initialdir=self._dst_path.get())
        if p != '':
            self._dst_path.set(p)

    def _setup_click(self):
        # 环境检查
        if not self._pre_check():
            return

        self._btn_setup.destroy()
        self._frm_bottom.destroy()

        # 解压文件
        self._unpack()

        # 配置参数
        self._set_config()

    def _run_click(self):
        print('"' + self._dst_path.get() + '\\server\\FinanceClient.exe"')
        os.popen('"' + self._dst_path.get() + '\\server\\FinanceClient.exe"')
        # 运行主程序
        self.root.destroy()

    @staticmethod
    def _query_reg_value(key, sub_key, name):
        ret = ''
        try:
            with winreg.OpenKey(key, sub_key) as reg:
                ret = str(winreg.QueryValueEx(reg, name)[0])
        except Exception as e:
            print(e)
        return ret

    # 1、.net依赖包
    def _pre_check(self):
        net_ver = self._query_reg_value(winreg.HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full", 'Version')
        if net_ver > '4.6':
            return True

        if messagebox.showerror('安装前检查失败',
                                '需要安装.net framework 4.6， 请去微软官网下载安装：https://dotnet.microsoft.com/zh-cn/download/dotnet-framework/net46'):
            webbrowser.open('https://dotnet.microsoft.com/zh-cn/download/dotnet-framework/net46')
        return False

    # 1、数据库连接
    # 2、服务端对外服务ip：port
    # 3、客户端服务端连接
    def _set_config(self):
        # 检查config文件是否存在
        if (os.path.exists(self._dst_path.get() + "\\server\\Finance.exe.config") and
            os.path.exists(self._dst_path.get() + "\\server\\FinanceClient.exe.config")):
            self._set_connect()
            return True
        else:
            messagebox.showerror('安装失败', '安装失败，请重新安装')
            self.root.destory()
            return False

    def _set_connect(self):
        self._frm_top.config(height=250)
        self._frm_connect.grid(row=1, columnspan=3)

        data_source = tk.StringVar()
        user_id = tk.StringVar()
        password = tk.StringVar()

        data_source.set('127.0.0.1')
        user_id.set('sa')
        password.set('123456')

        ttk.Label(self._frm_connect, text='数据源').grid(row=1, column=0, pady=5, sticky=(W))
        ttk.Entry(self._frm_connect, textvariable=data_source, width=30).grid(row=1, column=1, padx=5, pady=5)
        ttk.Label(self._frm_connect, text='用户ID').grid(row=2, column=0, pady=2, sticky=(W))
        ttk.Entry(self._frm_connect, textvariable=user_id).grid(row=2, column=1, padx=5,pady=2, sticky=(W))
        ttk.Label(self._frm_connect, text='密码').grid(row=3, column=0, pady=2, sticky=(W))
        ttk.Entry(self._frm_connect, textvariable=password, show='*').grid(row=3, column=1, padx=5,pady=2, sticky=(W))

        ttk.Button(self._frm_connect, text='下一步',
                   command=lambda:self._test_connect(data_source.get(), user_id.get(), password.get())).grid(row=3, column=3, pady=2)

    def _test_connect(self, data_source, user_id, password):
        try:
            print(data_source, user_id, password)
            cn = pymssql.connect(data_source, user_id, password, 'master')
            if cn:
                cn.autocommit(True)
                cn.cursor().execute("if db_id('finance') is null create database finance")
                cn.close()
                self._update_config('connectionStrings', 'Data Source='+ data_source +';Initial Catalog=finance;User ID='+ user_id +';Password=' + password)
                self._frm_connect.destroy()
                self._set_server_url()
                return True
        except Exception as e:
            print(e)
        if messagebox.askokcancel('数据库连接失败', '是否确定安装完成后手工配置'):
            self._frm_connect.destroy()
            self._set_server_url()
            return True
        return False

    def _set_server_url(self):
        self._frm_top.config(height=300)
        self._frm_serv_url.grid(row=1, columnspan=3)

        server_url = StringVar()
        server_url.set('http://'+ socket.gethostbyname(socket.gethostname()) + ':9000')

        ttk.Label(self._frm_serv_url, text='服务端URL').grid(row=1, column=0, pady=20)
        ttk.Entry(self._frm_serv_url, textvariable=server_url, width=30).grid(row=1, column=1, pady=20)
        ttk.Button(self._frm_serv_url, text='下一步', command=lambda:self._set_finish(server_url.get())).grid(row=1, column=3, pady=20)

    def _set_uninstall(self):
        try:
            subkey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Finance"
            reg = winreg.CreateKeyEx(winreg.HKEY_LOCAL_MACHINE, subkey)
            winreg.SetValueEx(reg, 'DisplayName', 0, winreg.REG_SZ, 'Finance')
            winreg.SetValueEx(reg, 'DisplayIcon', 0, winreg.REG_SZ, self._dst_path.get() + '\\server\\FinanceClient.exe')
            winreg.SetValueEx(reg, 'DisplayVersion', 0, winreg.REG_SZ, '2.0')
            winreg.SetValueEx(reg, 'Publisher', 0, winreg.REG_SZ, 'Orchis')
            winreg.SetValueEx(reg, 'InstallLocation', 0, winreg.REG_SZ, self._dst_path.get())
            winreg.SetValueEx(reg, 'UninstallString', 0, winreg.REG_SZ, self._dst_path.get() + '\\server\\unist.exe')
        except Exception as e:
            print(e)
        finally:
            winreg.CloseKey(reg)

    def _create_desktop_link(self):
        desk_path = self._query_reg_value(winreg.HKEY_CURRENT_USER, r"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", 'Desktop')
        try:
            target_exe = self._dst_path.get()+'\\server\\FinanceClient.exe'
            winshell.CreateShortcut(Path=desk_path+'\\FinanceClient.lnk', Target=target_exe, Icon=(target_exe, 0), Description='FinanceClient')
        except Exception as e:
            print(e)

    def _set_finish(self, server_url):
        self._update_config('server_url', server_url)
        self._frm_serv_url.destroy()
        self._lab_title.set('正在启动服务')
        self.root.update()
        register_cmd = 'sc create FinanceService binPath= "'+self._dst_path.get() + '\\server\\Finance.exe" start= auto > FinanceServiceReg.txt'
        ret = os.system(register_cmd)
        if ret != 0:
            if ret == 1073: #已存在
                os.system('net stop FinanceService')
                os.system('sc delete FinanceService')
                ret = os.system(register_cmd)
            if ret != 0:
                messagebox.showerror('安装失败', '注册服务异常：' + str(ret) + '，请打开FinanceServiceReg.txt查看详细错误！')
                self.root.destroy()
                return
        os.popen('net start FinanceService')

        self._set_uninstall()
        self._create_desktop_link()

        ttk.Button(self.root, text='立即运行', command=self._run_click).grid(row=1, column=1, pady=20)
        self._lab_title.set('安装完成')

    def _update_config_app_settings(self, file, key,  value):
        print('_update_config_app_settings')
        with open(file, "r", encoding="utf-8") as f:
            doc = minidom.parse(f)
            root_node = doc.documentElement
            app_set = root_node.getElementsByTagName('appSettings')[0]
            add_s = app_set.getElementsByTagName('add')
            for k in add_s:
                if (k.getAttribute('key') == key):
                    k.setAttribute('value', value)
                    break
        with open(file, 'w', encoding='utf-8') as f:
            print('write _update_config_app_settings')
            doc.writexml(f)

    def _update_config_connection_strings(self, file, name,  value):
        print('_update_config_connection_strings')
        with open(file, "r", encoding="utf-8") as f:
            doc = minidom.parse(f)
            root_node = doc.documentElement
            app_set = root_node.getElementsByTagName('connectionStrings')[0]
            add_s = app_set.getElementsByTagName('add')
            for k in add_s:
                if (k.getAttribute('name') == name):
                    k.setAttribute('connectionString', value)
                    break
        with open(file, 'w', encoding='utf-8') as f:
            print('write _update_config_connection_strings')
            doc.writexml(f)

    def _update_config(self, key, value):
        server_config_file = self._dst_path.get() + "\\server\\Finance.exe.config"
        client_config_file = self._dst_path.get() + "\\server\\FinanceClient.exe.config"
        if (key == 'server_url'):
            self._update_config_app_settings(server_config_file, 'server_url', value)
            self._update_config_app_settings(client_config_file, 'service_url', value)
        elif(key == 'connectionStrings'):
            self._update_config_connection_strings(server_config_file, 'default', value)

    def _unpack(self):
        self.processbar.grid(row=1, column=0, columnspan=3)
        if getattr(sys, 'frozen', False):
            libcos = ctypes.CDLL('libcos.dll')
            pSrcFile = ctypes.c_char_p()
            pSrcFile.value = sys.argv[0].encode('utf-8')
            pDstPath = ctypes.c_char_p(self._dst_path.get().encode('utf-8'))

            PyCallbackFunc = ctypes.WINFUNCTYPE(None, ctypes.c_int)      #定义函数类型
            libcos.Unpack(pSrcFile, pDstPath, PyCallbackFunc(UnpackProcessReport))
        else:
            self._test_step()
        self.processbar.destroy()

    def _test_step(self):
        for i in range(2):
            self.processbar['value'] += 50
            self.root.update()
            time.sleep(1)

    @staticmethod
    def MouseDown(event): # 不要忘记写参数event
        global mousX  # 全局变量，鼠标在窗体内的x坐标
        global mousY  # 全局变量，鼠标在窗体内的y坐标
        mousX=event.x  # 获取鼠标相对于窗体左上角的X坐标
        mousY=event.y  # 获取鼠标相对于窗左上角体的Y坐标

    @staticmethod
    def MouseMove(event):
        window.geometry(f'+{event.x_root - mousX}+{event.y_root - mousY}') # 窗体移动代码

window = ttk.Window()
win_main = WinMain(window)

def UnpackProcessReport(percent):
    win_main.processbar['value'] = percent
    window.update()

window.mainloop()