using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//기본 유닛 데이터 등 상속 데이터.
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
        public int[] FieldEquipsNum { get; private set; } = { 0, 0, 0 };
        public int[] SkillsNum { get; private set; } = { 1, 1, 1, 1 };
        public int lv { get; private set; } = 1;

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

            if (GameManager.manager.battleClearManager.ManagerType == InventoryComponent.InventoryType.Villige)
            {
                villigeHero = GetComponent<VilligeHero>();
                GetDefaultName();
                ResetFieldEquip();
            }
            CheckInitCount();
        }
        void GetDefaultName()
        {
            if (isDefaultName)
            {
                DefaultNameManager.mananger.GetRandomName(out string heroName);
                curstat.NAME = heroName;
                name = heroName;

                Villige_CheckText();
                isDefaultName = false;
            }
            else
            {
                curstat.NAME = name;
            }
        }
        protected override IEnumerator DelayGetUI()
        {
            initMaxCount = 5;
            return base.DelayGetUI();
        }
        public override void Selected(bool asdf)
        {
            copyUICircle.gameObject.SetActive(asdf);

            if (curstat.HP.Equals(curstat.curHP))
                copyBar.gameObject.SetActive(false);

            if (asdf)
            {
                GameManager.manager.onSelected.eventAction?.Invoke(gameObject.layer, transform.position);
                PlayerNavi.nav.HeroAdd(this);
            }
            selected = asdf;
        }
        protected override void GetSelecting()
        {
            GameManager.manager.objectManager.NewObject(ObjectManager.CObjectType.Hero, this);
            GameManager.manager.HereComesNewChallenger(this, keycode);
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
            //아이템이 되어서 줍는 것도 가능.
            ItemComp_corpse itemComp = gameObject.AddComponent<ItemComp_corpse>();
            itemComp.Init(12, CirclePad, 0.1f, this);
        }
        protected override void DeathEvent()
        {
            GameManager.manager.HeroDeathEvent();
        }

        public override void DetectbyHit(CUnit Attacker)
        {
            GameManager.manager.Search(ObjectManager.CObjectType.Monster, Attacker, unitMove);
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
                            InputEffect.e.PrintTextMesh(transform.position + 0.5f * texts.Count * Vector3.up, "회피");
            CorNesting(textMesh);
        }
        public override void PrintMiss()
        {
            TMPro.TextMeshProUGUI textMesh =
                            InputEffect.e.PrintTextMesh(transform.position + 0.5f * texts.Count * Vector3.up, "빗나감");
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
            Debug.Log("회복" + add);
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
            int jobCount = 2;
            curstat.RefreshStatus(stat, ((EquipsNum[equipNum] - 1) * (3 * jobCount)) + equipNum);
            curstat.QuirkDiseaseCalculate(quirks, disease);
        }
        public void LoadTeamString(in string newTeam)
        {
            TeamChange(newTeam);
            PlayerNavi.nav.SetTeam(unitMove, newTeam);
            PlayerNavi.nav.HeroClear(this, "=");
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
        public override void FieldEquipUpgrade(int equipNum, int level)
        {
            FieldEquipsNum[equipNum] = level - 1;
        }
        public void ResetFieldEquip()
        {
            EquipsNum[0] -= FieldEquipsNum[0];
            EquipsNum[1] -= FieldEquipsNum[1];
            EquipsNum[2] -= FieldEquipsNum[2];

            FieldEquipsNum[0] = 0;
            FieldEquipsNum[1] = 0;
            FieldEquipsNum[2] = 0;

            EquipSet();
        }
    }
}
