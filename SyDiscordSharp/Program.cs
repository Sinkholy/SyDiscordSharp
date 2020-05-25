using System;   
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Http;
using Gateway;

using Discord;
using Discord.WebSocket;

namespace SyDiscordSharp
{
    class Program
    {
        static DiscordSocketClient client;
        static async Task Main(string[] args)
        {
            DiscordHttpClient httpClient = new DiscordHttpClient();
            GatewayInfo gatewayInfo = await httpClient.GetGatewayInfoAsync();
            DiscordGatewayClient gatewayClient = new DiscordGatewayClient(gatewayInfo.Uri);
            await gatewayClient.StartAsync();
            Console.WriteLine("end of main");



            //client = new DiscordSocketClient();
            //await client.LoginAsync(TokenType.Bot, "NTU5MDkwMTUzOTM1NjAxNjY1.Xn113g.5hIVKJ9fsDES_arHH0EkZRTMBdA");
            //await client.StartAsync();
            //await Task.Delay(-1);
        }
    }
}