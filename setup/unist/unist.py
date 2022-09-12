import tkinter as tk
from tkinter import *
import ttkbootstrap as ttk
from ttkbootstrap.constants import *
import os,sys
import winreg
import psutil
import shutil
import tempfile
import logging
import time

logging.basicConfig(level=logging.DEBUG,
                    format='%(asctime)s %(filename)s[line:%(lineno)d] %(levelname)s %(message)s',
                    datefmt='%a, %d %b %Y %H:%M:%S',
                    filename= time.strftime("%Y-%m-%d") + '.log',
                    filemode='w')

if len(sys.argv) == 1:
    tmp_path = tempfile.mkdtemp()
    _dst_file_name = tmp_path + '\\Fu_.exe'
    self_path = os.path.abspath(sys.argv[0])
    shutil.copyfile(self_path, _dst_file_name)
    os.popen('start /b ' + _dst_file_name + ' u')
    sys.exit(0)
logging.debug(os.path.abspath(sys.argv[0]))

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

        self._lab_title = tk.StringVar()
        self._close_png = tk.PhotoImage(file= get_resource_path('./image/close.png'))

        self._frm_top = ttk.Frame(self.root, height=300, width=self._ww, style='Top.TFrame')

        self._lab_title.set('感谢使用 Finance')
        self._btn_unist = ttk.Button(self.root, text='立即卸载', command=self._unist_click)

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

        self._btn_unist.grid(row=1, column=1, pady=10)

        self._frm_top.bind("<Button-1>", self.MouseDown)
        self._frm_top.bind("<B1-Motion>", self.MouseMove)

    def _unist_click(self):
        if self._btn_unist['text'] == '立即卸载':
            try:
                os.popen('net stop FinanceService')
                os.popen('sc delete FinanceService')
            except Exception as e:
                logging.debug(e)

            UnpackProcessReport(40)
            logging.debug('after unist service')

            try:
                self._kill_process('Finance.exe')
                self._kill_process('FinanceClient.exe')
                self._kill_process('FinanceAcountManager.exe')
            except Exception as e:
                logging.debug(e)

            UnpackProcessReport(60)
            logging.debug('after kill process')

            try:
                subkey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Finance"
                installLocation = self._query_reg_value(winreg.HKEY_LOCAL_MACHINE, subkey, 'InstallLocation')
                if installLocation != '':
                    logging.debug(installLocation)
                    shutil.rmtree(installLocation)
            except Exception as e:
                logging.debug(e)

            logging.debug('after rmvtree')
            UnpackProcessReport(80)

            try:
                self._delete_reg_subkey(winreg.HKEY_LOCAL_MACHINE, subkey)
            except Exception as e:
                logging.debug(e)

            UnpackProcessReport(90)
            logging.debug('after delete reg')

            try:
                desk_path = self._query_reg_value(winreg.HKEY_CURRENT_USER, r"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", 'Desktop')
                os.remove(path=desk_path+'\\FinanceClient.lnk')
            except Exception as  ex:
                logging.debug('Exception:', ex)

            logging.debug('after rmv lnk')
            UnpackProcessReport(100)
            self._btn_unist['text'] = '卸载完成'
        else:
            self.root.destroy()

    @staticmethod
    def _query_reg_value(key, sub_key, name):
        ret = ''
        try:
            with winreg.OpenKey(key, sub_key) as reg:
                ret = str(winreg.QueryValueEx(reg, name)[0])
        except Exception as e:
            logging.debug(e)
        return ret

    @staticmethod
    def _delete_reg_subkey(key, sub_key):
        try:
            winreg.DeleteKey(key, sub_key)
        except Exception as e:
            logging.debug(e)

    @staticmethod
    def _kill_process(process_name):
        for proc in psutil.process_iter():
            if proc.name() == process_name:
                proc.kill()
                return

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