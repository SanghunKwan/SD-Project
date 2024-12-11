using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSummon : MonoBehaviour
{
    [SerializeField] CharacterList characterList;

    private void OnMouseDown()
    {

        characterList.NewCharacterCall((Unit.TypeNum)Random.Range(0, 2), transform.position + Vector3.right * 10);
    }
}
