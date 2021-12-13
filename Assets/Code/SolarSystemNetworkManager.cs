using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;


namespace Code
{
    public sealed class SolarSystemNetworkManager : NetworkManager
    {
        [SerializeField] private TMP_InputField _nameInput;

        public override void OnServerAddPlayer(NetworkConnection connection, short playerControllerId, NetworkReader extraMessageReader)
        {
            var spawnTransform = GetStartPosition();

            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            var shipController = player.GetComponent<ShipController>();

            shipController.PlayerName = $"Player{connection.connectionId}";
            
            if (extraMessageReader != null)
            {
                var playerName = extraMessageReader.ReadMessage<StringMessage>().value;
                if (!string.IsNullOrEmpty(playerName))
                {
                    shipController.PlayerName = playerName;
                }
            }

            NetworkServer.AddPlayerForConnection(connection, player, playerControllerId);
            Debug.Log($"OnServerAddPlayer {shipController.PlayerName}");
        }
        
        public override void OnClientConnect(NetworkConnection conn)
        {
            var message = new StringMessage();
            message.value = _nameInput.text;
            ClientScene.AddPlayer(conn, 0, message);
        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
        }
    }
}