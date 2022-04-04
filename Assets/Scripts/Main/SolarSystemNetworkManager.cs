using Characters;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Main
{
    [System.Obsolete]
    public class SolarSystemNetworkManager : NetworkManager
    {
        //[SerializeField] private string playerName;
        [SerializeField] private GameObject _menu;
        [SerializeField] private InputField _playerName;
        [SerializeField] private Text _nameText;

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            var spawnTransform = GetStartPosition();

            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            player.GetComponent<ShipController>().PlayerName = _playerName.text;
            _nameText.text = player.GetComponent<ShipController>().PlayerName;
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }

        public override void OnStartHost()
        {
            _menu.SetActive(false);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            _menu.SetActive(false);
        }

        public override void OnStartClient(NetworkClient client)
        {
            _menu.SetActive(false);
        }

        public override void OnStopHost()
        {
            _menu.SetActive(true);
        }

        public override void OnStopServer()
        {
            _menu.SetActive(true);
        }

        public override void OnStopClient()
        {
            _menu.SetActive(true);
        }
    }
}
