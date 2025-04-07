using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit;

public class UnitState : MonoBehaviour
{
    public float turningTime { get; private set; }
    protected Animator basic_Ani;
    Dictionary<bool, int> booltoint = new Dictionary<bool, int>();
    int nPCis1;
    static int hashHitType = Animator.StringToHash("HitType");
    static int hashHit = Animator.StringToHash("hit");
    static int battleSkill = Animator.StringToHash("BattleSkill");
    static int ambushSkill = Animator.StringToHash("AmbushSkill");
    static int id = Animator.StringToHash("State");
    static int buildingWorkAnim = Animator.StringToHash("BuildingWorking");
    static int buildingWorkType = Animator.StringToHash("Building");
    static int buildingManagerIndex = Animator.StringToHash("manager");

    System.Action<MovingState, bool>[] selectSet;

    IEnumerator corSC;
    IEnumerator corHeadWeigh;

    enum UPdown
    {
        down,
        up
    }

    [SerializeField] protected AudioClip[] AttackVoice;
    [SerializeField] protected AudioClip[] DodgeVoice;
    [SerializeField] protected AudioClip[] ShowStrength;
    [SerializeField] protected AudioClip[] EatVoice;
    [SerializeField] protected AudioClip[] WaitVoice;
    [SerializeField] protected AudioClip[] HitVoice;
    [SerializeField] protected AudioClip[] DieVoice;
    List<AudioClip[]> voices = new List<AudioClip[]>();

    //임시 soundmananger
    protected SoundManager soundManager;

    public enum voiceType
    {
        AttackVoice,
        DodgeVoice,
        ShowStrength,
        EatVoice,
        WaitVoice,
        HitVoice,
        DieVoice
    }
    public bool isVoiceStop { get; set; }

    private void Awake()
    {
        basic_Ani = GetComponentInChildren<Animator>();

        selectSet = new System.Action<MovingState, bool>[]
        {
            (state, trfal) => { SetStatePlayer(state,trfal); },
            (state, trfal) => { SetStateNPC(state, trfal); }
        };
        SetVoiceList();
    }
    void SetVoiceList()
    {
        voices.Capacity = 7;
        voices.Add(AttackVoice);
        voices.Add(DodgeVoice);
        voices.Add(ShowStrength);
        voices.Add(EatVoice);
        voices.Add(WaitVoice);
        voices.Add(HitVoice);
        voices.Add(DieVoice);
    }

    protected virtual void Start()
    {
        soundManager = GameManager.manager.soundManager;
        turningTime = 0;
    }
    public void SetState(MovingState movingState, bool enemySearch)
    {
        if (corSC != null) StopCoroutine(corSC);
        selectSet[nPCis1](movingState, enemySearch);
        StartCoroutine(corSC);
    }
    void SetStatePlayer(MovingState movingState, bool enemySearch)
    {
        corSC = StateChange((int)movingState * booltoint[enemySearch]);
    }
    void SetStateNPC(MovingState movingState, bool enemySearch)
    {
        corSC = StateChange((int)movingState + booltoint[enemySearch]);
    }
    protected IEnumerator StateChange(int state)
    {
        float offset = state - basic_Ani.GetFloat(id);
        float fAdd = offset / 32;
        float fValue;

        while (Mathf.Abs(offset) > 0.1f)
        {
            fValue = basic_Ani.GetFloat(id) + fAdd;
            basic_Ani.SetFloat(id, fValue);
            offset = state - fValue;

            yield return null;
        }
        basic_Ani.SetFloat(id, state);
    }

    void TurningHead(Turning head)
    {
        basic_Ani.SetInteger("Turning", (int)head);
    }
    public void TurningTimeCheck()
    {
        turningTime += Time.deltaTime * Random.Range(-4, 6);
        switch (Mathf.Round(turningTime))
        {
            case 4:
                TurningHead(Turning.Left);
                break;
            case 9:
                TurningHead(Turning.Right);
                break;

            case 11:
                turningTime = 0;
                break;
            default:
                TurningHead(Turning.Front);
                break;
        }
    }
    public virtual void FocusTarget(bool downIsTrue)
    {
        if (basic_Ani.GetBool(buildingWorkAnim))
            return;

        if (corHeadWeigh != null)
        {
            StopCoroutine(corHeadWeigh);
        }

        UPdown updown = UPdown.up;
        if (downIsTrue)
            updown = UPdown.down;

        corHeadWeigh = Head_TurnWeighDown(updown);
        StartCoroutine(corHeadWeigh);
    }


    IEnumerator Head_TurnWeighDown(UPdown updown)
    {
        if (updown.Equals(UPdown.down))
            while (basic_Ani.GetLayerWeight(3) > 0)
            {
                basic_Ani.SetLayerWeight(3, basic_Ani.GetLayerWeight(3) - 0.03f);
                yield return new WaitForFixedUpdate();
            }

        else
            while (basic_Ani.GetLayerWeight(3) < 0.6)
            {
                basic_Ani.SetLayerWeight(3, basic_Ani.GetLayerWeight(3) + 0.02f);
                yield return new WaitForFixedUpdate();
            }
    }

    public void Attack()
    {
        basic_Ani.SetTrigger("Attack");
        VoicePlay(voiceType.AttackVoice);
    }
    public void VoicePlay(voiceType type, int index = -1)
    {
        if (soundManager == null || (isVoiceStop && type == voiceType.HitVoice))
            return;

        int listIndex = VoiceTypeAction(type);
        int voiceIndex = index < 0 ? Random.Range(0, voices[listIndex].Length) : index;
        AudioClip clip = voices[(int)type][voiceIndex];

        soundManager.PlayNewSource(SoundManager.DictionaryType.VOICE, clip);
    }
    int VoiceTypeAction(voiceType type)
    {
        if (type == voiceType.HitVoice)
            StartCoroutine(VoiceCoolTime());

        return (int)type;
    }
    IEnumerator VoiceCoolTime()
    {
        isVoiceStop = true;
        yield return new WaitForSecondsRealtime(4);
        isVoiceStop = false;

    }

    public void Hit(int hit, bool isLoaded = false)
    {
        basic_Ani.SetFloat(hashHitType, hit);               //0회피 1평타 3 스킬
        basic_Ani.SetTrigger(hashHit);

        int soundNum = hit % 2;
        if (!isLoaded)
            VoicePlay((voiceType)(soundNum * 4) + 1);
    }
    public void Death(bool backAttack, bool isLoadedDead)
    {
        basic_Ani.SetTrigger(hashHit);
        basic_Ani.SetTrigger("Death");

        if (!isLoadedDead)
            VoicePlay(voiceType.DieVoice);
        StopHeadShake();

        if (backAttack) basic_Ani.SetTrigger("BackDeath");
    }

    public void SelectSet(int Playeris0)
    {
        nPCis1 = Playeris0;
        if (nPCis1 == 1)
        {
            booltoint.Add(true, 0);
            booltoint.Add(false, -2);
        }
        else
        {
            booltoint.Add(true, 1);
            booltoint.Add(false, -1);
        }
    }

    public void Skill()
    {
        basic_Ani.SetTrigger(battleSkill);

    }
    public void Ambush()
    {
        basic_Ani.SetTrigger(ambushSkill);
    }

    public void SetBuildingWork(bool onoff)
    {
        basic_Ani.SetBool(buildingWorkAnim, onoff);

        if (onoff)
            StopHeadShake();
        else
        {
            FocusTarget(onoff);
            basic_Ani.SetLayerWeight(2, 1);
        }
    }
    void StopHeadShake()
    {
        if (corHeadWeigh != null)
            StopCoroutine(corHeadWeigh);

        corHeadWeigh = AttackWeighControl(true);
        StartCoroutine(corHeadWeigh);
    }
    IEnumerator AttackWeighControl(bool trueIsMinus)
    {
        int sign = System.Convert.ToInt32(trueIsMinus) * 2 - 1;
        for (int i = 0; i < 20; i++)
        {
            basic_Ani.SetLayerWeight(2, basic_Ani.GetLayerWeight(2) - (0.05f * sign));
            basic_Ani.SetLayerWeight(3, basic_Ani.GetLayerWeight(3) - (0.03f * sign));
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void SetBuildingAnimation(AddressableManager.BuildingImage buildingType, int managerIndex)
    {

        basic_Ani.SetFloat(buildingWorkType, (int)buildingType);
        basic_Ani.SetFloat(buildingManagerIndex, managerIndex);
    }
}
