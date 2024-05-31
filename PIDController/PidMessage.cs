using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PIDController
{
    public static class PidMessage
    {
        #region 메시지 보내기 - SendMessage(windowHandle, message, wordParameter, longParameter)

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        #endregion

        #region 마우스 이벤트 발생시키기 - mouse_event(flag, deltaX, deltaY, data, extraInformation)

        [DllImport("user32.dll")]
        private static extern void mouse_event(int flag, int deltaX, int deltaY, int data, UIntPtr extraInformation);

        #endregion

        #region 키보드 이벤트 발생시키기 - mouse_event(flag, deltaX, deltaY, data, extraInformation)

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte vk, byte scan, int flags, ref int extrainfo);

        #endregion



        #region Field

        /// <summary>
        /// WM_SYS_COMMAND
        /// </summary>
        private const int WM_SYS_COMMAND = 0x0112;

        /// <summary>
        /// SC_MONITOR_POWER
        /// </summary>
        private const int SC_MONITOR_POWER = 0xf170;

        /// <summary>
        /// MONITOR_SHUT_OFF
        /// </summary>
        private const int MONITOR_SHUT_OFF = 2;

        /// <summary>
        /// MOUSE_EVENT_FLAG_MOVE
        /// </summary>
        private const int MOUSE_EVENT_FLAG_MOVE = 0x0001;


        /// <summary>
        /// KEY_UP
        /// </summary>
        private const int KEY_UP = 0x0002;


        /// <summary>
        /// KEY_DOWN
        /// </summary>
        private const int KEY_DOWN = 0;


        #endregion

        #region 모니터 전원 끄기 - MonitorPowerOff()

        /// <summary>
        /// 모니터 전원 끄기
        /// </summary>
        public static void MonitorPowerOff()
        {
            SendMessage((IntPtr)0xffff, WM_SYS_COMMAND, (IntPtr)SC_MONITOR_POWER, (IntPtr)MONITOR_SHUT_OFF);
        }

        #endregion


        #region 모니터 전원 끄기 - MonitorPowerOff()

        /// <summary>
        /// 모니터 전원 끄기
        /// </summary>
        public static void PlayMovie()
        {
            const byte SpaceKey = 20;
            int Info = 0;
           
            keybd_event(SpaceKey, 0, KEY_DOWN, ref Info);
            keybd_event(SpaceKey, 0, KEY_UP, ref Info);
        }

        #endregion
    }
}


// Keys.Space
// keydown()


//[DllImport("user32.dll)]
//public static extern void Keybd_event(byte vk, byte scan, int flags, ref int extrainfo);

//※ ex)
//const byte AltKey = 18;
//const int KEYUP = 0x0002;
//int Info = 0;
//keybd_event(AltKey, 0, 0, ref Info); // ALT key 다운
//keybd_event(AltKey, 0, KEYUP, ref Info); // ALT key 업





//VOID keybd_event(
//BYTE bVk, // 가상 키코드
//BYTE bScan, // 하드웨어 스캔 코드
//DWORD dwFlags, // 동작 지정 Flag
//PTR dwExtraInfo // 추가 정보
//);