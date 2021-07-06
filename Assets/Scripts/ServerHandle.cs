using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ServerHandle {
    public static void WelcomeRecieved(int _fromClient, Packet _packet) {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        //Debug.Log($"{Server.Clients[_fromClient].Tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        Debug.Log($"{Server.Clients[_fromClient].Tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient} with username {_username} .");

        if (_fromClient != _clientIdCheck) {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }

        // Send player to game.
        Server.Clients[_fromClient].SendIntoGame(_username);
    }

    public static void PlayerMovement(int _fromClient, Packet _packet) {

        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++) {
            _inputs[i] = _packet.ReadBool();
        }

        Quaternion _rotation = _packet.ReadQuaternion();

        Server.Clients[_fromClient].player.SetInput(_inputs, _rotation);
    }
}
