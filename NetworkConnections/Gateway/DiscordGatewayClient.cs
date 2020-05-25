using Gateway.Enums;
using Gateway.PayloadObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gateway
{
    public class DiscordGatewayClient
    {
        private readonly string botToken = "NTU5MDkwMTUzOTM1NjAxNjY1.Xqx1NQ.egtltET15L0bDfA89oGOr2MVXr0";
        private ClientWebSocket clientWebSocket;
        private readonly JsonSerializer jsonSerializer;
        private string SerializeJson(object toSerialize)
        {
            var sb = new StringBuilder(256); //TODO: подумать над размером буффера 
            using (TextWriter tw = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                jsonSerializer.Serialize(tw, toSerialize);
                return sb.ToString();

                //TODO : подумать над тем стоит ли использовать JsonWriter
                //Его преимущество заключается в том, что он как-то умно скипает парсинг (?) JToken 
                //в обще он имеет преимущества, как я понял, в скорости при работе с JToken и при работе с большими JSon-объектами

                //using (JsonWriter jw = new JsonTextWriter(text))
                //{
                //    jsonSerializer.Serialize(tw, toSerialize);
                //    return sb.toString();
                //}
            }
        }
        private TObject DeserializeJson<TObject>(string value)
        {
            using (TextReader textReader = new StringReader(value))
            {
                using (JsonReader jsonReader = new JsonTextReader(textReader))
                {
                    return (TObject)jsonSerializer.Deserialize(jsonReader);
                }
            }
        }
        public Uri GatewayUri { get; private set; }//RO?
        public async Task StartAsync() //Мне не очень хочется передавать Uri параметром, мб подтянуть из конфига или что-нибудь другое хз
        {
            await ConnectToGateway();
            await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(new byte[4096]), CancellationToken.None);
            Console.WriteLine(this.clientWebSocket.State.ToString());
            await TcpSocketTest();
        }
        private async Task ConnectToGateway()
        {
            await clientWebSocket.ConnectAsync(GatewayUri, CancellationToken.None);
        }
        private async Task TcpSocketTest()
        {
            IdentityProperties properties = new IdentityProperties("SinkholesImpl", "SinkholesDevice");
            Identity identityObj = new Identity(botToken, properties);

            GatewayPayload payload = new GatewayPayload(Opcode.Identity, identityObj);          

            await clientWebSocket.SendAsync(new ArraySegment<byte>(Encoding.Default.GetBytes(SerializeJson(payload))), WebSocketMessageType.Text, true, CancellationToken.None);
            Console.WriteLine("Sended");
            byte[] recieveBuffer = new byte[1024 * 4];
            await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(recieveBuffer), CancellationToken.None);
            string str = Encoding.Default.GetString(recieveBuffer);
            Console.WriteLine("Recieved");
            Console.ReadLine();
        }
        private async Task SendAuthorization()
        {

        }
        public DiscordGatewayClient(Uri gatewayUri)
        {
            GatewayUri = gatewayUri;
            clientWebSocket = new ClientWebSocket();
        }
    }
}