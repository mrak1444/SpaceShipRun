using Characters;
using UnityEngine;
using UnityEngine.Networking;
namespace Main
{
    [System.Obsolete]
    public class SolarSystemNetworkManager : NetworkManager
    {
        [SerializeField] private string _playerName;
        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            var spawnTransform = GetStartPosition();
            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            player.GetComponent<ShipController>().PlayerName = _playerName;
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }
    }
}