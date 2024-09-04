using System.Collections;
using UnityEngine;
using Unit;

public class WeaponRange : WeaponComponent
{

    [SerializeField] Transform arrowPosition;
    public override IEnumerator NormalAttackandDelay()
    {
        unitMove.NomalAttackAniStart(0.25f, ActingState.Concentrate);
        InputEffect.e.PrintEffect3(2, arrowPosition);


        yield return new WaitForSeconds(1f);

        InputEffect.e.PrintEffect4(transform.position, unitMove.targetSave.transform.position, 3);

        unitMove.Damage(UnitMove.Skill.NormalAttack);
        unitMove.SkillEnd();
        yield return new WaitForSeconds(0.5f);
        unitMove.ActionEnd(UnitMove.Skill.NormalAttack);
        yield return new WaitForSeconds(2f);
        unitMove.bitArray[2] = false;
    }


    public override IEnumerator NormalSkillandDelay()
    {
        unitState.Skill();
        unitMove.SkillAniStart(0, ActingState.Concentrate);
        unitMove.bitArray[0] = true;
        GameObject arrow1 = InputEffect.e.PrintEffect3(2, arrowPosition);
        GameObject arrow2 = InputEffect.e.PrintEffect3(2, arrowPosition);
        GameObject arrow3 = InputEffect.e.PrintEffect3(2, arrowPosition);

        arrow2.transform.localEulerAngles = Vector3.down * 20;
        arrow3.transform.localEulerAngles = Vector3.up * 20;

        arrow3.GetComponent<EffectReturn>().TempTimeChange(SkillData.manager.SkillInfo.skill[2].firstDelay
                                                            + SkillData.manager.SkillInfo.skill[2].durTime);
        arrow1.GetComponent<EffectReturn>().TempTimeChange(SkillData.manager.SkillInfo.skill[2].durTime * 2
                                                            + SkillData.manager.SkillInfo.skill[2].firstDelay);


        yield return new WaitForSeconds(SkillData.manager.SkillInfo.skill[2].firstDelay);
        unitState.VoicePlay(UnitState.voiceType.ShowStrength);
        InputEffect.e.PrintEffect4(transform.position, unitMove.targetSave.transform.position, 3);
        GameManager.manager.DamageCalculate(unitMove.cUnit, unitMove.targetSave, (int)UnitMove.Skill.Skill,
                                            SkillData.manager.SkillInfo.skill[2]);

        yield return new WaitForSeconds(SkillData.manager.SkillInfo.skill[2].durTime);
        InputEffect.e.PrintEffect4(transform.position, unitMove.targetSave.transform.position, 3);
        GameManager.manager.DamageCalculate(unitMove.cUnit, unitMove.targetSave, (int)UnitMove.Skill.Skill,
                                            SkillData.manager.SkillInfo.skill[2]);

        yield return new WaitForSeconds(SkillData.manager.SkillInfo.skill[2].durTime);
        InputEffect.e.PrintEffect4(transform.position, unitMove.targetSave.transform.position, 3);
        GameManager.manager.DamageCalculate(unitMove.cUnit, unitMove.targetSave, (int)UnitMove.Skill.Skill,
                                            SkillData.manager.SkillInfo.skill[2]);

        yield return new WaitForSeconds(SkillData.manager.SkillInfo.skill[2].lastDelay);
        unitMove.ActionEnd(UnitMove.Skill.Skill);
        unitMove.SkillEnd();
        unitMove.bitArray[0] = false;

        yield return new WaitForSeconds(SkillData.manager.SkillInfo.skill[2].coolTime);
        unitMove.bitArray[(int)UnitMove.Skill.Skill + 1] = false;
    }

    protected override IEnumerator GoblinSurprise()
    {
        PlayerSurpriseReact();
        yield return Ambush();
    }

    protected override IEnumerator PlayerSurprise()
    {
        bool skillCooltime = unitMove.bitArray[(int)UnitMove.Skill.Skill + 1];
        yield return Ambush();

        if (!skillCooltime)
            unitMove.bitArray[(int)UnitMove.Skill.Skill + 1] = false;
    }
    IEnumerator Ambush()
    {
        unitState.Ambush();
        unitMove.SkillAniStart(1 / 4f, ActingState.Concentrate);
        unitMove.bitArray[0] = true;
        GameObject arrow = InputEffect.e.PrintEffect3(2, arrowPosition);
        arrow.GetComponent<EffectReturn>().TempTimeChange(SkillData.manager.SkillInfo.skill[5].firstDelay);

        yield return new WaitForSeconds(SkillData.manager.SkillInfo.skill[5].firstDelay);
        GameObject flyingArrow = InputEffect.e.PrintEffect4(transform.position, unitMove.targetSave.transform.position, 3);
        flyingArrow.transform.GetChild(0).GetChild(0).transform.localPosition = Vector3.up * 0.4f;
        GameManager.manager.DamageCalculate(unitMove.cUnit, unitMove.targetSave, (int)UnitMove.Skill.Skill,
                                            SkillData.manager.SkillInfo.skill[5]);

        yield return new WaitForSeconds(SkillData.manager.SkillInfo.skill[5].lastDelay);
        unitMove.ActionEnd(UnitMove.Skill.Skill);
        unitMove.SkillEnd();
    }
}
