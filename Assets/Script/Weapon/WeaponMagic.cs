using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class WeaponMagic : WeaponComponent
{
    public override IEnumerator NormalAttackandDelay()
    {
        unitMove.NomalAttackAniStart(0f, ActingState.Concentrate);

        yield return new WaitForSeconds(1f);

        //이펙트 상대방 추적.
        InputEffect.e.PrintEffect4(transform.position, unitMove.targetSave.transform.position, 3);

        unitMove.Damage(UnitMove.Skill.NormalAttack);
        unitMove.SkillEnd();
        yield return new WaitForSeconds(0.5f);
        unitMove.ActionEnd(UnitMove.Skill.NormalAttack);
        yield return new WaitForSeconds(3f);
        unitMove.bitArray[2] = false;
    }

    public override IEnumerator NormalSkillandDelay()
    {
        unitState.Skill();
        unitMove.SkillAniStart(0, ActingState.Concentrate);
        unitState.VoicePlay(UnitState.voiceType.ShowStrength, 0);
        yield return StartCoroutine(SkillData.manager.MakeSkillStruct(6, this, unitMove.targetSave, unitMove.targetSave.transform.position));

        unitMove.ActionEnd(UnitMove.Skill.Skill);
        unitMove.SkillEnd();

        //(SkillData.manager.SkillInfo.skill 배열 index 확인
        yield return new WaitForSeconds(SkillData.manager.SkillInfo.skill[1].coolTime);
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
