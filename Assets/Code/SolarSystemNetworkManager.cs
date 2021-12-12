using TMPro;
using UnityEngine;
using UnityEngine.Networking;


namespace Code
{
    public sealed class SolarSystemNetworkManager : NetworkManager
    {
        [SerializeField] private TMP_InputField _nameInput;

        public override void OnServerAddPlayer(NetworkConnection connection, short playerControllerId)
        {
            var spawnTransform = GetStartPosition();

            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            var playerName = !string.IsNullOrEmpty(_nameInput.text)
                ? _nameInput.text
                : $"Player{connection.connectionId}";
            player.GetComponent<ShipController>().PlayerName = playerName;
            NetworkServer.AddPlayerForConnection(connection, player, playerControllerId);
        }
    }
}