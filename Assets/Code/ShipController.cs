using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


namespace Code
{
    public sealed class ShipController : NetworkMovableObject
    {
        [SerializeField] private Transform _cameraAttach;
        private CameraOrbit _cameraOrbit;
        private PlayerLabel _playerLabel;
        private float _shipSpeed;
        
        private Rigidbody _rigidbody;
        private Collider _collider;
        private Renderer _renderer;

        [SyncVar] private string _playerName;
        [SyncVar] private bool _serverRigidbodyKinematic;
        [SyncVar] private bool _serverColliderEnabled;
        [SyncVar] private bool _serverRendererEnabled;
        
        public string PlayerName
        {
            get => _playerName;
            set => _playerName = value;
        }

        protected override float _speed { get; }
        
        [ClientCallback]
        private void LateUpdate()
        {
            _cameraOrbit?.CameraMovement();
        }

        private void Start()
        {
            gameObject.name = _playerName;
        }
        
        [Server]
        private void OnTriggerEnter(Collider other)
        {
            OnObjectTriggerEnter();
        }
        
        private void OnObjectTriggerEnter()
        {
            StartCoroutine(DestroyPlayer());
        }
        
        private IEnumerator DestroyPlayer()
        {
            DeactivatePlayer();
            yield return new WaitForSeconds(3);
            ActivatePlayer();
        }

        [Server]
        private void DeactivatePlayer()
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.isKinematic = true;
            _collider.enabled = false;
            _renderer.enabled = false;
            RpcDeactivatePlayer();
        }

        [ClientRpc]
        private void RpcDeactivatePlayer()
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.isKinematic = true;
            _collider.enabled = false;
            _renderer.enabled = false;
        }
        
        [Server]
        private void ActivatePlayer()
        {
            var objectTransform = transform;
            var startPosition = NetworkManager.singleton.GetStartPosition();
            objectTransform.position = startPosition.position;
            objectTransform.rotation = startPosition.rotation;
            _rigidbody.isKinematic = false;
            _collider.enabled = true;
            _renderer.enabled = true;
            RpcActivatePlayer(startPosition.position, startPosition.rotation);
        }
        
        [ClientRpc]
        private void RpcActivatePlayer(Vector3 position, Quaternion rotation)
        {
            var objectTransform = transform;
            objectTransform.position = position;
            objectTransform.rotation = rotation;
            _rigidbody.isKinematic = false;
            _collider.enabled = true;
            _renderer.enabled = true;
        }

        public override void OnStartAuthority()
        {
            GetNecessaryComponents();
            if (_rigidbody == null)
            {
                return;
            }
            
            _cameraOrbit = FindObjectOfType<CameraOrbit>();
            _cameraOrbit.Initiate(_cameraAttach == null ? transform : _cameraAttach);
            _playerLabel = GetComponentInChildren<PlayerLabel>();
            Initiate(UpdatePhase.FixedUpdate);
        }

        private void GetNecessaryComponents()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _renderer = GetComponentInChildren<Renderer>();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            GetNecessaryComponents();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            GetNecessaryComponents();
        }

        protected override void HasAuthorityMovement()
        {
            var spaceShipSettings = SettingsContainer.Instance?.SpaceShipSettings;
            if (spaceShipSettings == null)
            {
                return;
            }

            var isFaster = Input.GetKey(KeyCode.LeftShift);
            var speed = spaceShipSettings.ShipSpeed;
            var faster = isFaster ? spaceShipSettings.Faster : 1.0f;

            _shipSpeed = Mathf.Lerp(_shipSpeed, speed * faster,
                spaceShipSettings.Acceleration);

            var currentFov = isFaster ? spaceShipSettings.FasterFov : spaceShipSettings.NormalFov;
            _cameraOrbit.SetFov(currentFov, spaceShipSettings.ChangeFovSpeed);

            var velocity = _cameraOrbit.transform.TransformDirection(Vector3.forward) * _shipSpeed;
            _rigidbody.velocity = velocity * (_updatePhase == UpdatePhase.FixedUpdate ? Time.fixedDeltaTime : Time.deltaTime);

            if (!Input.GetKey(KeyCode.C))
            {
                var targetRotation =
                    Quaternion.LookRotation(Quaternion.AngleAxis(_cameraOrbit.LookAngle, -transform.right) * velocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            }
        }
        
        private void OnGUI()
        {
            if (_cameraOrbit == null)
            {
                return;
            }

            _cameraOrbit.ShowPlayerLabels(_playerLabel);
        }

        protected override void FromServerUpdate()
        {
             _rigidbody.isKinematic = _serverRigidbodyKinematic;
            _collider.enabled = _serverColliderEnabled;
            _renderer.enabled = _serverRendererEnabled;
        }

        protected override void SendToServer()
        {
            _serverRigidbodyKinematic = _rigidbody.isKinematic;
            _serverColliderEnabled = _collider.enabled;
            _serverRendererEnabled = _renderer.enabled;
        }
    }
}