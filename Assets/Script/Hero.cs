using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//�⺻ ���� ������ �� ��� ������.
namespace Unit
{

    public class Hero : CUnit
    {
        public string keycode { get; protected set; } = "=";
        Animator deathAlert;
        int[] triggerHashes = new int[(int)InputEffect.WARNINGANIMTYPE.MAX];
        VilligeHero villigeHero;

        public QuirkData.Quirk[] quirks { get; private set; } = new QuirkData.Quirk[5] { new QuirkData.Quirk(),
                                                                                         new QuirkData.Quirk(),
                                                                                         new QuirkData.Quirk(),
                                                                                         new QuirkData.Quirk(),
                                                                                         new QuirkData.Quirk()};

        public QuirkData.Quirk[] disease { get; private set; } = new QuirkData.Quirk[4]{ new QuirkData.Quirk(),
                                                                                         new QuirkData.Quirk(),
                                                                                         new QuirkData.Quirk(),
                                                                                         new QuirkData.Quirk()};
        public int[] EquipsNum { get; private set; } = { 1, 1, 1 };
        public int[] SkillsNum { get; private set; } = { 1, 1, 1, 1 };
        public int lv { get; private set; } = 1;
        public int heroInStageIndex { get; set; }

        public ActionAlert.ActionType VilligeAction { get; private set; } = ActionAlert.ActionType.walking;
        public AddressableManager.BuildingImage BuildingAction { get; private set; }
        public bool isDefaultName { private get; set; }


        protected override void Start()
        {
            base.Start();

            for (int i = 0; i < triggerHashes.Length; i++)
            {
                triggerHashes[i] = Animator.StringToHash(((InputEffect.WARNINGANIMTYPE)i).ToString());
            }

            GetDefaultName();
            CheckInitCount();
        }
        void GetDefaultName()
        {
            if (isDefaultName)
            {
                DefaultNameManager.mananger.GetRandomName(out string heroName);
                stat.NAME = heroName;
                curstat.NAME = heroName;

                villigeHero = GetComponent<VilligeHero>();
                Villige_CheckText();
            }
            isDefaultName = false;

        }
        protected override IEnumerator DelayGetUI()
        {
            initMaxCount = 5;
            return base.DelayGetUI();
        }
        public override void Selected(bool asdf)
        {
            selected = asdf;
            copyUICircle.gameObject.SetActive(asdf);

            if (curstat.HP.Equals(curstat.curHP))
                copyBar.gameObject.SetActive(false);

            if (asdf)
            {
                GameManager.manager.onSelected.eventAction?.Invoke(gameObject.layer, transform.position);
                PlayerNavi.nav.HeroAdd(this);
            }
        }
        protected override void GetSelecting()
        {
            GameManager.manager.HereComesNewChallenger(this, keycode);
            GameManager.manager.battleClearManager.NewHero(this);
        }
        public override void Hit(int skill)
        {
            base.Hit(skill);
            copyBar.gameObject.SetActive(true);
            if (curstat.curHP < curstat.HP * 0.3f && deathAlert == default)
            {
                GameManager.manager.InputSpace();
                PrintLowHP();
            }
        }
        protected override void LoadDead(bool isLoaded, in Vector3 vec)
        {
            GameManager.manager.ChallengerOut(this, keycode, detected);
            base.LoadDead(isLoaded, vec);
            PrintLowHP(InputEffect.WARNINGANIMTYPE.DIE);
            //�������� �Ǿ �ݴ� �͵� ����.
            ItemComp_corpse itemComp = gameObject.AddComponent<ItemComp_corpse>();
            itemComp.Init(12, CirclePad, 0.1f, this);
        }

        public override void PrintDamage(int damage)
        {
            TMPro.TextMeshProUGUI textMesh =
                            InputEffect.e.PrintTextMesh(transform.position + 0.5f * texts.Count * Vector3.up, damage.ToString());
            CorNesting(textMesh);

        }
        public override void PrintDodge()
        {
            TMPro.TextMeshProUGUI textMesh =
                            InputEffect.e.PrintTextMesh(transform.position + 0.5f * texts.Count * Vector3.up, "ȸ��");
            CorNesting(textMesh);
        }
        public override void PrintMiss()
        {
            TMPro.TextMeshProUGUI textMesh =
                            InputEffect.e.PrintTextMesh(transform.position + 0.5f * texts.Count * Vector3.up, "������");
            CorNesting(textMesh);
        }
        void PrintLowHP(InputEffect.WARNINGANIMTYPE type = InputEffect.WARNINGANIMTYPE.NONE)
        {

            deathAlert = InputEffect.e.PrintAnimation(transform.position).GetComponent<Animator>();
            deathAlert.SetTrigger(triggerHashes[(int)type]);
            InputEffect.e.EffectPositionFollow(deathAlert.gameObject, this);
        }
        public override void Recovery(int add)
        {
            base.Recovery(add);
            Debug.Log("ȸ��" + add);
            if (deathAlert != default && curstat.HP > stat.HP * 0.3f)
            {
                PrintLowHP(InputEffect.WARNINGANIMTYPE.CANCEL);
                deathAlert = default;

            }
        }

        public void TeamChange(in string str)
        {
            keycode = str;
        }

        public void MakeQuirk()
        {
            MakeQuirk(quirks, QuirkData.manager.quirkInfo, Random.Range(1, 8));
        }
        public void MakeDisease()
        {
            MakeQuirk(disease, QuirkData.manager.diseaseInfo, Random.Range(1, 2));
        }
        void MakeQuirk(QuirkData.Quirk[] quirkArray, QuirkData.QuirkS info, int quirkIndex)
        {
            int i = 0;

            while (quirkArray[i].index != 0)
            {
                i++;
            }
            quirkArray[i] = info.quirks[quirkIndex];
        }
        #region LoadData
        public void SetLevel(int num)
        {
            lv = num;
        }
        public void SetQuirk(in int[] data)
        {
            QuirkLoad(quirks, QuirkData.manager.quirkInfo, data);
        }
        public void SetDisease(in int[] data)
        {
            QuirkLoad(disease, QuirkData.manager.diseaseInfo, data);
        }
        void QuirkLoad(QuirkData.Quirk[] quirkArray, in QuirkData.QuirkS info, in int[] data)
        {
            int length = data.Length;

            for (int i = 0; i < length; i++)
            {
                quirkArray[i] = info.quirks[data[i]];
            }

        }
        public void SetData(int[] skillOrArm, in int[] data)
        {
            int length = data.Length;

            for (int i = 0; i < length; i++)
            {
                skillOrArm[i] = data[i];
            }
        }
        public void EquipSet()
        {
            EquipOne(0);
            EquipOne(1);
            EquipOne(2);
        }
        public override void EquipOne(int equipNum)
        {
            int jobNum = 2;
            curstat.RefreshStatus(stat, ((EquipsNum[equipNum] - 1) * (3 * jobNum)) + equipNum);
        }
        #endregion
        public void alloBuilding(ActionAlert.ActionType type)
        {
            VilligeAction = type;
        }
        public void alloBuilding(AddressableManager.BuildingImage type)
        {
            alloBuilding(ActionAlert.ActionType.buildingWork);
            BuildingAction = type;
        }
        public void Villige_CheckText()
        {
            villigeHero.villigeInteract.CheckText();
        }

        public override void EquipUpgrade(int equipNum, int level)
        {
            EquipsNum[equipNum] = level;
        }
    }
}
