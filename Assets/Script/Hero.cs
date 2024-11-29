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
        [SerializeField] Material[] corpseMat;
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

        public override void Selected(bool asdf)
        {
            selected = asdf;
            copyUICircle.gameObject.SetActive(asdf);

            if (stat.HP.Equals(curstat.HP))
                copyBar.gameObject.SetActive(asdf);
        }
        protected override void GetSelecting()
        {
            GameManager.manager.HereComesNewChallenger(this, keycode);
        }
        public override void Hit(int skill)
        {
            base.Hit(skill);
            copyBar.gameObject.SetActive(true);
            if (curstat.HP < stat.HP * 0.3f && deathAlert == default)
            {
                GameManager.manager.InputSpace();
                PrintLowHP();
            }
        }
        public override void Death(Vector3 vec)
        {
            GameManager.manager.ChallengerOut(this, keycode, detected);
            base.Death(vec);
            PrintLowHP(InputEffect.WARNINGANIMTYPE.DIE);
            //아이템이 되어서 줍는 것도 가능.
            ItemComponent itemComp = gameObject.AddComponent<ItemComponent>();
            itemComp.Init(12, corpseMat, CirclePad, 0.1f);
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
            int i = 0;
            while (quirks[i].index != 0)
            {
                i++;
            }

            quirks[i] = QuirkData.manager.quirkInfo.quirks[Random.Range(1, 8)];

            int j = 0;

            while (disease[j].index != 0)
            {
                j++;
            }
            disease[j] = QuirkData.manager.diseaseInfo.quirks[Random.Range(1, 2)];
        }


        public void alloBuilding(ActionAlert.ActionType type)
        {
            VilligeAction = type;
        }
        public void alloBuilding(AddressableManager.BuildingImage type)
        {
            VilligeAction = ActionAlert.ActionType.buildingWork;
            BuildingAction = type;
        }
        public void Villige_CheckText()
        {
            villigeHero.villigeInteract.CheckText();
        }
    }
}
