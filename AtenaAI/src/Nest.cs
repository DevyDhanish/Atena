using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace atena
{
    public class Nest
    {
        private IPEndPoint? _nestAddress;
        private readonly Socket _listenSocket;
        private bool _connected;

        public Nest()
        {
            _connected = false;
            string serverAddr = Config.Instance.Data.serverAddr;
            string serverPort = Config.Instance.Data.tcpPort;

            IPAddress ipAddress = Dns.GetHostAddresses(serverAddr)
                                     .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            AtenaEvent.instance.OnPigeonConnected += updatePigeonConnectStatus;

            int srvPort = 0;

            try
            {
                srvPort = int.Parse(serverPort);
            }
            catch (ArgumentNullException)
            {
                Log.Err("Provided string {0}, cannot be converted to int", serverPort);
                return;
            }
            catch (FormatException)
            {
                Log.Err("Provided string {0}, cannot be converted to int", serverPort);
                return;
            }
            catch (OverflowException)
            {
                Log.Err("Provided string {0}, cannot be converted to int", serverPort);
                return;
            }

            _nestAddress = new IPEndPoint(ipAddress, srvPort);

            _listenSocket = new Socket(_nestAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _listenSocket.Bind(_nestAddress);

            _listenSocket.Listen(1);
        }

        private void updatePigeonConnectStatus(Socket pigeonSocket)
        {
            _connected = true;
        }
        public void Accept(bool blocking = false)
        {
            if (blocking)
            {
                AtenaEvent.instance?.FireOnPigeonConnected(_listenSocket.Accept());
                return;
            }

            Task.Run(() =>
            {
                AtenaEvent.instance?.FireOnPigeonConnected(_listenSocket.Accept());
            });

            return;
        }
        public static void ListenForData(Socket clientSocket, bool blocking = false)
        {
            if (clientSocket == null)
            {
                Log.Err("Client socket is null. Cannot listen for data.");
                return;
            }

            Action listenAction = () =>
            {
                try
                {
                    while (clientSocket.Connected)
                    {
                        // Step 1: Read length prefix (4 bytes)
                        byte[]? lengthBuffer = ReadExact(clientSocket, 4);
                        if (lengthBuffer == null) break;

                        int messageLength = BitConverter.ToInt32(lengthBuffer.Reverse().ToArray(), 0); // Big-endian

                        // Step 2: Read message
                        byte[]? messageBuffer = ReadExact(clientSocket, messageLength);
                        if (messageBuffer == null) break;

                        // Step 3: Parse protobuf
                        atenaNest.StreamData streamData = atenaNest.StreamData.Parser.ParseFrom(messageBuffer);

                        // Step 4: Decode Base64
                        byte[] rawBytes = Convert.FromBase64String(streamData.Data.ToStringUtf8());
                        streamData.Data = Google.Protobuf.ByteString.CopyFrom(rawBytes);

                        AtenaEvent.instance?.FireOnPegionDataRecieved(clientSocket, streamData);
                    }
                }
                catch (Exception ex)
                {
                    Log.Err("Error while listening for data: {0}", ex.Message);
                }
                finally
                {
                    clientSocket.Close();
                }
            };

            if (blocking)
                listenAction();
            else
                Task.Run(listenAction);
        }

        private static byte[]? ReadExact(Socket socket, int length)
        {
            byte[] buffer = new byte[length];
            int totalRead = 0;

            while (totalRead < length)
            {
                int read = socket.Receive(buffer, totalRead, length - totalRead, SocketFlags.None);
                if (read == 0) return null; // Connection closed
                totalRead += read;
            }

            return buffer;
        }
    }
}
