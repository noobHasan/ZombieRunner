using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombiRunner
{
    public class WeaponManager : MonoBehaviour
    {
        public enum WeaponType
        {
            Fist,
            Knife,
            Pistol,
            Rifle
        }

        [Header("Arsenal")]
        [SerializeField] private Fists _fist;
        [SerializeField] private Bat _bat;
        [SerializeField] private Pistol _pistol;
        [SerializeField] private Rifle _rifle;

        [Header("Settings")]
        [SerializeField] private bool _isPlayer;
        [SerializeField] private LayerMask _target;
        [SerializeField] private float _delayBeforeShot;


        public bool IsSkirmish { get; private set; }
        public bool IsOwnWeapon { get; private set; }

        [Header("Scripts")]
        [SerializeField] private Health _healthScr;
        [SerializeField] private RoadManager _roadManagerScr;
        [SerializeField] private WeaponManager _weaponManagerScr;
        [SerializeField] private PlayerEndSkirmish _playerEndSkirmish;
        [SerializeField] private PlayerAnimations _playerAnimationsScr;
        [SerializeField] private EnemyAnimations _enemyAnimationsScr;

        private void Awake()
        {
            SetArsenal();
        }
        
        public void SetArsenal()
        {
            if (_fist != null)
            {
                _fist.SetOwner(gameObject, _target, _isPlayer, _delayBeforeShot);
                _fist.SetScripts(_weaponManagerScr, _playerAnimationsScr, _enemyAnimationsScr);
            }
            if (_bat != null)
            {
                _bat.SetOwner(gameObject, _target, _isPlayer, _delayBeforeShot);
                _bat.SetScripts(_weaponManagerScr, _playerAnimationsScr, _enemyAnimationsScr);
            }
            if (_pistol != null)
            {
                _pistol.SetOwner(gameObject, _target, _isPlayer, _delayBeforeShot);
                _pistol.SetScripts(_weaponManagerScr, _playerAnimationsScr, _enemyAnimationsScr);
            }
            if (_rifle != null)
            {
                _rifle.SetOwner(gameObject, _target, _isPlayer, _delayBeforeShot);
                _rifle.SetScripts(_weaponManagerScr, _playerAnimationsScr, _enemyAnimationsScr);
            }
        }
        public void ChangeDirection()
        {
            int direction;
            if (_isPlayer) direction = 1;
            else direction = -1;

            if (_fist != null) _fist.SetShotDirection(new Vector3(_roadManagerScr.DirectionX, 0f, _roadManagerScr.DirectionZ) * direction);
            if (_bat != null) _bat.SetShotDirection(new Vector3(_roadManagerScr.DirectionX, 0f, _roadManagerScr.DirectionZ) * direction);
            if (_pistol != null) _pistol.SetShotDirection(new Vector3(_roadManagerScr.DirectionX, 0f, _roadManagerScr.DirectionZ) * direction);
            if (_rifle != null) _rifle.SetShotDirection(new Vector3(_roadManagerScr.DirectionX, 0f, _roadManagerScr.DirectionZ) * direction);
        }
        public void ChangeSkirmishTargets(EnemyFighter[] skirmishTargets)
        {
            IsSkirmish = true;

            if (_fist != null) _fist.SetSkirmishTargets(skirmishTargets);
            if (_bat != null) _bat.SetSkirmishTargets(skirmishTargets);
            if (_pistol != null) _pistol.SetSkirmishTargets(skirmishTargets);
            if (_rifle != null) _rifle.SetSkirmishTargets(skirmishTargets);
        }
        public void ShotAttempt()
        {
            if (_isPlayer) _healthScr.TurnOnImmortality();

            if (_bat != null) if (_bat.gameObject.activeSelf) StartCoroutine(_bat.Shot());
            if (_pistol != null) if (_pistol.gameObject.activeSelf) StartCoroutine(_pistol.Shot());
            if (_rifle != null) if (_rifle.gameObject.activeSelf) StartCoroutine(_rifle.Shot());

            StartCoroutine(Recharge());
        }
        private IEnumerator Recharge()
        {
            yield return new WaitForSeconds(1);

            _healthScr.TurnOffImmortality();
        }
        public void GetRandomWeapon(int number = -1, int offset = 0)
        {
            DropWeapon(false);

            int randomWeapon = Random.Range(1, 4);
            IsOwnWeapon = true;
            if (number != -1) randomWeapon = number;

            if (randomWeapon == 1) ChooseBat(offset);
            else if (randomWeapon == 2) ChoosePistol(offset);
            else if (randomWeapon == 3) ChooseRifle(offset);
        }
        public void ChooseBat(int offset)
        {
            _bat.gameObject.SetActive(true);
            _bat.GetWeapon(offset);
            if (_isPlayer)
            {
                _playerAnimationsScr.SetWeapon(PlayerAnimations.State.RunWithBat);
                _playerAnimationsScr.StartAnimation(PlayerAnimations.State.RunWithBat);
            }
        }
        public void ChoosePistol(int offset)
        {
            _pistol.gameObject.SetActive(true);
            _pistol.GetWeapon(offset);
            if (_isPlayer)
            {
                _playerAnimationsScr.SetWeapon(PlayerAnimations.State.RunWithPistol);
                _playerAnimationsScr.StartAnimation(PlayerAnimations.State.RunWithPistol);
            }
        }
        public void ChooseRifle(int offset)
        {
            _rifle.gameObject.SetActive(true);
            _rifle.GetWeapon(offset);
            if (_isPlayer)
            {
                _playerAnimationsScr.SetWeapon(PlayerAnimations.State.RunWithRifle);
                _playerAnimationsScr.StartAnimation(PlayerAnimations.State.RunWithRifle);
            }
        }
        public void DropWeapon(bool isUseAnim)
        {
            _bat.gameObject.SetActive(false);
            _pistol.gameObject.SetActive(false);
            _rifle.gameObject.SetActive(false);

            IsOwnWeapon = false;

            if (isUseAnim)
            {
                _playerAnimationsScr.SetWeapon(PlayerAnimations.State.RunWithFists);
                _playerAnimationsScr.StartAnimation(PlayerAnimations.State.RunWithFists);
            }
        }

        public void SetScripts(RoadManager roadManagerScr, PlayerAnimations playerAnimationsScr)
        {
            _roadManagerScr = roadManagerScr;
            _playerAnimationsScr = playerAnimationsScr;
            SetArsenal();
        }
        public void EndSkirmish()
        {
            IsSkirmish = false;
            DropWeapon(false);
            StartCoroutine(_playerEndSkirmish.EndSkirmish());
        }
    }
}

