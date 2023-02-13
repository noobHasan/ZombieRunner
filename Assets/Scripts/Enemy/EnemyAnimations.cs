using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombiRunner
{
    public class EnemyAnimations : MonoBehaviour
    {
        public enum State
        {
            Stand,
            Walking,
            Running,
            Shooting,
            Cover,
            Hit,
            Die
        }

        [Header("Objects for animations")]
        [SerializeField] private Animator _enemyAnimator;

        [Header("Settings for animations")]
        [SerializeField] private string _hitTriggerName;
        [SerializeField] private string _deathTriggerName;
        [SerializeField] private string _getHitName;
        [SerializeField] private string _takeHitName;
        [SerializeField] private string _timeSpeedName;
        [SerializeField] private string _shotName;

        [Header("Scripts")]
        [SerializeField] private EnemyMovement _enemyMovementScr;
        [SerializeField] private EnemyFighter _enemyFighterScr;

        public void StartAnimation(State state) => _enemyAnimator.Play($"{state}");
        public void HitActivate() => _enemyAnimator.SetTrigger(_hitTriggerName);
        public void DeathActivate() 
        {
            _enemyMovementScr.Fall();
            _enemyFighterScr.DisableDeathCollider();
            _enemyAnimator.SetTrigger(_deathTriggerName);
        } 
        public void GetHitActivate() => _enemyAnimator.SetTrigger(_getHitName);
        public void TakeHitActivate() => _enemyAnimator.SetTrigger(_takeHitName);
        public void ChangeTimeSpeed(float value) => _enemyAnimator.SetFloat(_timeSpeedName, value);
        public void ShotActivate() => _enemyAnimator.SetTrigger(_shotName);

    }
}