#ifndef _MAIN_CMD_HHH___
#define _MAIN_CMD_HHH___

namespace protocol
{
    struct Head
    {
        unsigned short       usize;       //包含包头Head的总共数据包大小， 网络字节序
        unsigned short       ucmd;        //主命令enum MAIN_CMD， 网络字节序
    }__attribute__ ((packed, aligned(1)));

    enum MAIN_CMD{
        CMD_BASE = 0,
        CMD_LOGIN = 1, //登陆

    };

};
#endif