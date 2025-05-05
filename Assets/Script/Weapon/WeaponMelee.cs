using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit;

public class WeaponMelee : WeaponComponent
{

    public override IEnumerator NormalAttackandDelay()
    {
        unitMove.NomalAttackAniStart(3f / 4f, ActingState.Concentrate);

        yield return new WaitForSeconds(0.5f);
        unitMove.Damage(UnitMove.Skill.NormalAttack);
        unitMove.SkillEnd();

        yield return new WaitForSeconds(1f);
        unitMove.ActionEnd(UnitMove.Skill.NormalAttack);

        yield return new WaitForSeconds(1f);
        unitMove.bitArray[2] = false;
    }

    public override IEnumerator NormalSkillandDelay()
    {
        unitState.Skill();
        unitMove.SkillAniStart(0, ActingState.Superarmor);
        unitState.VoicePlay(UnitState.voiceType.ShowStrength, 0);
        yield return StartCoroutine(SkillData.manager.MakeSkillStruct(0, this, unitMove.targetSave, transform.position));

        unitState.VoicePlay(UnitState.voiceType.ShowStrength, 1);

        yield return StartCoroutine(SkillData.manager.MakeSkillStruct(1, this, unitMove.targetSave, transform.position));

        unitMove.ActionEnd(UnitMove.Skill.Skill);
        unitMove.SkillEnd();

        yield return new WaitForSeconds(SkillData.manager.SkillInfo.skill[1].coolTime);
        unitMove.bitArray[(int)UnitMove.Skill.Skill + 1] = false;
    }

    protected override IEnumerator GoblinSurprise()
    {
        PlayerSurpriseReact();
        unitState.Ambush();
        unitMove.SkillAniStart(1 / 4f, ActingState.Concentrate);

        yield return new WaitForSeconds(SkillData.manager.SkillInfo.skill[4].firstDelay);
        GameManager.manager.DamageCalculate(unitMove.cUnit, unitMove.targetSave, (int)UnitMove.Skill.Skill,
                                            SkillData.manager.SkillInfo.skill[4],
                                            () => unitMove.targetSave.GetStatusEffect
                                            (SkillData.manager.SkillInfo.skill[4].effect_IndexEnum, transform.position));

        yield return new WaitForSeconds(SkillData.manager.SkillInfo.skill[4].lastDelay);
        unitMove.ActionEnd(UnitMove.Skill.Skill);
        unitMove.SkillEnd();
    }

    protected override IEnumerator PlayerSurprise()
    {
        yield return NormalAttackandDelay();
    }
}
