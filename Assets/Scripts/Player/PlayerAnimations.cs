using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombiRunner
{
    public class PlayerAnimations : MonoBehaviour
    {
        public enum State
        {
            RunWithFists,
            RunWithBat,
            RunWithPistol,
            RunWithRifle,
            RightDash,
            LeftDash,
            Jump,
            Tackle,
            FistShot,
            BatShot,
            PistolShot,
            RifleShot,
            Cover,
            Climb,
            Die
        }

        [Header("Objects for animations")]
        [SerializeField] private Animator[] _playerAnimators;

        [Header("Settings for animations")]
        [SerializeField] private string _weaponStateName;
        [SerializeField] private string _shotTriggerName;
        [SerializeField] private string _timeSpeedName;
        [SerializeField] private string _meleeCombatName;
        [SerializeField] private string _loseMeleeCombatName;
        [SerializeField] private string _coverName;
        [SerializeField] private string _climbName;
        [SerializeField] private string _leftDash;
        [SerializeField] private string _rightDash;

        [Header("Scripts")]
        [SerializeField] private PlayerController _playerControllerScr;

        public void StartAnimation(State state)
        {
            for (int i = 0; i < _playerAnimators.Length; i++)
            {
                _playerAnimators[i].Play($"{state}");
            }
        }
        public void ShotActivate()
        {
            for (int i = 0; i < _playerAnimators.Length; i++)
            {
                //_playerAnimators[i].
                _playerAnimators[i].SetTrigger(_shotTriggerName);
            }
        }
        public void SetWeapon(State state)
        {
            int currentWeaponState = 0;

            if (state == State.RunWithFists) currentWeaponState = 0;
            else if (state == State.RunWithBat) currentWeaponState = 1;
            else if (state == State.RunWithPistol) currentWeaponState = 2;
            else if (state == State.RunWithRifle) currentWeaponState = 3;

            for (int i = 0; i < _playerAnimators.Length; i++)
            {
                _playerAnimators[i].SetInteger(_weaponStateName, currentWeaponState);
            }
        }
        public void SetTimeSpeed(float speed)
        {
            for (int i = 0; i < _playerAnimators.Length; i++)
            {
                _playerAnimators[i].SetFloat(_timeSpeedName, speed);
            }
        }
        public void NextMeleeCombatStage()
        {
            for (int i = 0; i < _playerAnimators.Length; i++)
            {
                _playerAnimators[i].SetTrigger(_meleeCombatName);
            }
        }
        public void LoseMeleeCombat()
        {
            for (int i = 0; i < _playerAnimators.Length; i++)
            {
                _playerAnimators[i].SetTrigger(_loseMeleeCombatName);
            }
        }
        public void Cover(bool IsCover)
        {
            for (int i = 0; i < _playerAnimators.Length; i++)
            {
                _playerAnimators[i].SetBool(_coverName, IsCover);
            }
        }
        public void Climb()
        {
            Cover(false);
            for (int i = 0; i < _playerAnimators.Length; i++)
            {
                _playerAnimators[i].SetTrigger(_climbName);
            }
        }
        public void StartDash(int direction)
        {
            string dash = "";
            if (direction == 1) dash = _rightDash;
            else dash = _leftDash;

            for (int i = 0; i < _playerAnimators.Length; i++)
            {
                _playerAnimators[i].SetTrigger(dash);
            }
            StartCoroutine(ChangeLayerWeidht(0, 1));
        }
        public void EndDash()
        {
            StartCoroutine(ChangeLayerWeidht(1, 0));
        }
        private IEnumerator ChangeLayerWeidht(float firstValue, float secondValue)
        {
            float duration = 0.1f;
            float value = firstValue;
            for (float t = 0; t < duration; t += Time.fixedDeltaTime)
            {
                value = Mathf.Lerp(firstValue, secondValue, t / duration);
                for (int i = 0; i < _playerAnimators.Length; i++)
                {
                    _playerAnimators[i].SetLayerWeight(1, value);
                }

                yield return null;
            }
        }
        public void DeathActivate()
        {
            StartAnimation(PlayerAnimations.State.Die);
            _playerControllerScr.Die();
        }
    }
}

