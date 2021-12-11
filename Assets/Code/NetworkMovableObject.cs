using System;
using UnityEngine;
using UnityEngine.Networking;


namespace Code
{
    #pragma warning disable 618
    public abstract class NetworkMovableObject : NetworkBehaviour
    #pragma warning restore 618
    {
        protected abstract float _speed { get; }
        protected Action _onUpdateAction { get; set; }
        protected Action _onFixedUpdateAction { get; set; }
        protected Action _onLateUpdateAction { get; set; }
        protected Action _onPreRenderAction { get; set; }
        protected Action _onPostRenderAction { get; set; }

        protected UpdatePhase _updatePhase;
        
#pragma warning disable 618
        [SyncVar] protected Vector3 _serverPosition;
        [SyncVar] protected Vector3 _serverEuler;
#pragma warning restore 618

        public override void OnStartAuthority()
        {
            Initiate(UpdatePhase.Update);
        }

        protected virtual void Initiate(UpdatePhase updatePhase = UpdatePhase.Update)
        {
            _updatePhase = updatePhase;
            
            switch (_updatePhase)
            {
                case UpdatePhase.Update:
                    _onUpdateAction += Movement;
                    break;
                case UpdatePhase.FixedUpdate:
                    _onFixedUpdateAction += Movement;
                    break;
                case UpdatePhase.LateUpdate:
                    _onLateUpdateAction += Movement;
                    break;
                case UpdatePhase.PreRender:
                    _onPreRenderAction += Movement;
                    break;
                case UpdatePhase.PostRender:
                    _onPostRenderAction += Movement;
                    break;
            }
            
            Debug.Log($"Initiate {name}");
        }

        private void Update()
        {
            _onUpdateAction?.Invoke();
            Debug.Log($"Update {name}");
        }

        private void LateUpdate()
        {
            _onLateUpdateAction?.Invoke();
            Debug.Log($"LateUpdate {name}");
        }

        private void FixedUpdate()
        {
            _onFixedUpdateAction?.Invoke();
            Debug.Log($"FixedUpdate {name}");
        }

        private void OnPreRender()
        {
            _onPreRenderAction?.Invoke();
        }

        private void OnPostRender()
        {
            _onPostRenderAction?.Invoke();
        }

        protected virtual void Movement()
        {
            if (hasAuthority)
            {
                HasAuthorityMovement();
            }
            else
            {
                FromServerUpdate();
            }
        }

        protected abstract void HasAuthorityMovement();
        protected abstract void FromServerUpdate();
        protected abstract void SendToServer();
    }
}