using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombiRunner
{
    public class EnemyMovement : MonoBehaviour
    {
        [Header("Objects for moving")]
        [SerializeField] private Rigidbody _enemyRigidbody;

        [Header("Settings for moving")]
        [SerializeField] private float _moveSpeed;
        [SerializeField] private MoveType _moveType;

        [Header("Varibles for moving")]
        private Vector3 _moveDirection;
        private Vector3 _normale;
        private Vector3 _startLocalPosition;

        public enum MoveType
        {
            Stand,
            Walking,
            Running,
            Shooting,
            Skirmish
        }
        public bool IsRisen { get; private set; }
        public bool IsMove { get; private set; }

        [Header("Scripts")]
        [SerializeField] private EnemyAnimations _enemyAnimationsScr;
        [SerializeField] private WeaponManager _weaponManagerScr;
        [SerializeField] private Health _healthScr;
        private RoadManager _roadManagerScr;

        public void EnemyMoving()
        {
            if (!IsRisen || !IsMove) return;

            Vector3 directionAlongSurface = CalculateNormale(_moveDirection.normalized);
            Vector3 offset = Vector3.zero;
            
            offset = new Vector3(directionAlongSurface.z * _moveSpeed * Time.fixedDeltaTime * -_roadManagerScr.DirectionX, 0f, directionAlongSurface.z * _moveSpeed * Time.fixedDeltaTime * -_roadManagerScr.DirectionZ);
            //offset = new Vector3(directionAlongSurface.x * _moveSpeed * Time.fixedDeltaTime, 0f, 0f);
            
            _enemyRigidbody.MovePosition(_enemyRigidbody.position + offset);
        }

        private Vector3 CalculateNormale(Vector3 forward) => forward - Vector3.Dot(forward, _normale) * _normale;


        //Functions for SetUp
        public void SetUpMovement(RoadManager roadManagerScr)
        {
            _roadManagerScr = roadManagerScr;

            _moveDirection = Vector3.zero;
            _moveDirection.z = _moveSpeed;

            _startLocalPosition = transform.localPosition;
        }
        public void SetAnimation()
        {
            if (_moveType == MoveType.Stand) _enemyAnimationsScr.StartAnimation(EnemyAnimations.State.Stand);
            else if (_moveType == MoveType.Walking) _enemyAnimationsScr.StartAnimation(EnemyAnimations.State.Walking);
            else if (_moveType == MoveType.Running) _enemyAnimationsScr.StartAnimation(EnemyAnimations.State.Running);
            else if (_moveType == MoveType.Shooting) _enemyAnimationsScr.StartAnimation(EnemyAnimations.State.Shooting);
            else if (_moveType == MoveType.Skirmish) _enemyAnimationsScr.StartAnimation(EnemyAnimations.State.Cover);
        }
        public void BackToRightPosition() => transform.localPosition = _startLocalPosition;
        public void Rise() 
        {
            IsRisen = true;
            _healthScr.Birth();
        } 
        public void StartMove() => IsMove = true;
        public void StopMove() => IsMove = false;

        public void Fall() 
        {
            IsMove = false;
            IsRisen = false;
        } 
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Health hpScr = other.gameObject.GetComponent<Health>();
                if (hpScr.IsImmortality) _healthScr.TakeDamage(1000);
            }
        }
    }
}