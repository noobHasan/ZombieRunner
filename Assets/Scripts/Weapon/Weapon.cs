using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombiRunner
{
    public class Weapon : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] protected GameObject Bullet;
        [SerializeField] protected GameObject ShotEffect;

        [Header("Weapon objects")]
        [SerializeField] protected GameObject ShotPoint;
        [SerializeField] protected AudioSource ShotSound;
        [SerializeField] protected AudioSource HitSound;

        [Header("Weapon Settings")]
        [SerializeField] protected float ShotDistance;
        [SerializeField] protected float BulletSpeed;
        [SerializeField] protected float Damage;
        [SerializeField] protected int CartidgesAmount;
        [SerializeField] protected PlayerAnimations.State ShotState;
        [SerializeField] protected bool IsRangedWeapon;
        private int _currentCartidgesAmount;
        private RaycastHit _shotHit;

        [Header("Owner settings")]
        protected GameObject Owner;
        protected Vector3 DirectionOfView;
        protected LayerMask TargetLayer;
        protected bool IsPlayerOwner;
        protected float DelayBeforeShot;

        [Header("Variables for targets")]
        private List<EnemyFighter> _skirmishTargets;
        private Vector3 _shotDirection;

        [Header("Scripts")]
        private WeaponManager _weaponManagerScr;
        private PlayerAnimations _playerAnimationsScr;
        private EnemyAnimations _enemyAnimationsScr;

        public void SetOwner(GameObject owner, LayerMask targetLayer, bool isPlayerOwner, float delayBeforeShot)
        {
            Owner = owner;
            TargetLayer = targetLayer;
            IsPlayerOwner = isPlayerOwner;
            DelayBeforeShot = delayBeforeShot;
        }
        public void SetShotDirection(Vector3 directionOfView) => DirectionOfView = directionOfView;
        public void SetScripts(WeaponManager weaponManagerScr, PlayerAnimations playerAnimationsScr, EnemyAnimations enemyAnimationsScr)
        {
            _weaponManagerScr = weaponManagerScr;
            _playerAnimationsScr = playerAnimationsScr;
            _enemyAnimationsScr = enemyAnimationsScr;
        }
        public void GetWeapon(int offset)
        {
            _currentCartidgesAmount = CartidgesAmount + offset;
        }
        private bool FindingTarget()
        {
            if (!IsPlayerOwner) _shotDirection = new Vector3(_playerAnimationsScr.transform.position.x - transform.position.x, 0, _playerAnimationsScr.transform.position.z - transform.position.z);
            else 
            {
                if (_weaponManagerScr.IsSkirmish) _shotDirection = GetDirection();
                else _shotDirection = DirectionOfView;
            }
            if (IsRangedWeapon)
            {
                if (Physics.Raycast(ShotPoint.transform.position, _shotDirection, out _shotHit, ShotDistance, TargetLayer)) return true;
                else return false;
            }
            else
            {
                if (Physics.Raycast(Owner.transform.position, _shotDirection, out _shotHit, ShotDistance, TargetLayer)) return true;
                else return false;
            }

        }
        public IEnumerator Shot()
        {
            if (IsPlayerOwner) _playerAnimationsScr.ShotActivate();
            else _enemyAnimationsScr.ShotActivate();
            
            yield return new WaitForSeconds(DelayBeforeShot);
            bool isShotInTarget = FindingTarget();

            Vector3 targetPosition = Vector3.zero;

            if (isShotInTarget)
            {
                Health healthScr = _shotHit.collider.gameObject.GetComponent<Health>();
                healthScr.TakeDamage(Damage);
                targetPosition = _shotHit.collider.transform.position;
            }
            if (IsRangedWeapon) SpawnBullet(_shotDirection, targetPosition);
            else StartCoroutine(PlayHitSound(isShotInTarget));

            _currentCartidgesAmount--;
            CheckCartidges();

            if (_weaponManagerScr.IsSkirmish) StartCoroutine(CheckEnemyLife());
        }
        private void SpawnBullet(Vector3 direction, Vector3 targetPosition)
        {
            ShotSound.Play();

            GameObject bullet = Instantiate(Bullet);
            bullet.transform.position = ShotPoint.transform.position;

            GameObject shotEffect = Instantiate(ShotEffect, transform);
            shotEffect.transform.position = ShotPoint.transform.position;

            Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
            bulletRB.AddForce(direction * BulletSpeed);

            StartCoroutine(DestroyBullet(bulletRB, targetPosition));
        }
        private IEnumerator PlayHitSound(bool isShotInTarget)
        {
            ShotSound.Play();
            yield return new WaitForSeconds(0.03f);
            if (isShotInTarget) HitSound.Play();
        }
        private IEnumerator DestroyBullet(Rigidbody bulletRB, Vector3 targetPosition)
        {
            if (targetPosition == Vector3.zero)
            {
                yield return new WaitForSeconds(3);
                Destroy(bulletRB.gameObject);
            }
            else
            {
                float previousDistance = float.MaxValue;
                float currentDistance = Vector3.Distance(bulletRB.transform.position, targetPosition);

                yield return null;
                while (previousDistance >= currentDistance)
                {
                    previousDistance = currentDistance;
                    currentDistance = Vector3.Distance(bulletRB.transform.position, targetPosition);
                    yield return null;
                }

                bulletRB.velocity = Vector3.zero;
                bulletRB.isKinematic = false;

                MeshRenderer bulletMesh = bulletRB.GetComponent<MeshRenderer>();
                bulletMesh.enabled = false;

                yield return new WaitForSeconds(2);
                Destroy(bulletRB.gameObject);
            }
        }
        private void CheckCartidges()
        {
            if (_currentCartidgesAmount < 0) _weaponManagerScr.DropWeapon(true);
        }
        public void SetSkirmishTargets(EnemyFighter[] skirmishTargets)
        {
            _skirmishTargets = new List<EnemyFighter>();
            for (int i = 0; i < skirmishTargets.Length; i++)
            {
                _skirmishTargets.Add(skirmishTargets[i]);
            }
        }
        private Vector3 GetDirection()
        {
            GameObject target = GetEnemy();

            if (target != null)
            {
                Vector3 shotDirection = new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z);
                return shotDirection;
            }
            else return DirectionOfView;
        }
        private GameObject GetEnemy()
        {
            for (int i = 0; i < _skirmishTargets.Count; i++)
            {
                if (_skirmishTargets[i].IsStand)
                {
                    return _skirmishTargets[i].gameObject;
                }
            }
            return null;
        }
        private IEnumerator CheckEnemyLife()
        {
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < _skirmishTargets.Count; i++)
            {
                if (_skirmishTargets[i].GetEnemyLife()) yield break;
            }

            _weaponManagerScr.EndSkirmish();
        }
    }
}