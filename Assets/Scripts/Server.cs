using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Server {

    public static int MaxPlayers { get; private set; }
    public static int Port { get; private set; }

    public static Dictionary<int, Client> Clients = new Dictionary<int, Client>();

    public delegate void PacketHandler(int _fromClient, Packet _packet);
    public static Dictionary<int, PacketHandler> packetHandlers;

    private static TcpListener tcpListener;

    private static UdpClient udpListener;

    public static void Start(int _maxPlayers, int _port) {

        MaxPlayers = _maxPlayers;
        Port = _port;

        Debug.Log("Starting Server!");

        InitializeServerData();

        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

        udpListener = new UdpClient(Port);
        udpListener.BeginReceive(UDPRecieveCallback, null);


        Debug.Log($"Server started successfully on {Port}. ");
    }

    private static void TCPConnectCallback(IAsyncResult _result) {
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
        Debug.Log($"Incoming connection from {_client.Client.RemoteEndPoint} ");

        // Iterate through each client, find the next empty slot and conenct to it.
        for (int i = 1; i <= MaxPlayers; i++) {
            if (Clients[i].Tcp.socket == null) {
                Clients[i].Tcp.Connect(_client);
                return;
            }
        }

        // If loop completes all iterations server must be full
        Debug.Log($"{_client.Client.RemoteEndPoint} failed to connect!");
    }

    private static void UDPRecieveCallback(IAsyncResult _result) {
        try {
            IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            Byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
            udpListener.BeginReceive(UDPRecieveCallback, null);

            if (_data.Length < 4) {
                return;
            }

            using (Packet _packet = new Packet(_data)) {
                int _clientId = _packet.ReadInt();

                if (_clientId == 0) {
                    return;
                }

                if (Clients[_clientId].Udp.endPoint == null) {
                    Clients[_clientId].Udp.Connect(_clientEndPoint);
                    return;
                }

                if (Clients[_clientId].Udp.endPoint.ToString() == _clientEndPoint.ToString()) {
                    Clients[_clientId].Udp.HandleData(_packet);
                }
            }
        }
        catch (Exception _ex) {

            Debug.Log($"Error recieving UDP data: {_ex}");
        }
    }

    public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet) {
        try {
            if (_clientEndPoint != null) {
                udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
            }
        }
        catch (Exception _ex) {

            Debug.Log($"Error sending data to {_clientEndPoint} via UDP : {_ex} ");
        }
    }


    private static void InitializeServerData() {
        for (int i = 1; i <= MaxPlayers; i++) {
            Clients.Add(i, new Client(i));
        }

        packetHandlers = new Dictionary<int, PacketHandler>() {
                 {(int) ClientPackets.welcomeReceived, ServerHandle.WelcomeRecieved },
                 {(int) ClientPackets.playerMovement, ServerHandle.PlayerMovement }
            };

        Debug.Log("Initialized Packets.");
    }

    public static void Stop() {
        tcpListener.Stop();
        udpListener.Close();
    }
}
