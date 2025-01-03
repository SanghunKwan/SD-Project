using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearButtonEvent : MonoBehaviour
{
    [SerializeField] ButtonInteractableHighlighted buttonHighlight;


    public void OnAnimationEventCall()
    {
        buttonHighlight.GetButton.enabled = true;
        buttonHighlight.Set = true;
    }
}
