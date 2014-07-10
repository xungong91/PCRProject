using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCRSever
{
    public enum MsgType
    {
        CONNECTION_SUCCESSFUL = 0x01,
        CONNECTION_FAILURE = 0x02,
        CONNECTION_NAT_UDP_CTS = 0x03,		//c->s 请求监听ip和端口
        CONNECTION_NAT_UDP_STC = 0x04,		//s->c 回复监听ip和端口
        CONNECTION_CONTROL = 0x05,          //控制手机
        CONNECTION_CONTROL_COLOR = 0x06,    //控制手机颜色
        FUNCTION_SHUTDOWN = 0x11,		//关机
        FUNCTION_CANCELSHUTDOWN = 0x12,		//取消关机
        FUNCTION_RESTART = 0x13,		//重启
        FUNCTION_DORMANCY = 0x14,	//休眠
        FUNCTION_AUDIO_MUTE = 0x15,	//静音
        FUNCTION_AUDIO_UP = 0x16,	//减小音量
        FUNCTION_AUDIO_DOWN = 0x17,		//加大音量
        FUNCTION_KEY_DOWN = 0x18,		//按下某键
        FUNCTION_KEY_UP = 0x19,		//松开某件
        FUNCTION_ESC = 0x1a,		//关闭当前程序
        FUNCTION_SCHALT = 0x1b,		//切换程序
        FUNCTION_PPT_UP = 0x30,		//ppt上一页
        FUNCTION_PPT_DOWN = 0x31,		//ppt下一页
        FUNCTION_PPT_FULLSCREEN = 0x32,		//ppt全屏
        FUNCTION_PPT_QUITSCREEN = 0x33,		//ppt退出全屏
        FUNCTION_CAMERA_START = 0x40,     //启动摄像头
        FUNCTION_CAMERA_CLOSE = 0x41,     //关闭摄像头
        FUNCTION_CAMERA_SENDIMAHE = 0x42,  //发送图片
        FUNCTION_CAMERA_NOTIMAGE = 0x43,     //取消发送图片
        FUNCTION_CAMERA_DESTTOP=0x44,       //发送桌面
        FUNCTION_MUSIC_OPEN = 0x50,	//打开默认音乐播放器
        FUNCTION_MUSIC_MINI = 0x51,	//迷你窗口
        FUNCTION_MUSIC_LYC = 0x52,	//歌词
        FUNCTION_MUSIC_PLAY = 0x53,//播放音乐
        FUNCTION_MUSIC_NOTAUDIO = 0x54,	//静音
        FUNCTION_MUSIC_DOWNAUDIO = 0x55,//减小
        FUNCTION_MUSIC_UPAUDIO = 0x56,	//加大音量
        FUNCTION_MUSIC_NEXTSONG = 0x57,	//下一首
        FUNCTION_MUSIC_ONSONG = 0x58	//上一首
    }
    public class Defines
    {
        public static int Port = 8087;
        public static int UdpPort = 8086;
        public static MainWindow MainW;
    }
}
