using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace vAzhureRacingHub
{
    public class WSClient : IDisposable
    {
        public enum Opcode
        {
            Fragment = 0,
            Text = 1,
            Binary = 2,
            CloseConnection = 8,
            Ping = 9,
            Pong = 10
        }

        public event EventHandler Disconnected;
        private readonly Socket clientSocket;
        public WSClient(Socket socket) => (clientSocket) = socket;

        public string IP
        {
            get
            {
                try
                {
                    return ((IPEndPoint)(clientSocket.RemoteEndPoint ?? clientSocket.LocalEndPoint)).Address.ToString();
                }
                catch
                {
                    return "";
                }
            }
        }

        public bool Ping()
        {
            try
            {
                var dataToSend = CreateFrameFromString("", Opcode.Ping);
                clientSocket.Send(dataToSend);
                return true;
            }
            catch { return false; }
        }

        private string oldMessage = "";
        public bool SendMessage(string msg)
        {
            if (clientSocket.Connected)
            {
                if (msg == "" || msg == oldMessage)
                    return true;

                oldMessage = msg;

                try
                {
                    var dataToSend = CreateFrameFromString(msg);
                    clientSocket.Send(dataToSend);
                }
                catch
                {
                    Disconnected?.Invoke(this, new EventArgs());
                    Dispose();
                    return false;
                }
                return true;
            }
            else
            {
                Disconnected?.Invoke(this, new EventArgs());
                Dispose();
                return false;
            }
        }

        public bool Handshake()
        {
            var receivedData = new byte[clientSocket.ReceiveBufferSize];
            var receivedDataLength = clientSocket.Receive(receivedData);

            string requestString = Encoding.UTF8.GetString(receivedData, 0, receivedDataLength);

            if (new Regex("^GET").IsMatch(requestString))
            {
                const string eol = "\r\n";

                var receivedWebSocketKey = new Regex("Sec-WebSocket-Key: (.*)").Match(requestString).Groups[1].Value.Trim();
                if (receivedWebSocketKey != "")
                {
                    var keyHash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(receivedWebSocketKey + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"));

                    var response = "HTTP/1.1 101 Switching Protocols" + eol;
                    response += "Connection: Upgrade" + eol;
                    response += "Upgrade: websocket" + eol;
                    response += "Sec-WebSocket-Accept: " + Convert.ToBase64String(keyHash) + eol;
                    response += eol;

                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    try
                    {
                        clientSocket.Send(responseBytes);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                else
                {
                    // Other HTTP request

                    Match ReqMatch = Regex.Match(requestString, @"GET\s+([^?\s]+)((?:[?&][^&\s]+)*)\s+(HTTP/.*)");

                    if (ReqMatch == Match.Empty)
                    {
                        clientSocket.Send(GetError(400));
                        return false;
                    }

                    string RequestUri = ReqMatch.Groups[1].Value;

                    if (RequestUri == "/")
                        RequestUri = "/index.html";

                    string strWorkPath = WebServer.sStrServerPath;

                    RequestUri = Path.Combine(strWorkPath, RequestUri.Substring(1));
                    RequestUri = RequestUri.Replace("/", "\\").ToLower();

                    string Extension = RequestUri.Substring(RequestUri.LastIndexOf('.'));

                    string ContentType;

                    switch (Extension)
                    {
                        case ".json":
                            ContentType = "application/json";
                            break;
                        case ".htm":
                        case ".html":
                            ContentType = "text/html";
                            break;
                        case ".css":
                            ContentType = "text/css";
                            break;
                        case ".js":
                            ContentType = "text/javascript";
                            break;
                        case ".jpg":
                            ContentType = "image/jpeg";
                            break;
                        case ".ico":
                            ContentType = "image/x-icon";
                            break;
                        case ".jpeg":
                        case ".png":
                        case ".gif":
                            ContentType = "image/" + Extension.Substring(1);
                            break;
                        default:
                            if (Extension.Length > 1)
                            {
                                ContentType = "application/" + Extension.Substring(1);
                            }
                            else
                            {
                                ContentType = "application/unknown";
                            }
                            break;
                    }

                    if (ContentType != "")
                    {
                        try
                        {
                            using (FileStream FS = new FileStream(RequestUri, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                // Посылаем заголовки
                                string Headers = $"HTTP/1.1 200 OK\nContent-Type: {ContentType}\nX-Content-Type-Options: \"nosniff\"\nContent-Length: {FS.Length}\n\n";
                                byte[] HeadersBuffer = Encoding.UTF8.GetBytes(Headers);

                                byte[] buffer = new byte[FS.Length];
                                FS.Read(buffer, 0, (int)FS.Length);

                                byte[] data = HeadersBuffer.Concat(buffer).ToArray();
                                clientSocket.Send(data, SocketFlags.None);

                                FS.Close();
                            }
                        }
                        catch
                        {
                            clientSocket.Send(GetError(400));
                        }
                    }
                }
            }
            return false;
        }

        private static byte[] GetError(int Code)
        {
            // Получаем строку вида "200 OK"
            // HttpStatusCode хранит в себе все статус-коды HTTP/1.1
            string CodeStr = Code.ToString() + " " + ((HttpStatusCode)Code).ToString();
            // Код простой HTML-странички
            string Html = $"<html><body><h1>{CodeStr}</h1></body></html>";
            // Необходимые заголовки: ответ сервера, тип и длина содержимого. После двух пустых строк - само содержимое
            string Str = $"HTTP/1.1 {CodeStr}\nContent-type: text/html\nContent-Length:{Html.Length}\n\n{Html}";
            // Приведем строку к виду массива байт
            return Encoding.ASCII.GetBytes(Str);
        }

        public static byte[] CreateFrameFromString(string message, Opcode opcode = Opcode.Text)
        {
            var payload = Encoding.UTF8.GetBytes(message);

            byte[] frame;

            if (payload.Length < 126)
            {
                frame = new byte[1 /*op code*/ + 1 /*payload length*/ + payload.Length /*payload bytes*/];
                frame[1] = (byte)payload.Length;
                Array.Copy(payload, 0, frame, 2, payload.Length);
            }
            else if (payload.Length >= 126 && payload.Length <= 65535)
            {
                frame = new byte[1 /*op code*/ + 1 /*payload length option*/ + 2 /*payload length*/ + payload.Length /*payload bytes*/];
                frame[1] = 126;
                frame[2] = (byte)((payload.Length >> 8) & 255);
                frame[3] = (byte)(payload.Length & 255);
                Array.Copy(payload, 0, frame, 4, payload.Length);
            }
            else
            {
                frame = new byte[1 /*op code*/ + 1 /*payload length option*/ + 8 /*payload length*/ + payload.Length /*payload bytes*/];
                frame[1] = 127; // <-- Indicates that payload length is in following 8 bytes.
                frame[2] = (byte)((payload.Length >> 56) & 255);
                frame[3] = (byte)((payload.Length >> 48) & 255);
                frame[4] = (byte)((payload.Length >> 40) & 255);
                frame[5] = (byte)((payload.Length >> 32) & 255);
                frame[6] = (byte)((payload.Length >> 24) & 255);
                frame[7] = (byte)((payload.Length >> 16) & 255);
                frame[8] = (byte)((payload.Length >> 8) & 255);
                frame[9] = (byte)(payload.Length & 255);
                Array.Copy(payload, 0, frame, 10, payload.Length);
            }

            frame[0] = (byte)((byte)opcode | 0x80 /*FIN bit*/);

            return frame;
        }

        public void Dispose()
        {
            try
            {
                clientSocket.Close();
            }
            catch { }
        }
    }
}
