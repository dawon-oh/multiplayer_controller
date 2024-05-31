using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using Button = System.Windows.Controls.Button;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using MessageBox = System.Windows.MessageBox;
using System.Net;
using System.Windows.Media.Media3D;

namespace PIDController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : System.Windows.Window, INotifyPropertyChanged
    {
        #region - UI Control 데이터 주고 받기
        // ------------- UI Control 데이터 주고 받는 코드 -------------
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool OnPropertyChanged<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return false;

            backingField = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion

        private string _sFileName;

        public string sFileName
        {
            get { return _sFileName; }
            set { OnPropertyChanged(ref _sFileName, value); }
        }

        public class ProcessItem
        {
            public int PId { get; set; }
            public string PName { get; set; }
            public bool status_sel { get; set; }
        }


        List<ProcessItem> process_data = new List<ProcessItem>();
        List<ProcessItem> sel_process_data = new List<ProcessItem>();
        List<Button> sel_process_btn = new List<Button>();
        private int _sel_process_count = 0;
        public int sel_process_count
        {
            get { return _sel_process_count; }
            set { OnPropertyChanged(ref _sel_process_count, value); }
        }


        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            GetProcessList();
         }


        private void Btn_top_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = (ToggleButton)sender;
            NavigationService navigationService = NavigationService.GetNavigationService(this);

            if (toggle.IsChecked == true)
            {
                toggle.Content = "Back";
                // this.WindowState = WindowState.Minimized;

                MiniPage minipage = new MiniPage();
                minipage.Show();
                minipage.Topmost = true;

                this.WindowState = WindowState.Minimized;
            }
            else
            {
                this.Topmost = false;
                toggle.Content = "Minimize";
            }
        }

        // 프로세스 파일 리스트 받아오는 함수 
        public void GetProcessList()
        {
            System.Diagnostics.Process[] AllProcess = System.Diagnostics.Process.GetProcesses();

            for (int i = 0; i < AllProcess.Length; i++)
            {
                bool isWindows = false;
                var hList = GetWindowHandleList((uint)AllProcess[i].Id);
                foreach (IntPtr handle in hList)
                {

                    if (IsWindowVisible(handle) == true) //(IsWindow(handle) == false || IsWindowVisible(handle) == false)
                    {
                        isWindows = true;
                        break;
                    }
                }

                if (isWindows == false)
                    continue;

                try
                {
                    process_data.Add(new ProcessItem { PId = AllProcess[i].Id, PName = AllProcess[i].ProcessName, status_sel = false });
                    ProcessListView.ItemsSource = process_data;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
            ProcessListView.Items.Refresh();

            //for (int i = 0; i < AllProcess.Length; i++)
            //{
            //    if(AllProcess[i].)
            //    try
            //    {
            //        process_data.Add(new ProcessItem { PId = AllProcess[i].Id, PName = AllProcess[i].ProcessName, status_sel = false });
            //        ProcessListView.ItemsSource = process_data;
            //    }
            //    catch (Exception ex)
            //    {
            //        System.Windows.MessageBox.Show(ex.Message);
            //    }
            //}
            //ProcessListView.Items.Refresh();
        }        
        

        // 프로세스 리스트 및 선택된 프로세스 리셋
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {           
            Button btn = sender as Button; 

            if(btn != null)
            {
                process_data.Clear();
                sel_process_data.Clear();
                sel_process_count = 0;
                SelProcessListView.Children.Clear();

                PlayPauseBtn.IsChecked = false;

                GetProcessList();
            }
        }


        // 프로세스 리스트 및 선택된 프로세스 리셋
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null)
            {
                process_data.Clear();
                sel_process_data.Clear();
                sel_process_count = 0;
                SelProcessListView.Children.Clear();

                PlayPauseBtn.IsChecked = false;

                System.Diagnostics.Process[] AllProcess = System.Diagnostics.Process.GetProcesses();

                //for (int i = 0; i < AllProcess.Length; i++)
                //{
                //    if(AllProcess[i].ProcessName == "PotPlayer")
                //    {
                //        try
                //        {
                //            process_data.Add(new ProcessItem { PId = AllProcess[i].Id, PName = AllProcess[i].ProcessName, status_sel = false });
                //            ProcessListView.ItemsSource = process_data;
                //        }
                //        catch (Exception ex)
                //        {
                //            System.Windows.MessageBox.Show(ex.Message);
                //        }
                //    }
                //}

                // 프로세스 중 작업줄에 표시되는 프로그램만 가져오기
                for (int i = 0; i < AllProcess.Length; i++)
                {
                    if (AllProcess[i].MainWindowTitle.Equals(""))
                    {
                        continue;
                    }
                    else
                    {
                        try
                        {
                            process_data.Add(new ProcessItem { PId = AllProcess[i].Id, PName = AllProcess[i].ProcessName, status_sel = false });
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show(ex.Message);
                        }
                    }
                }

                ProcessListView.ItemsSource = process_data;
                ProcessListView.Items.Refresh();
            }
        }


        // 프로세스 클릭시 Select 처리하는 함수 
        private void SelectProcess_Clicked(object sender, RoutedEventArgs e)
        {
            ToggleButton clicked_btn = (ToggleButton)sender;
            var item = (sender as FrameworkElement).DataContext;
            int index = ProcessListView.Items.IndexOf(item);

            if (clicked_btn.IsChecked == true)
            {
                Button btn = new Button();
                btn.Name = "btn_" + process_data[index].PId.ToString();
                btn.Content = process_data[index].PName + "\n" + process_data[index].PId.ToString();
                btn.Width = 80;

                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    SelProcessListView.Children.Add(btn);
                    sel_process_btn.Add(btn);
                    sel_process_count++;

                    sel_process_data.Add(process_data[index]);
                }));
            }
            else
            {
                foreach (Button btn in sel_process_btn)
                {
                    if (btn.Name == "btn_" + process_data[index].PId.ToString())
                    {
                        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            SelProcessListView.Children.Remove(btn);
                            sel_process_btn.Remove(btn);
                            sel_process_count--;
                        }));
                        break;
                    }
                }
            }
        }


        #region - SendMessage

        const uint WM_KEYDOWN = 0x100;
        const uint WM_KEYUP = 0x101;
        const uint WM_CHAR = 0x102;
        const uint WM_ACTIVATE = 0x0006;
        const uint WA_ACTIVE = 1;

        [DllImport("user32")]
        static extern bool IsWindow(IntPtr hWnd);
        [DllImport("user32")]
        public static extern bool IsWindowVisible(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]

        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, ref COPYDATASTRUCT lParam);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern void keybd_event(uint vk, uint scan, uint flags, uint extraInfo);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(int wCode, int wMapType);


        [DllImport("user32")]
        public static extern int GetClassName(int hwnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr parentWindowHandle, IntPtr childAfterWindowHandle, string className, string windowText);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr windowHandle, IntPtr processID);

 
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        private const int MediaPlayPause = 0xB3;

        String GetClassName(IntPtr hWnd)
        {
            StringBuilder sClass = new StringBuilder(100);
            GetClassName((int)hWnd, sClass, 100);
            return sClass.ToString();
        }


        public List<IntPtr> GetWindowHandleList(uint processID)
        {

            List<IntPtr> windowHandleList = new List<IntPtr>();

            IntPtr windowHandle = IntPtr.Zero;

            do
            {
                windowHandle = FindWindowEx(IntPtr.Zero, windowHandle, null, null);

                uint windowProcessID = 0;

                GetWindowThreadProcessId(windowHandle, out windowProcessID);

                if (windowProcessID == processID)
                {
                    windowHandleList.Add(windowHandle);
                }
            }
            while (windowHandle != IntPtr.Zero);

            return windowHandleList;
        }

 
        public void SendButtonClick(object sender, EventArgs e)
        {
            if (sel_process_data.Count > 0)
            {
                COPYDATASTRUCT cds = new COPYDATASTRUCT();
                cds.dwData = IntPtr.Zero;
                cds.cbData = MediaPlayPause;

                foreach (ProcessItem pi in sel_process_data)
                {
                    Process process = Process.GetProcessById(pi.PId);

                    var hList = GetWindowHandleList((uint)pi.PId);
                    foreach (IntPtr handle in hList)
                    {
                        if (IsWindowVisible(handle) == false) //(IsWindow(handle) == false || IsWindowVisible(handle) == false)
                            continue;

                        PostKeyEx(handle, Keys.Space);
                    }

                    //if (hWnd == (IntPtr)0)
                    //    continue;


                    //SendMessage(process.MainWindowHandle, WM_KEYDOWN, (int)Keys.Space, ref cds);
                    //SendMessage(process.MainWindowHandle, WM_CHAR, (int)Keys.Space, ref cds);
                    //SendMessage(process.MainWindowHandle, WM_KEYUP, (int)Keys.Space, ref cds);
                }

                if(PlayPauseBtn.IsChecked == true)
                {
                    Uri logo_resourceUri = new Uri("/Resource/stop_n.png", UriKind.Relative);
                    PlayMovieImage.Source = new BitmapImage(logo_resourceUri);
                }
                else 
                {
                    Uri logo_resourceUri = new Uri("/Resource/play_n.png", UriKind.Relative);
                    PlayMovieImage.Source = new BitmapImage(logo_resourceUri);
                }
            }
            else
            {
                PlayPauseBtn.IsChecked = false;
                Uri logo_resourceUri = new Uri("/Resource/play_n.png", UriKind.Relative);
                PlayMovieImage.Source = new BitmapImage(logo_resourceUri);
            }
        }

        public void PrevButtonClick(object sender, EventArgs e)
        {
            if (sel_process_data.Count > 0)
            {
                COPYDATASTRUCT cds = new COPYDATASTRUCT();
                cds.dwData = IntPtr.Zero;
                cds.cbData = MediaPlayPause;

                foreach (ProcessItem pi in sel_process_data)
                {
                    Process process = Process.GetProcessById(pi.PId);

                    var hList = GetWindowHandleList((uint)pi.PId);
                    foreach (IntPtr handle in hList)
                    {
                        if (IsWindowVisible(handle) == false) //(IsWindow(handle) == false || IsWindowVisible(handle) == false)
                            continue;

                        PostKeyEx(handle, Keys.PageUp);
                    }

                    PlayPauseBtn.IsChecked = true;
                    Uri logo_resourceUri = new Uri("/Resource/stop_n.png", UriKind.Relative);
                    PlayMovieImage.Source = new BitmapImage(logo_resourceUri);
                }
            }
        }

        public void NextButtonClick(object sender, EventArgs e)
        {
            if (sel_process_data.Count > 0)
            {
                COPYDATASTRUCT cds = new COPYDATASTRUCT();
                cds.dwData = IntPtr.Zero;
                cds.cbData = MediaPlayPause;

                foreach (ProcessItem pi in sel_process_data)
                {
                    Process process = Process.GetProcessById(pi.PId);

                    var hList = GetWindowHandleList((uint)pi.PId);
                    foreach (IntPtr handle in hList)
                    {
                        if (IsWindowVisible(handle) == false) //(IsWindow(handle) == false || IsWindowVisible(handle) == false)
                            continue;

                        PostKeyEx(handle, Keys.PageDown);
                    }
                }

                PlayPauseBtn.IsChecked = true;
                Uri logo_resourceUri = new Uri("/Resource/stop_n.png", UriKind.Relative);
                PlayMovieImage.Source = new BitmapImage(logo_resourceUri);
            }
        }

        #endregion


        void PostKeyEx(IntPtr hwnd, params Keys[] args)
        {
            ForceForegroundWindow(hwnd);
            Thread.Sleep(10);

            for(int i = 0; i < args.Length; i++)
            {
                keybd_event((uint)args[i], 0, 0x00, 0);
            }
            for (int i = 0; i < args.Length; i++)
            {
                keybd_event((uint)args[i], 0, 0x02, 0);
            }
        }


        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd,
                out uint lpdwProcessId);


        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        /// The GetForegroundWindow function returns a handle to the 
        /// foreground window.
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool AttachThreadInput(uint idAttach,
            uint idAttachTo, bool fAttach);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(HandleRef hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        [DllImport("user32.dll")]
        static extern IntPtr SetFocus(IntPtr handle);

        private static void ForceForegroundWindow(IntPtr hWnd)
        {
            uint foreThread = GetWindowThreadProcessId(GetForegroundWindow(),
                IntPtr.Zero);
            uint appThread = GetCurrentThreadId();
            const uint SW_SHOW = 5;

            if (foreThread != appThread)
            {
                AttachThreadInput(foreThread, appThread, true);
                BringWindowToTop(hWnd);
                ShowWindow(hWnd, SW_SHOW);
                AttachThreadInput(foreThread, appThread, false);
            }
            else
            {
                BringWindowToTop(hWnd);
                ShowWindow(hWnd, SW_SHOW);
            }
        }


        public static void AttachedThreadInputAction(Action action)
        {
            var foreThread = GetWindowThreadProcessId(GetForegroundWindow(),
                IntPtr.Zero);
            var appThread = GetCurrentThreadId();
            bool threadsAttached = false;

            try
            {
                threadsAttached =
                    foreThread == appThread ||
                    AttachThreadInput(foreThread, appThread, true);

                if (threadsAttached) action();
                else throw new ThreadStateException("AttachThreadInput failed.");
            }
            finally
            {
                if (threadsAttached)
                    AttachThreadInput(foreThread, appThread, false);
            }
        }


        public const uint SW_SHOW = 5;

        ///<summary>
        /// Forces the window to foreground.
        ///</summary>
        ///hwnd”>The HWND.</param>
        public static void ForceWindowToForeground(IntPtr hwnd)
        {
            AttachedThreadInputAction(
                () =>
                {
                    BringWindowToTop(hwnd);
                    ShowWindow(hwnd, SW_SHOW);
                });
        }

        public static IntPtr SetFocusAttached(IntPtr hWnd)
        {
            var result = new IntPtr();
            AttachedThreadInputAction(
                () =>
                {
                    result = SetFocus(hWnd);
                });
            return result;
        }


        #region - Keyboard Hooking

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private LowLevelKeyboardProc _proc = HookProc;

        const int WH_KEYBOARD_LL = 13;
        //const int WH_KEY_DOWN = 0x100;
        // const int WM_KEYDOWN = 256;
        const int WM_SYSTEMDOWN = 260;     

        private static IntPtr hhook = IntPtr.Zero;

        public void SetHook(object sender, RoutedEventArgs e)
        {
            try
            {
                IntPtr hInstance = LoadLibrary("User32");
                hhook = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, hInstance, 0);
            }
            catch (Exception e1)
            {
                int t = 0;
            }

        }

        public static void UnHook()
        {
            UnhookWindowsHookEx(hhook);
        }

        public static IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam)    // lParam : 입력된 키보드 값, 
        {
            if(code >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (vkCode.ToString() == "20")
                    System.Windows.MessageBox.Show("You pressed a CTR");

                return CallNextHookEx(hhook, code, (int)wParam, lParam);
                //return (IntPtr)1;
            }
            else
                return CallNextHookEx(hhook, code, (int)wParam, lParam);

            //if (VKeys.S.Equal(curKey) && IsKeyPress(VKeys.VK_LCONTROL))
            //{
            //    // Left Ctrl + S 일 때 처리할 내용 쓰기
            //}
        }

        private void Form1_Load(object sender, RoutedEvent e)
        {
            //SetHook();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UnHook();
        }

        #endregion
    }






}
