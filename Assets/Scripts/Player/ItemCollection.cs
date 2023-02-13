using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombiRunner
{
    public class ItemCollection : MonoBehaviour
    {
        [Header("Sounds")]
        [SerializeField] private AudioSource _takeCoin;
        [SerializeField] private AudioSource _takeWeapon;

        [Header("Tags")]
        [SerializeField] private string _coinTag;
        [SerializeField] private string _weaponBoxTag;
        [SerializeField] private string _pistolBoxTag;

        [Header("Scripts")]
        [SerializeField] private WeaponManager _weaponManagerScr;
        [SerializeField] private TopBar _topBarScr;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == _coinTag)
            {
                other.gameObject.SetActive(false);
                _topBarScr.AddCoin();
                _takeCoin.PlayOneShot(_takeCoin.clip);
            }
            else if (other.gameObject.tag == _weaponBoxTag)
            {
                _weaponManagerScr.GetRandomWeapon();
                other.gameObject.SetActive(false);
                _takeWeapon.Play();
            }
            else if (other.gameObject.tag == _pistolBoxTag)
            {
                _weaponManagerScr.GetRandomWeapon(2, 100);
                other.gameObject.SetActive(false);
                _takeWeapon.Play();
            }
        }
    }
}