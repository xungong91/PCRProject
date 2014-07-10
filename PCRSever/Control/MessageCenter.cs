using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PCRSever.Control
{
    public class MessageCenter
    {
        public static void SendMsg(MsgType _type_,out byte[] data, out int length)
        {
            data = new byte[3];
            data[0] = Convert.ToByte((int)_type_);
            short len = 0;
            byte[] buffer = new byte[2];
            buffer = BitConverter.GetBytes(len);
            data[1] = buffer[0];
            data[2] = buffer[1];
            length = 3;
        }
        public static void SendMsg(MsgType _type_, out byte[] data, out int length, IPEndPoint ip)
        {
            string strip = ip.ToString();
            length = 3 + strip.Length;
            data = new byte[length];
            data[0] = Convert.ToByte((int)_type_);
            short len = (short)strip.Length;
            byte[] buffer = new byte[2];
            buffer = BitConverter.GetBytes(len);
            data[1] = buffer[0];
            data[2] = buffer[1];
            for (int i = 0; i < length-3; i++)
            {
                data[i+3] = (byte)strip[i];
            }
        }
        public static void SendMsg(MsgType _type_, out byte[] data, out int length, string s)
        {
            s += "\0";
            length = 3 + s.Length;
            data = new byte[length];
            data[0] = Convert.ToByte((int)_type_);
            short len = (short)s.Length;
            byte[] buffer = new byte[2];
            buffer = BitConverter.GetBytes(len);
            data[1] = buffer[0];
            data[2] = buffer[1];
            for (int i = 0; i < length - 3; i++)
            {
                data[i + 3] = (byte)s[i];
            }
        }
        public static void ReciveMsg(byte[] data,int length,PCRSever.Interface.IPCRSocket socketmsg,IPEndPoint ip)
        {
            MsgType type = (MsgType)Convert.ToInt32(data[0]);
            int msglength;
            msglength = (int)(data[1] | data[2] << 8);
            byte[] msg=new byte[msglength];
            Array.Copy(data, 3, msg, 0, msglength);
            switch (type)
            {
                case MsgType.FUNCTION_SHUTDOWN:
                    SystemCMDHelper.Singleton().ShutDown();
                    socketmsg.SocketMessage("收到关机命令，在10秒后关机");
                    break;
                case MsgType.FUNCTION_CANCELSHUTDOWN:
                    SystemCMDHelper.Singleton().Cancel_ShutDown();
                    socketmsg.SocketMessage("收到取消命令，会结束关机，睡眠，重启命令");
                    break;
                case MsgType.FUNCTION_RESTART:
                    SystemCMDHelper.Singleton().Restart();
                    socketmsg.SocketMessage("收到重启命令，会在10秒后重启");
                    break;
                case MsgType.FUNCTION_DORMANCY:
                    SystemCMDHelper.Singleton().Dormancyd();
                    socketmsg.SocketMessage("收到睡眠命令，会在10秒后睡眠");
                    break;
                case MsgType.FUNCTION_AUDIO_MUTE:
                    AudioHepler.Mute();
                    socketmsg.SocketMessage("收到静音命令");
                    break;
                case MsgType.FUNCTION_AUDIO_UP:
                    AudioHepler.VolumeDown();
                    socketmsg.SocketMessage("收到减小音量命令");
                    break;
                case MsgType.FUNCTION_AUDIO_DOWN:
                    AudioHepler.VolumeUp();
                    socketmsg.SocketMessage("收到增加音量命令");
                    break;
                case MsgType.FUNCTION_KEY_DOWN:
                    KeyboardHelper.Singleton().KeyDown(data[3]);
                    socketmsg.SocketMessage("收到按键按下");
                    break;
                case MsgType.FUNCTION_KEY_UP:
                    KeyboardHelper.Singleton().KeyUp(data[3]);
                    socketmsg.SocketMessage("收到按键谈起");
                    break;
                case MsgType.FUNCTION_ESC:
                    ProcessHelper.Singletion().closeProc();
                    socketmsg.SocketMessage("收到关闭当前程序命令");
                    break;
                case MsgType.FUNCTION_SCHALT:
                    EscProgram();
                    socketmsg.SocketMessage("收到切换程序命令");
                    break;
                case MsgType.FUNCTION_PPT_UP:
                    KeyboardHelper.Singleton().EasyDown(0x25);
                    KeyboardHelper.Singleton().EasyUp(0x25);
                    socketmsg.SocketMessage("PPT上页");
                    break;
                case MsgType.FUNCTION_PPT_DOWN:
                    KeyboardHelper.Singleton().EasyDown(0x27);
                    KeyboardHelper.Singleton().EasyUp(0x27);
                    socketmsg.SocketMessage("PPT下页");
                    break;
                case MsgType.FUNCTION_PPT_FULLSCREEN:
                    PPTFULL();
                    socketmsg.SocketMessage("PPT全屏");
                    break;
                case MsgType.FUNCTION_PPT_QUITSCREEN:
                    PPTQUIR();
                    socketmsg.SocketMessage("PPT退出全屏");
                    break;
                case MsgType.FUNCTION_CAMERA_START:
                    CameraHelper.Singleton().StartCamera();
                    socketmsg.SocketMessage("请求打开摄像头");
                    break;
                case MsgType.FUNCTION_CAMERA_CLOSE:
                    CameraHelper.Singleton().CloseCamera();
                    socketmsg.SocketMessage("请求关闭摄像头");
                    break;
                case MsgType.FUNCTION_CAMERA_SENDIMAHE:
                    CameraHelper.Singleton().SendImage(ip);
                    socketmsg.SocketMessage("请求发送图片到ip:"+ip.ToString());
                    break;
                case MsgType.FUNCTION_CAMERA_NOTIMAGE:
                    CameraHelper.Singleton().NotSendImage();
                    socketmsg.SocketMessage("请求停止发送图片");
                    break;
                case MsgType.FUNCTION_CAMERA_DESTTOP:
                    
                    break;
                case MsgType.FUNCTION_MUSIC_OPEN:
                    socketmsg.SocketMessage("请求打开默认音乐播放器");
                    Process.Start("explorer.exe", string.Format("{0}Shortcuts\\{1}",AppDomain.CurrentDomain.BaseDirectory,"KuGou.lnk"));
                    break;
                case MsgType.FUNCTION_MUSIC_MINI:
                    socketmsg.SocketMessage("请求迷你化播放器");
                    ComboKey3(0x11,0x12,0x4d);
                    break;
                case MsgType.FUNCTION_MUSIC_LYC:
                    socketmsg.SocketMessage("显示/关闭歌词");
                    ComboKey3(0x11,0x12,0x44);
                    break;
                case MsgType.FUNCTION_MUSIC_PLAY:
                    socketmsg.SocketMessage("请求播放音乐");
                    ComboKey2(0x12,0x74);
                    break;
                case MsgType.FUNCTION_MUSIC_NOTAUDIO:
                    socketmsg.SocketMessage("请求静音");
                    ComboKey3(0x11,0x12,0x53);
                    break;
                case MsgType.FUNCTION_MUSIC_DOWNAUDIO:
                    socketmsg.SocketMessage("减小音量");
                    ComboKey2(0x12,0x28);
                    break;
                case MsgType.FUNCTION_MUSIC_UPAUDIO:
                    socketmsg.SocketMessage("增加音量");
                    ComboKey2(0x12,0x26);
                    break;
                case MsgType.FUNCTION_MUSIC_NEXTSONG:
                    socketmsg.SocketMessage("下一首歌");
                    ComboKey2(0x12,0x27);
                    break;
                case MsgType.FUNCTION_MUSIC_ONSONG:
                    socketmsg.SocketMessage("上一首歌");
                    ComboKey2(0x12,0x25);
                    break;
                default:
                    break;
            }
        }
        private static void PPTFULL()
        {
            ProcessHelper.Singletion().RaiseOtherProcess("POWERPNT");
            KeyboardHelper.Singleton().EasyDown(0x10);
            KeyboardHelper.Singleton().EasyDown(0x74);
            KeyboardHelper.Singleton().EasyUp(0x74);
            KeyboardHelper.Singleton().EasyUp(0x10);
        }
        private static void PPTQUIR()
        {
            ProcessHelper.Singletion().RaiseOtherProcess("POWERPNT");
            KeyboardHelper.Singleton().EasyDown(0x1b);
            KeyboardHelper.Singleton().EasyUp(0x1b);
        }
        private static void EscProgram()
        {
            KeyboardHelper.Singleton().EasyDown(0x12);
            KeyboardHelper.Singleton().EasyDown(0x09);
            KeyboardHelper.Singleton().EasyUp(0x09);
            KeyboardHelper.Singleton().EasyUp(0x12);
        }
        private static void ComboKey2(byte key1, byte key2)
        {
            KeyboardHelper.Singleton().EasyDown(key1);
            KeyboardHelper.Singleton().EasyDown(key2);
            KeyboardHelper.Singleton().EasyUp(key2);
            KeyboardHelper.Singleton().EasyUp(key1);
        }
        private static void ComboKey3(byte key1, byte key2,byte key3)
        {
            KeyboardHelper.Singleton().EasyDown(key1);
            KeyboardHelper.Singleton().EasyDown(key2);
            KeyboardHelper.Singleton().EasyDown(key3);
            KeyboardHelper.Singleton().EasyUp(key3);
            KeyboardHelper.Singleton().EasyUp(key2);
            KeyboardHelper.Singleton().EasyUp(key1);
        }
    }
}
