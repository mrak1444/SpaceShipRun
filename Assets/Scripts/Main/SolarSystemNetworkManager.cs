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
            //player.GetComponent<ShipController>().PlayerName = playerName.text;
            _players.Add(conn.connectionId, player.GetComponent<ShipController>());
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            Msggg n = new Msggg();
            n.Name = playerName.text;
            conn.Send(100, n);
            Debug.Log(1);
        }

        /*public override void OnStartHost()
        {
            _textObj.SetActive(false);
        }*/

        private void ReceiveName(NetworkMessage netMsg)
        {
            ShipController shipController = _players[netMsg.conn.connectionId];
            var nameMessage = netMsg.ReadMessage<Msggg>();
            shipController.PlayerName = nameMessage.Name;
            Debug.Log(nameMessage.Name);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            _textObj.SetActive(false);

            NetworkServer.RegisterHandler(100, ReceiveName);
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
    }
}
