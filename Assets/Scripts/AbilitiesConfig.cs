using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "LevelEnemyConfig", menuName = "Ability Configurations/Ability Config", order = 1)]
public class AbilitiesConfig : ScriptableObject
{
    public Ability slowMoAbility, sameShape,blast;

}
public enum AbilityType
{
    SlowMO, SameShape,Blast
}
[Serializable]
public class Ability
{
    public AbilityType type;
    public float duration;
    public int levelRequire;
    public float factor;
}