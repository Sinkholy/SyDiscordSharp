using Gateway.Payload.DataObjects;
using Gateway.Payload.DataObjects.Enums;
using System;

namespace Gateway
{
    class SystemEventHandler
    {
        #region Internal events
        internal delegate void NewSystemEvent(IGatewayDataObject payload);
        internal event NewSystemEvent Connected = delegate { };
        internal event NewSystemEvent Heartbeat = delegate { };
        internal event NewSystemEvent HeartbeatACK = delegate { };
        internal event NewSystemEvent ReconnectRequest = delegate { };
        internal event NewSystemEvent InvalidSession = delegate { };
        #endregion

        internal void OnNewSystemEventReceived(Opcode opcode, IGatewayDataObject data)
        {
            switch (opcode)
            {
                case Opcode.Heartbeat:
                    Heartbeat(data); 
                    break;
                case Opcode.Reconnect:
                    ReconnectRequest(data); 
                    break;
                case Opcode.InvalidSession:
                    InvalidSession(data); 
                    break;
                case Opcode.Hello:
                    Connected(data); 
                    break;
                case Opcode.HeartbeatACK:
                    HeartbeatACK(data); 
                    break;
                default: 
                    throw new Exception("Unhandled system event opcode"); // TODO : исключение
            }
        }
        internal SystemEventHandler() { }
    }
}
