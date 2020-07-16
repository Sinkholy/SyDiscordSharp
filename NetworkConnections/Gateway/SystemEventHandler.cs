using Gateway.Enums;
using Gateway.Payload.DataObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gateway
{
    class SystemEventHandler
    {
        private List<Thread> handlers;
        internal delegate void newSystemEvent(IGatewayDataObject payload);
        internal delegate void sequenceReceived(string sequence);
        internal delegate void newEvent(string eventData, string eventName);
        internal event newEvent NewEvent = delegate { };
        internal event sequenceReceived NewSequenceReceived = delegate { };
        internal event newSystemEvent Connected = delegate { };
        internal event newSystemEvent Heartbeat = delegate { };
        internal event newSystemEvent HeartbeatACK = delegate { };
        internal event newSystemEvent ReconnectRequest = delegate { };
        internal event newSystemEvent InvalidSession = delegate { };

        internal void OnPayloadReceived(GatewayPayload payload)
        {
            //GatewayPayload payload = JsonConvert.DeserializeObject<GatewayPayload>(value); //TODO : использовать сериализацию извне
            if (payload.Sequence != null) NewSequenceReceived(payload.Sequence);
            switch (payload.Opcode)
            {
                case Opcode.Dispatch: 
                    NewEvent((payload.Data as Dispatch).EventData, payload.EventName); 
                    break;
                case Opcode.Heartbeat:
                    Heartbeat(payload.Data); 
                    break;
                case Opcode.Reconnect:
                    ReconnectRequest(payload.Data); 
                    break;
                case Opcode.InvalidSession:
                    InvalidSession(payload.Data); 
                    break;
                case Opcode.Hello:
                    Connected(payload.Data); 
                    break;
                case Opcode.HeartbeatACK:
                    HeartbeatACK(payload.Data); 
                    break;
                default: 
                    throw new Exception("Unhandled received opcode"); // TODO : исключение
            }
        }
        internal void AddNewHandler(ConcurrentQueue<GatewayPayload> queue)
        {
            Thread newHandler = new Thread(() => ListenToPayloadQueue(queue));
            newHandler.Name = "HandlerThread#" + handlers.Count;
            newHandler.Start();
            handlers.Add(newHandler);
        }
        private void ListenToPayloadQueue(ConcurrentQueue<GatewayPayload> payloads)
        {
            while (true)
            {
                if (payloads.TryDequeue(out GatewayPayload payload)) OnPayloadReceived(payload);
                else // TAI : оно тут нужно?
                {

                }
            }
        }
        internal SystemEventHandler(ConcurrentQueue<GatewayPayload> payloads) 
        {
            handlers = new List<Thread>(capacity: 8);
            AddNewHandler(payloads);
            AddNewHandler(payloads);
        }
    }
}
