using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;


namespace Code
{
    public sealed class SolarSystemNetworkManager : NetworkManager
    {
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private int _crystalsAmount;
        [SerializeField] private float _minSpawnDistance;
        [SerializeField] private float _maxSpawnDistance;
        
        public override void OnServerSceneChanged(string sceneName)
        {
            base.OnServerSceneChanged(sceneName);
            
            var crystal = spawnPrefabs[0];
        
            for (int i = 0; i < _crystalsAmount; i++)
            {
                var position = Vector3.zero;
                position.x = Random.Range(_minSpawnDistance, _maxSpawnDistance);
                position.y = Random.Range(_minSpawnDistance, _maxSpawnDistance);
                position.z = Random.Range(_minSpawnDistance, _maxSpawnDistance);
                var crystalInstance = Instantiate(crystal, position, Quaternion.identity);
                crystalInstance.name = "Crystal";
                NetworkServer.Spawn(crystalInstance);
            }
        }
        

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