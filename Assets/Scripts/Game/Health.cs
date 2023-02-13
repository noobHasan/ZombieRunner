using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombiRunner
{
    public class Health : MonoBehaviour
    {
        [Header("Effects")]
        [SerializeField] private GameObject _bloodEffect1;
        [SerializeField] private GameObject _bloodEffect2;

        [Header("Objects")]
        [SerializeField] private GameObject _getDamagePoint;
        [SerializeField] private AudioSource _deathCry;

        [Header("Settings")]
        [SerializeField] private bool _isPlayer;
        [SerializeField] private float _maxHealth;
        [SerializeField] private float _armor;

        [Header("Variables")]
        private float _health;
        public bool IsImmortality { get; private set; }

        [Header("Scripts")]
        [SerializeField] private PlayerAnimations _playerAnimationsScr;
        [SerializeField] private EnemyAnimations _enemyAnimationsScr;
        [SerializeField] private LoseScreen _loseScreenScr;

        private void Start()
        {
            Birth();
        }

        public void Birth()
        {
            _health = _maxHealth;
        }
        public void TakeDamage(float damage)
        {
            if (_health <= 0) return;

            if (_armor > damage) _armor = damage;
            _health -= damage + _armor;
            if (_health <= 0) StartCoroutine(Death());

            StartCoroutine(SpawnBlood());
        }
        public void TakeHeal(float heal)
        {
            _health += heal;
            if (_health > _maxHealth) _health = _maxHealth;
        }
        private IEnumerator Death()
        {
            if (!_isPlayer) _enemyAnimationsScr.DeathActivate();
            else _playerAnimationsScr.DeathActivate();

            if (_isPlayer)
            {
                _deathCry.Play();
                yield return new WaitForSeconds(1.5f);
                _loseScreenScr.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.2f);
                _loseScreenScr.ReatartGame();
            }
        }
        private IEnumerator SpawnBlood()
        {
            yield return new WaitForSeconds(0.1f);

            GameObject blood1 = Instantiate(_bloodEffect1, transform);
            GameObject blood2 = Instantiate(_bloodEffect2, transform);

            blood1.transform.position = _getDamagePoint.transform.position;
            blood2.transform.position = _getDamagePoint.transform.position;
        }

        public void TurnOnImmortality() => IsImmortality = true;
        public void TurnOffImmortality() => IsImmortality = false;
    }
}