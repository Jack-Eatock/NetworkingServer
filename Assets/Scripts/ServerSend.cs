using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend {
    private static void SendTCPData(int _toClient, Packet _packet) {

        _packet.WriteLength();
        Server.Clients[_toClient].Tcp.SendData(_packet);
    }

    private static void SendTCPDataToAll(Packet _packet) {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++) {
            Server.Clients[i].Tcp.SendData(_packet);
        }
    }

    private static void SendTCPDataToAll(int _exceptClient, Packet _packet) {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++) {
            if (i != _exceptClient) {
                Server.Clients[i].Tcp.SendData(_packet);
            }

        }
    }

    private static void SendUDPData(int _toClient, Packet _packet) {
        _packet.WriteLength();
        Server.Clients[_toClient].Udp.SendData(_packet);
    }

    private static void SendUDPDataToAll(Packet _packet) {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++) {
            Server.Clients[i].Udp.SendData(_packet);
        }
    }

    private static void SendUDPDataToAll(int _exceptClient, Packet _packet) {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++) {
            if (i != _exceptClient) {
                Server.Clients[i].Udp.SendData(_packet);
            }

        }
    }

    public static void Welcome(int _toClient, string _msg) {
        using (Packet _packet = new Packet((int)ServerPackets.welcome)) {
            _packet.Write(_msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void SpawnPlayer(int _toclient, Player _player) {

        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer)) {

            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);

            SendTCPData(_toclient, _packet);
        }
    }

    public static void PlayerPosition(Player _player) {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition)) {

            _packet.Write(_player.id);
            _packet.Write(_player.Car.transform.position);
          
            SendUDPDataToAll(_packet);
        };
    }

    public static void PlayerRotation(Player _player) {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation)) {

            _packet.Write(_player.id);
            _packet.Write(_player.Car.transform.rotation);

            SendUDPDataToAll( _packet);
        };
    }

    public static void PlayerDisconnected(int _playerId) {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected)) {

            _packet.Write(_playerId);
            SendTCPDataToAll(_packet);
        };
    }

}

