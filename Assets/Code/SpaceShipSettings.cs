using UnityEngine;


namespace Code
{
    [CreateAssetMenu(menuName = "Settings/SpaceShipSettings", fileName = nameof(SpaceShipSettings))]
    public sealed class SpaceShipSettings : ScriptableObject
    {
        [SerializeField, Range(0.01f, 0.1f)] private float _acceleration;
        [SerializeField, Range(1.0f, 2000.0f)] private float _shipSpeed;
        [SerializeField, Range(1.0f, 5.0f)] private float _faster;
        [SerializeField, Range(0.01f, 179.0f)] private float _normalFov;
        [SerializeField, Range(0.01f, 179.1f)] private float _fasterFov;
        [SerializeField, Range(0.1f, 5.0f)] private float _changeFovSpeed;
        
        public float Acceleration => _acceleration;
        public float ShipSpeed => _shipSpeed;
        public float Faster => _faster;
        public float NormalFov => _normalFov;
        public float FasterFov => _fasterFov;
        public float ChangeFovSpeed => _changeFovSpeed;
    }
}