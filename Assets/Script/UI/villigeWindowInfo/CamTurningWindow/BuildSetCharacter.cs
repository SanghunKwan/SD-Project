using TMPro;
using UnityEngine;

namespace SDUI
{
    public class BuildSetCharacter
    {
        public TextMeshProUGUI team;
        public TextMeshProUGUI heroName;
        string defaultName;
        public GameObject gameObject { get; private set; }
        public bool isAllocated;
        public bool isValid { get { return isAllocated || !gameObject.activeSelf; } }


        public BuildSetCharacter(Transform tr)
        {
            team = tr.GetChild(0).GetComponent<TextMeshProUGUI>();
            heroName = tr.GetChild(1).GetComponent<TextMeshProUGUI>();
            gameObject = tr.gameObject;
            defaultName = heroName.text;
        }
        public void ResetTeam()
        {
            team.text = "";
            heroName.text = defaultName;
            isAllocated = false;
        }
        public void ChangeTeam(in string nameText, in string teamText)
        {

            team.text = teamText;
            heroName.text = nameText;
            isAllocated = true;
        }
    }

}
