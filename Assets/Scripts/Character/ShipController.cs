using System;
using Main;
using Mechanics;
using Network;
using UI;
using UnityEngine;
using UnityEngine.Networking;

namespace Characters
{
    public class ShipController : NetworkMovableObject
    {
        [SerializeField] private Transform _cameraAttach;
        private CameraOrbit _cameraOrbit;
        private PlayerLabel playerLabel;
        private float _shipSpeed;
        private Rigidbody _rigidbody;

#pragma warning disable CS0618 // ��� ��� ���� �������
        [SyncVar]
#pragma warning restore CS0618 // ��� ��� ���� �������
        private string _playerName;

#pragma warning disable CS0618 // ��� ��� ���� �������
        [SyncEvent]
#pragma warning restore CS0618 // ��� ��� ���� �������
        public event Action OnSomethingHappend;

        protected override float speed => _shipSpeed;

        public string PlayerName
        {
            get => _playerName;
            set => _playerName = value;
        }

        private void OnGUI()
        {
            if (_cameraOrbit == null)            
                return;
            
            _cameraOrbit.ShowPlayerLabels(playerLabel);
        }

        public override void OnStartAuthority()
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)            
                return;
            
            gameObject.name = _playerName;
            _cameraOrbit = FindObjectOfType<CameraOrbit>();
            _cameraOrbit.Initiate(_cameraAttach == null ? transform : _cameraAttach);
            playerLabel = GetComponentInChildren<PlayerLabel>();
            base.OnStartAuthority();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            Debug.Log("OnStartClient");
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            Debug.Log("OnStartLocalPlayer");
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            Debug.Log("OnStartServer");
        }

        protected override void HasAuthorityMovement()
        {
            var spaceShipSettings = SettingsContainer.Instance?.SpaceShipSettings;
            if (spaceShipSettings == null)            
                return;            

            var isFaster = Input.GetKey(KeyCode.LeftShift);
            var speed = spaceShipSettings.ShipSpeed;
            var faster = isFaster ? spaceShipSettings.Faster : 1.0f;

            _shipSpeed = Mathf.Lerp(_shipSpeed, speed * faster, spaceShipSettings.Acceleration);

            var currentFov = isFaster ? spaceShipSettings.FasterFov : spaceShipSettings.NormalFov;
            _cameraOrbit.SetFov(currentFov, spaceShipSettings.ChangeFovSpeed);

            var velocity = _cameraOrbit.transform.TransformDirection(Vector3.forward) * _shipSpeed;
            _rigidbody.velocity = velocity * (_updatePhase == UpdatePhase.FixedUpdate ? Time.fixedDeltaTime : Time.deltaTime);

            if (!Input.GetKey(KeyCode.C))
            {
                var targetRotation = Quaternion.LookRotation(Quaternion.AngleAxis(_cameraOrbit.LookAngle, -transform.right) * velocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            }
        }

        protected override void FromServerUpdate() { }
        protected override void SendToServer() { }

#pragma warning disable CS0618 // ��� ��� ���� �������
        [Command]
#pragma warning restore CS0618 // ��� ��� ���� �������
        private void CmdCommandMethod()
        {

        }

#pragma warning disable CS0618 // ��� ��� ���� �������
        [ClientRpc]
#pragma warning restore CS0618 // ��� ��� ���� �������
        private void RpcMethod(int value)
        {
            _shipSpeed *= value;
        }

#pragma warning disable CS0618 // ��� ��� ���� �������
        [Client]
#pragma warning restore CS0618 // ��� ��� ���� �������
        private void ClientMethod()
        {

        }

#pragma warning disable CS0618 // ��� ��� ���� �������
        [ClientCallback]
#pragma warning restore CS0618 // ��� ��� ���� �������
        private void LateUpdate()
        {
            _cameraOrbit?.CameraMovement();
        }

#pragma warning disable CS0618 // ��� ��� ���� �������
        [Server]
#pragma warning restore CS0618 // ��� ��� ���� �������
        private void ServerMethod()
        {

        }

#pragma warning disable CS0618 // ��� ��� ���� �������
        [ServerCallback]
#pragma warning restore CS0618 // ��� ��� ���� �������
        private void ServerCalbackMethod()
        {

        }

#pragma warning disable CS0618 // ��� ��� ���� �������
        [TargetRpc]
#pragma warning restore CS0618 // ��� ��� ���� �������
        private void RpcTargetMethod()
        {

        }

#pragma warning disable CS0618 // ��� ��� ���� �������
        [ServerCallback]
#pragma warning restore CS0618 // ��� ��� ���� �������
        public void OnTriggerEnter(Collider other)
        {
            if(hasAuthority)
            {

            }
            else
            {

            }
        }
    }
}
