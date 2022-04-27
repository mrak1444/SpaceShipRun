using Characters;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Main
{
    [System.Obsolete]
    public class SolarSystemNetworkManager : NetworkManager
    {
        [SerializeField] private InputField playerName;
        [SerializeField] private GameObject _textObj;

        private Dictionary<int, ShipController> _players = new Dictionary<int, ShipController>();

        

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            var spawnTransform = GetStartPosition();

            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            _players.Add(conn.connectionId, player.GetComponent<ShipController>());
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
            NetworkServer.RegisterHandler(100, ReceiveName);
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            Msggg n = new Msggg();
            n.Name = playerName.text;
            conn.Send(100, n);
        }

        private void ReceiveName(NetworkMessage netMsg)
        {
            var nameMessage = netMsg.ReadMessage<Msggg>();
            _players[netMsg.conn.connectionId].PlayerName = nameMessage.Name;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            _textObj.SetActive(false);
        }

        public override void OnStartClient(NetworkClient client)
        {
            _textObj.SetActive(false);
        }

        public override void OnStopHost()
        {
            _textObj.SetActive(true);
        }

        public override void OnStopServer()
        {
            _textObj.SetActive(true);
        }

        public override void OnStopClient()
        {
            _textObj.SetActive(true);
        }

    }

    [System.Obsolete]
    public class Msggg : MessageBase
    {
        public string Name;

        public override void Deserialize(NetworkReader reader)
        {
            Name = reader.ReadString();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(Name);
        }
    }
}
