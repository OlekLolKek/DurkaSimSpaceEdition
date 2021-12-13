using UnityEngine;


namespace Code
{
    public sealed class PlanetOrbit : NetworkMovableObject
    {
        protected override float _speed => _smoothTime;

        [SerializeField] private Transform _aroundPoint;
        [SerializeField] private float _smoothTime = 0.3f;
        [SerializeField] private float _circleInSecond = 1.0f;

        [SerializeField] private float _offsetSin = 1.0f;
        [SerializeField] private float _offsetCos = 1.0f;
        [SerializeField] private float _rotationSpeed;

        private float _distance;
        private float _currentAngle;
        private float _currentRotationAngle;
        private Vector3 _currentPositionSmoothVelocity;
        
        private const float CIRCLE_RADIANS = Mathf.PI * 2;

        private void Start()
        {
            if (isServer)
            {
                _distance = (transform.position - _aroundPoint.position).magnitude;
                _currentAngle = Random.Range(1.0f, 359.0f);
            }
            
            Initiate(UpdatePhase.FixedUpdate);
        }

        public override void OnStartAuthority()
        {
        }

        protected override void HasAuthorityMovement()
        {
            if (!isServer)
            {
                return;
            }

            var position = _aroundPoint.position;
            position.x += Mathf.Sin(_currentAngle) * _distance * _offsetSin;
            position.z += Mathf.Cos(_currentAngle) * _distance * _offsetCos;
            transform.position = position;
            _currentRotationAngle += Time.deltaTime * _rotationSpeed;
            _currentRotationAngle = Mathf.Clamp(_currentRotationAngle, 0, 361);
            if (_currentRotationAngle >= 360)
            {
                _currentRotationAngle = 0;
            }

            transform.rotation = Quaternion.AngleAxis(_currentRotationAngle, transform.up);
            _currentAngle += 
                CIRCLE_RADIANS *
                _circleInSecond * 
                (_updatePhase == UpdatePhase.FixedUpdate 
                    ? Time.fixedDeltaTime
                    : Time.deltaTime);
            
            SendToServer();
        }

        protected override void FromServerUpdate()
        {
            if (!isClient)
            {
                return;
            }

            transform.position = Vector3.SmoothDamp(transform.position,
                _serverPosition, ref _currentPositionSmoothVelocity, _speed);
            transform.rotation = Quaternion.Euler(_serverEuler);
        }

        protected override void SendToServer()
        {
            _serverPosition = transform.position;
            _serverEuler = transform.eulerAngles;
        }
    }
}