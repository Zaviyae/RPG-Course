using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Weapons
{
    [CreateAssetMenu(menuName = "RPG/Weapon")]
    public class Weapon : ScriptableObject
    {

        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip attackAnimation;

        [SerializeField] float minTimeBetweenHits = .5f;
        [SerializeField] float maxAttackRange = 2f;

        public GameObject GetWeaponPrefab()
        {
            return weaponPrefab;
        }

        public AnimationClip GetAnimationClip()
        {
            return attackAnimation;
        }

        public float GetMinTime()
        {
            return minTimeBetweenHits;
        }

        public float GetMaxAttackRange()
        {
            return maxAttackRange;
        }
    }
}