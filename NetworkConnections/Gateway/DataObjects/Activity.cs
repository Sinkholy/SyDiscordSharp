using Gateway.PayloadCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects
{
    internal class Activity //TODO : попахивает чем-то нездоровым разберусь позже
    {
        string Name;
        ActivityType Type;
        Uri Uri; //Only Type == 1(streaming)
        DateTime CreatedAt;

    }
    internal enum ActivityType : byte //TODO : активити очень под вопросом потому что, пока что, 
                                      //не ясно как оно передается
    {
        Game = 0,
        Streaming = 1,
        Listening = 2,
        Custom = 4
    }
}