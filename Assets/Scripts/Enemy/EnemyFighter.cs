using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombiRunner
{
    public class EnemyFighter : MonoBehaviour
    {
        [Header("Objects for melee combat")]
        [SerializeField] private GameObject _meleeCombatPoint;

        [Header("Objects for hit")]
        [SerializeField] private BoxCollider[] _deadlyColliders;
        [SerializeField] private Rigidbody _enemyRigidbody;

        [Header("Settings for hit")]
        [SerializeField] private bool _isZombi;
        [SerializeField] private float _radiusHit;
        [SerializeField] private LayerMask _targetLayer;

        [Header("Variables for hit")]
        private bool _isHiting;
        public bool IsStand { get; private set; }

        [Header("Scripts")]
        [SerializeField] private RoadManager _roadManagerScr;
        [SerializeField] private EnemyAnimations _enemyAnimationsScr;
        [SerializeField] private EnemyMovement _enemyMovementScr;
        [SerializeField] private WeaponManager _weaponManagerScr;
        [SerializeField] private PlayerAnimations _playerAnimationsScr;

        public void CheckZombi()
        {
            if (!_isZombi) _weaponManagerScr.ChoosePistol(0);
        }
        public void SearchTarget()
        {
            if (_isHiting || !_enemyMovementScr.IsRisen) return;

            Collider[] collider = Physics.OverlapSphere(transform.position, _radiusHit, _targetLayer);
            if (collider.Length > 0 && _isZombi) Hit();
        }
        public void ChangeWeaponDirection() 
        {
            if (!_isZombi) _weaponManagerScr.ChangeDirection();
        } 
        private void Hit()
        {
            _isHiting = true;
            _enemyAnimationsScr.StartAnimation(EnemyAnimations.State.Hit);
        }

        public void EnableDeathCollider()
        {
            for (int i = 0; i < _deadlyColliders.Length; i++)
            {
                _deadlyColliders[i].enabled = true;
            }
            _enemyRigidbody.isKinematic = false;
        }
        public void DisableDeathCollider()
        {
            for (int i = 0; i < _deadlyColliders.Length; i++)
            {
                _deadlyColliders[i].enabled = false;
            }
            _enemyRigidbody.isKinematic = true;
        }

        public void DisableHit() => _isHiting = false;

        public void SetScripts(RoadManager roadManagerScr, PlayerAnimations playerAnimationsScr)
        {
            _roadManagerScr = roadManagerScr;
            _playerAnimationsScr = playerAnimationsScr;
            if (!_isZombi) _weaponManagerScr.SetScripts(roadManagerScr, playerAnimationsScr);
        }
        
        public IEnumerator Skirmish(bool isWithDelay)
        {
            transform.LookAt(_playerAnimationsScr.transform);

            if (isWithDelay) yield return new WaitForSeconds(Random.Range(0.3f, 1.3f));

            while (_enemyMovementScr.IsRisen)
            {
                _weaponManagerScr.ShotAttempt();
                yield return StartCoroutine(StandUp());
                yield return new WaitForSeconds(Random.Range(1.5f, 4));
            }
        }
        private IEnumerator StandUp()
        {
            yield return new WaitForSeconds(0.07f);
            IsStand = true;
            yield return new WaitForSeconds(0.2f);
            yield return new WaitForSeconds(1f);
            IsStand = false;
        }
        public bool GetEnemyLife() => _enemyMovementScr.IsRisen;
        public Vector3 GetMeleeCombatPosition() => _meleeCombatPoint.transform.position;
    }
}