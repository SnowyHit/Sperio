using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Sperio/SkillScriptableObject", order = 1)]
public class Skill : ScriptableObject
{
    public GameObject SkillPrefab;
    public GameObject SplashEffect;
    public float SkillReloadTime;
    public float SkillRootScale;
    public float Damage;
    public ShootingStyle SkillShootingStyle;
}
