using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ZombiRunner
{
    public class TopBar : MonoBehaviour
    {
        [Header("Objects")]
        [SerializeField] private TextMeshProUGUI _coinAmountText;

        [Header("Variables")]
        private int _coinAmount;

        public void AddCoin()
        {
            _coinAmount++;
            UpdateCoinTextValue();
        }
        private void UpdateCoinTextValue() => _coinAmountText.text = $"{_coinAmount}";
    }
}