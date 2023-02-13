using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombiRunner
{
    public class RoadPart : MonoBehaviour
    {
        [Header("Objects for road")]
        [SerializeField] private GameObject[] _things;
        [SerializeField] private EnemyMovement[] _enemiesMove;
        [SerializeField] private EnemyFighter[] _enemiesFight;

        [Header("Settings for road")]
        [SerializeField] protected int LinesAmount;
        [SerializeField] protected float RoadPartLength;
        [SerializeField] protected float RoadPartWidth;
        [SerializeField] protected float RoadPartLengthOffset;

        [Header("Objects for skirmish")]
        [SerializeField] protected EnemyFighter[] _skirmishEnemies;

        [Header("Settings for mode")]
        [SerializeField] protected bool IsMeleeCombatMode;
        [SerializeField] protected bool IsShootoutMode;

        private void FixedUpdate()
        {
            for (int i = 0; i < _enemiesMove.Length; i++)
            {
                _enemiesMove[i].EnemyMoving();
                _enemiesFight[i].SearchTarget();
            }
        }

        //Functions for set data
        public void SetUpAllEnemies(RoadManager roadManagerScr, PlayerAnimations playerAnimationsScr)
        {
            for (int i = 0; i < _enemiesMove.Length; i++)
            {
                _enemiesMove[i].SetUpMovement(roadManagerScr);
                _enemiesFight[i].SetScripts(roadManagerScr, playerAnimationsScr);
                _enemiesFight[i].CheckZombi();
            }
        }

        //Functions for update roadPart
        public void UpdateThings()
        {
            for (int i = 0; i < _things.Length; i++)
            {
                _things[i].SetActive(true);
            }
        }
        public void UpdateEnemies()
        {
            for (int i = 0; i < _enemiesMove.Length; i++)
            {
                _enemiesMove[i].gameObject.SetActive(true);
                _enemiesMove[i].BackToRightPosition();
                _enemiesFight[i].DisableHit();
                _enemiesFight[i].EnableDeathCollider();
                _enemiesFight[i].ChangeWeaponDirection();
                _enemiesMove[i].SetAnimation();
                _enemiesMove[i].Rise();
                _enemiesMove[i].StopMove();
            }
        }
        public void RiseEnemies()
        {
            for (int i = 0; i < _enemiesMove.Length; i++)
            {
                _enemiesMove[i].StartMove();
            }
        }

        //Functions for events
        public void StartSkirmish(bool isWithDelay)
        {
            for (int i = 0; i < _skirmishEnemies.Length; i++)
            {
                StartCoroutine(_skirmishEnemies[i].Skirmish(isWithDelay));
            }
        }

        //Functin for get
        public int GetLinesAmount() => LinesAmount;
        public float GetRoadPartLength() => RoadPartLength;
        public float GetRoadPartWidth() => RoadPartWidth;
        public float GetRoadPartLengthOffset() => RoadPartLengthOffset;
        public EnemyFighter[] GetSkirmishTarget() => _skirmishEnemies;
    }
}