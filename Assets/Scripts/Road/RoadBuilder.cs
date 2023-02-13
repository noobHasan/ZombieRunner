using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombiRunner
{
    public class RoadBuilder : MonoBehaviour
    {
        [Header("Variables")]
        private Vector3 _buildPosition;

        [Header("Scripts")]
        [SerializeField] private RoadManager _roadManagerScr;

        public void BuildRoadPart(RoadPart roadPart)
        {
            roadPart.transform.position = _buildPosition;
            roadPart.transform.rotation = Quaternion.Euler(_roadManagerScr.RoadRotate);
            roadPart.gameObject.SetActive(true);

            roadPart.UpdateThings();
            roadPart.UpdateEnemies();

            float length = roadPart.GetRoadPartLength() + roadPart.GetRoadPartLengthOffset();
            _buildPosition += new Vector3(length * _roadManagerScr.DirectionX, 0f, length * _roadManagerScr.DirectionZ);
        }

        public void SetTurnOffset(RoadCross roadCross, int direction) => _buildPosition += new Vector3(roadCross.GetRoadPartWidth() * direction * _roadManagerScr.DirectionZ, 0f, 
                                                                                                       roadCross.GetRoadPartWidth() * direction * -_roadManagerScr.DirectionX);
    }
}