using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit;

public class UnitState : MonoBehaviour
{
    public float turningTime { get; private set; }
    protected Animator basic_Ani;
    IEnumerator corSC;
    Dictionary<bool, int> booltoint = new Dictionary<bool, int>();
    int hashHitType;
    int hashHit;
    int nPCis1;
    int battleSkill;
    int ambushSkill;

    System.Action<MovingState, bool>[] selectSet;

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
    bool isVoiceStop;

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
    public void GetSoundManager(SoundManager manager)
    {
        soundManager = manager;
    }

    protected virtual void Start()
    {
        turningTime = 0;


        hashHitType = Animator.StringToHash("HitType");
        hashHit = Animator.StringToHash("hit");
        battleSkill = Animator.StringToHash("BattleSkill");
        ambushSkill = Animator.StringToHash("AmbushSkill");
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
        int id = Animator.StringToHash("State");

        float offset = state - basic_Ani.GetFloat(id);
        float fAdd = offset / 32;
        while (Mathf.Abs(offset) > 0.1f)
        {
            float fValue = basic_Ani.GetFloat(id) + fAdd;
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
        if (corHeadWeigh != null)
        {
            StopCoroutine(corHeadWeigh);
        }

        corHeadWeigh = Head_TurnWeighDown(UPdown.up);
        if (downIsTrue)
        {
            corHeadWeigh = Head_TurnWeighDown(UPdown.down);
        }
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
        if (isVoiceStop && type == voiceType.HitVoice)
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

    public void Hit(int hit)
    {
        basic_Ani.SetFloat(hashHitType, hit);               //0회피 1평타 3 스킬
        basic_Ani.SetTrigger(hashHit);

        int soundNum = hit % 2;
        VoicePlay((voiceType)(soundNum * 4) + 1);
    }
    public void Death(bool backAttack)
    {
        basic_Ani.SetTrigger(hashHit);
        basic_Ani.SetTrigger("Death");
        StartCoroutine(AttackWeighDown());
        VoicePlay(voiceType.DieVoice);
        if (corHeadWeigh != null)
            StopCoroutine(corHeadWeigh);

        if (backAttack) basic_Ani.SetTrigger("BackDeath");
    }
    IEnumerator AttackWeighDown()
    {
        for (int i = 0; i < 20; i++)
        {
            basic_Ani.SetLayerWeight(2, basic_Ani.GetLayerWeight(2) - 0.05f);
            basic_Ani.SetLayerWeight(3, basic_Ani.GetLayerWeight(3) - 0.03f);
            yield return new WaitForSeconds(0.1f);
        }

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
}
