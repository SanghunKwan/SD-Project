using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class WeaponMagic : WeaponComponent
{
    [SerializeField] Transform projectilePosition;
    public override IEnumerator NormalAttackandDelay()
    {
        unitMove.NomalAttackAniStart(0f, ActingState.Concentrate);

        yield return new WaitForSeconds(0.25f);
        ProjectileComponent projectile = InputEffect.e.CallProjectileComponent(0);
        projectile.Init(unitMove.targetSave, OnFireballCollision);
        projectile.transform.position = projectilePosition.position;
        projectile.Speed = 0;
        yield return new WaitForSeconds(0.25f);
        projectile.Speed = 0.2f;
        //이펙트 상대방 추적.

        yield return new WaitForSeconds(1f);
        unitMove.ActionEnd(UnitMove.Skill.NormalAttack);
        yield return new WaitForSeconds(3f);
        unitMove.bitArray[2] = false;
    }
    void OnFireballCollision()
    {
        unitMove.Damage(UnitMove.Skill.NormalAttack);
        StartCoroutine(SkillData.manager.MakeSkillStructExceptTarget(6, this, unitMove.targetSave, unitMove.targetSave.transform.position));
        InputEffect.e.PrintEffect(unitMove.targetSave.transform.position, 9);
        unitMove.SkillEnd();
    }

    public override IEnumerator NormalSkillandDelay()
    {
        unitState.Skill();
        unitMove.SkillAniStart(0, ActingState.Concentrate);
        unitState.VoicePlay(UnitState.voiceType.ShowStrength, 0);
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(SkillData.manager.MakeSkillStruct(7, this, unitMove.targetSave, unitMove.targetSave.transform.position));

        unitMove.ActionEnd(UnitMove.Skill.Skill);
        unitMove.SkillEnd();

        //(SkillData.manager.SkillInfo.skill 배열 index 확인
        yield return new WaitForSeconds(SkillData.manager.SkillInfo.skill[7].coolTime);
        unitMove.bitArray[(int)UnitMove.Skill.Skill + 1] = false;
    }

    protected override IEnumerator GoblinSurprise()
    {
        yield return NormalAttackandDelay();
    }

    protected override IEnumerator PlayerSurprise()
    {
        yield return NormalAttackandDelay();
    }
}
