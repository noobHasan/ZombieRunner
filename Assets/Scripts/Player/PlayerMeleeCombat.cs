using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ZombiRunner
{
    public class PlayerMeleeCombat : MonoBehaviour
    {
        [Header("Objects")]
        [SerializeField] private AudioSource _fistHitSound;

        [Header("Settings")]
        [SerializeField] private float _delayBeforeEvent;
        [SerializeField] private LayerMask _targetLayer;

        [Header("Scripts")]
        [SerializeField] private Operator _operatorScr;
        [SerializeField] private TimeLord _timeLordScr;
        [SerializeField] private RoadManager _roadManagerScr;
        [SerializeField] private QuickTimeEventsUI _quickTimeEventsUIScr;
        [SerializeField] private PlayerMovement _playerMovementScr;
        [SerializeField] private PlayerAnimations _playerAnimationsScr;

        [Header("Enemy scripts")]
        private EnemyFighter _enemyFighterScr;
        private EnemyAnimations _enemyAnimationsScr;
        private WeaponManager _enemyWeaponManagerScr;
        private Health _enemyHealthScr;

        public IEnumerator Fight1()
        {
            StartCoroutine(_operatorScr.TransitionToMCM());

            _quickTimeEventsUIScr.StartEvent(SwipeController.SwipeType.DOWN);
            while (_quickTimeEventsUIScr.IsEventWorking) { yield return null; }
            if (!_quickTimeEventsUIScr.IsLastEventWin) 
            {
                GetEnemy();
                _enemyWeaponManagerScr.ShotAttempt();
                yield break;
            }

            GetEnemy();
            _playerMovementScr.Tackle();
            _enemyWeaponManagerScr.ShotAttempt();
            StartCoroutine(EnemyTime());

            yield return new WaitForSeconds(_delayBeforeEvent);
            _quickTimeEventsUIScr.StartEvent(SwipeController.SwipeType.UP);
            while (_quickTimeEventsUIScr.IsEventWorking) { yield return null; }
            if (!_quickTimeEventsUIScr.IsLastEventWin)
            {
                _playerAnimationsScr.LoseMeleeCombat();
                yield return StartCoroutine(TakeHit());
                yield return new WaitForSeconds(0.1f);
                _enemyWeaponManagerScr.ShotAttempt();
                yield break;
            }
            _playerMovementScr.StopMove();
            MoveOnRightPosition();
            _playerAnimationsScr.NextMeleeCombatStage();
            StartCoroutine(GetHit());

            yield return new WaitForSeconds(_delayBeforeEvent);
            _quickTimeEventsUIScr.StartEvent(SwipeController.SwipeType.LEFT);
            while (_quickTimeEventsUIScr.IsEventWorking) { yield return null; }
            if (!_quickTimeEventsUIScr.IsLastEventWin)
            {
                yield return StartCoroutine(TakeHit());
                yield return new WaitForSeconds(0.1f);
                _enemyWeaponManagerScr.ShotAttempt();
                yield break;
            }
            _playerAnimationsScr.NextMeleeCombatStage();
            StartCoroutine(GetHit());

            yield return new WaitForSeconds(_delayBeforeEvent);
            _quickTimeEventsUIScr.StartEvent(SwipeController.SwipeType.RIGHT);
            while (_quickTimeEventsUIScr.IsEventWorking) { yield return null; }
            if (!_quickTimeEventsUIScr.IsLastEventWin)
            {
                _enemyWeaponManagerScr.ShotAttempt();
                yield break;
            }
            _playerAnimationsScr.NextMeleeCombatStage();

            yield return StartCoroutine(GetHit());
            _enemyHealthScr.TakeDamage(200);

            yield return new WaitForSeconds(0.25f);
            StartCoroutine(_operatorScr.TransitionToFPM());
            yield return new WaitForSeconds(0.3f);
            _playerAnimationsScr.NextMeleeCombatStage();
            _playerMovementScr.ContinueMove();
        }
        private IEnumerator GetHit()
        {
            yield return new WaitForSeconds(0.65f);
            _enemyAnimationsScr.GetHitActivate();
            _fistHitSound.Play();
        }
        private IEnumerator TakeHit()
        {
            yield return new WaitForSeconds(0.65f);
            _enemyAnimationsScr.TakeHitActivate();
        }

        private void GetEnemy()
        {
            try
            {
                RaycastHit hit;
                Physics.Raycast(transform.position, new Vector3(_roadManagerScr.DirectionX, 0, _roadManagerScr.DirectionZ), out hit, 30, _targetLayer);
                _enemyAnimationsScr = hit.collider.gameObject.GetComponent<EnemyAnimations>();
                _enemyWeaponManagerScr = hit.collider.gameObject.GetComponent<WeaponManager>();
                _enemyHealthScr = hit.collider.gameObject.GetComponent<Health>();
                _enemyFighterScr = hit.collider.gameObject.GetComponent<EnemyFighter>();
            }
            catch
            {
                StartCoroutine(_operatorScr.TransitionToFPM());
                _playerAnimationsScr.StartAnimation(PlayerAnimations.State.RunWithFists);
                _playerMovementScr.ContinueMove();
            }

        }
        private IEnumerator EnemyTime()
        {
            while (true)
            {
                yield return null;
                _enemyAnimationsScr.ChangeTimeSpeed(_timeLordScr.RealTime);
            }
        }
        private void MoveOnRightPosition()
        {
            Vector3 meleeCombatPosition = _enemyFighterScr.GetMeleeCombatPosition();
            meleeCombatPosition = new Vector3(meleeCombatPosition.x * Mathf.Abs(_roadManagerScr.DirectionX) + transform.position.x * Mathf.Abs(_roadManagerScr.DirectionZ),
                                              transform.position.y,
                                              meleeCombatPosition.z * Mathf.Abs(_roadManagerScr.DirectionZ) + transform.position.z * Mathf.Abs(_roadManagerScr.DirectionX));
            transform.DOMove(meleeCombatPosition, 0.6f);
        }
    }
}