using UnityEngine;


namespace Code
{
    public sealed class SettingsContainer : Singleton<SettingsContainer>
    {
        [SerializeField] private SpaceShipSettings _spaceShipSettings;
        
        public SpaceShipSettings SpaceShipSettings => _spaceShipSettings;
    }
}