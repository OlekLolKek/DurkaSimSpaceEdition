using UnityEngine;
using UnityEngine.Networking;


namespace Code
{
    public sealed class SolarSystemNetworkManager : NetworkManager
    {
        [SerializeField] private string _playerName;

        public override void OnServerAddPlayer(NetworkConnection connection, short playerControllerId)
        {
            var spawnTransform = GetStartPosition();

            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            player.GetComponent<ShipController>().PlayerName = _playerName;
            NetworkServer.AddPlayerForConnection(connection, player, playerControllerId);
        }
    }
}