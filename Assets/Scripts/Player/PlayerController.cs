using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombiRunner
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Objects")]
        [SerializeField] private GameObject _playerFPModel;
        [SerializeField] private GameObject _playerTPModel;

        [Header("Variables")]
        private bool _isDragging;
        private bool _isMobilePlatform;
        private bool _isControl = true;
        private bool _isShooting = true;
        public bool IsDie { get; private set; }

        [Header("Scripts")]
        [SerializeField] private PlayerMovement _playerMovementScr;
        [SerializeField] private WeaponManager _weaponManagerScr;
        [SerializeField] private QuickTimeEventsUI _quickTimeEventUIScr;

        private void Awake()
        {
            SwipeController.SwipeEvent += CheckInput;
        }

        private void CheckInput(SwipeController.SwipeType type)
        {
            if (IsDie) return;

            if (type == SwipeController.SwipeType.UP)
            {
                if (!_quickTimeEventUIScr.IsEventWorking && _isControl) _playerMovementScr.Jump(true);
                else _quickTimeEventUIScr.CheckEvent(SwipeController.SwipeType.UP);
            }
            else if (type == SwipeController.SwipeType.DOWN)
            {
                if (!_quickTimeEventUIScr.IsEventWorking && _isControl) _playerMovementScr.Tackle();
                else _quickTimeEventUIScr.CheckEvent(SwipeController.SwipeType.DOWN);
            }
            else if (type == SwipeController.SwipeType.RIGHT)
            {
                if (!_quickTimeEventUIScr.IsEventWorking && _isControl) _playerMovementScr.StartDash(1);
                else _quickTimeEventUIScr.CheckEvent(SwipeController.SwipeType.RIGHT);
            }
            else if (type == SwipeController.SwipeType.LEFT)
            {
                if (!_quickTimeEventUIScr.IsEventWorking && _isControl) _playerMovementScr.StartDash(-1);
                else _quickTimeEventUIScr.CheckEvent(SwipeController.SwipeType.LEFT);
            }
            else if (type == SwipeController.SwipeType.CLICK)
            {
                if (_isShooting)
                {
                    _weaponManagerScr.ShotAttempt();
                    StartCoroutine(_playerMovementScr.SetSkirmishCollider());
                }
            }
        }
        public void Die() => IsDie = true;
        public void EnableControl() => _isControl = true;
        public void DisableControl() => _isControl = false;
        public void EnableShooting() => _isShooting = true;
        public void DisableShooting() => _isShooting = false;

        public void ShowTPModel() => _playerTPModel.SetActive(true);
        public void HideTPModel() => _playerTPModel.SetActive(false);
    }
}

