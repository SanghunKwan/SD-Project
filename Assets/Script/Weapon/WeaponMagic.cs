using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class WeaponMagic : WeaponComponent
{
    public void Hit(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public void Hit2(Collider other)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator NormalAttackandDelay()
    {
        //unit_State.Attack();
        //targetSave = targetEnermy;

        //ActionStart(10f, ActingState.Concentrate, Skill.NormalAttack);

        //yield return new WaitForSeconds(1f);

        //Damage(Skill.NormalAttack);

        //yield return new WaitForSeconds(0.5f);
        //ActionEnd(Skill.NormalAttack);
        //yield return new WaitForSeconds(2f);
        //bitArray[2] = false;
        throw new System.NotImplementedException();
    }

    public override IEnumerator NormalSkillandDelay()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator GoblinSurprise()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator PlayerSurprise()
    {
        throw new System.NotImplementedException();
    }
}
