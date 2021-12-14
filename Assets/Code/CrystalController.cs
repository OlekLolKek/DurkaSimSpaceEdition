using UnityEngine;
using UnityEngine.Networking;


namespace Code
{
    public sealed class CrystalController : NetworkMovableObject
    {
        [SerializeField] private float _minRotationSpeed;
        [SerializeField] private float _maxRotationSpeed;
        private float _rotationSpeed;
        
        protected override float _speed { get; }

        private void Start()
        {
            Initiate();
            name = "Crystal";
            _rotationSpeed = Random.Range(_minRotationSpeed, _maxRotationSpeed);
        }
        
        
        [Server]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(typeof(ShipController), out _))
                OnObjectTriggerEnter();
        }
        
        private void OnObjectTriggerEnter()
        {
            gameObject.SetActive(false);
            NetworkServer.UnSpawn(gameObject);
        }

        protected override void HasAuthorityMovement()
        {
            transform.Rotate(Vector3.one * _rotationSpeed * Time.deltaTime);
            
            SendToServer();
        }

        protected override void FromServerUpdate()
        {
            transform.rotation = Quaternion.Euler(_serverEuler);
        }

        protected override void Movement()
        {
            if (isServer)
            {
                HasAuthorityMovement();
            }
            else
            {
                FromServerUpdate();
            }
        }

        protected override void SendToServer()
        {
            _serverEuler = transform.eulerAngles;
        }
    }
}