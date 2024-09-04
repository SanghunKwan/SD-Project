using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit;


public abstract class WeaponComponent : MonoBehaviour
{
    protected UnitMove unitMove;
    protected UnitState unitState;
    protected System.Action[] actions;
    private void Awake()
    {
        unitMove = GetComponent<UnitMove>();
        unitState = GetComponent<UnitState>();
    }

    public abstract IEnumerator NormalAttackandDelay();
    public abstract IEnumerator NormalSkillandDelay();

    public void Hit(Collider other, Vector3 vec, SkillData.Skill skill, CObject target)
    {
        if (other.gameObject.layer == 1 << 16 || (other.CompareTag(tag) && !skill.frendlyFire && target.ObjectCollider != other))
            return;
        CObject cTarget = other.GetComponent<CObject>();

        GameManager.manager.DamageCalculate(unitMove.cUnit, cTarget, (int)UnitMove.Skill.Skill, skill,
            () => cTarget.GetStatusEffect(skill.effect_IndexEnum, vec));
    }
    void Start()
    {
        actions = new System.Action[(int)Species.MAX];
        actions[(int)Species.Goblin] = () => StartCoroutine(GoblinSurprise());
        actions[(int)Species.player] = () => StartCoroutine(PlayerSurprise());
    }

    public void SurpriseAttack()
    {

        actions[(int)unitMove.cUnit.GetSpecies]();
    }
    protected abstract IEnumerator GoblinSurprise();
    protected abstract IEnumerator PlayerSurprise();

    protected void PlayerSurpriseReact()
    {
        GameManager.manager.InputSpace();
        //�ð� ������.

        //attacker �� detected�̸� ȸ��.

        //���� ȸ����.

    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
