using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class VilligeHeroSpawnAction : MonoBehaviour
{
    StageEnterHero stageEnterHero;
    [SerializeField] CharacterList characterList;

    private void Awake()
    {
        stageEnterHero = GetComponent<StageEnterHero>();

        stageEnterHero.villigeHeroSpawnAction += SpawnAction;
    }
    void SpawnAction(int nametagIndex, Hero hero)
    {
        characterList.MatchingHeroWithInteract(nametagIndex, hero);
        GameManager.manager.battleClearManager.ComebacktoVillige();
    }
}
