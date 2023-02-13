using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombiRunner
{
    public class RoadSpawner : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject[] _roadParts;
        [SerializeField] private GameObject[] _roadCross;

        [Header("Scripts")]
        [SerializeField] private RoadManager _roadManagerScr;
        [SerializeField] private PlayerAnimations _playerAnimationsScr;
        public void FillPools()
        {
            for (int i = 0; i < _roadParts.Length; i++)
            {
                GameObject roadPart = Instantiate(_roadParts[i].gameObject, transform);
                RoadPart roadPartScr = roadPart.GetComponent<RoadPart>();
                roadPartScr.SetUpAllEnemies(_roadManagerScr, _playerAnimationsScr);
                roadPart.gameObject.SetActive(false);

                _roadManagerScr.FillRoadPartPool(roadPartScr);
            }
            for (int i = 0; i < _roadCross.Length; i++)
            {
                GameObject roadCross = Instantiate(_roadCross[i].gameObject, transform);
                RoadCross roadCrossScr = roadCross.GetComponent<RoadCross>();
                roadCrossScr.SetUpAllEnemies(_roadManagerScr, _playerAnimationsScr);
                roadCross.gameObject.SetActive(false);

                _roadManagerScr.FillRoadCrossPool(roadCrossScr);
            }
        }
    }
}