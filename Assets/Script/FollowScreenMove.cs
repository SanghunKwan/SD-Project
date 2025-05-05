using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScreenMove : MonoBehaviour
{
    [SerializeField] int moveRatioX;
    [SerializeField] int moveRatioY;
    private void Start()
    {
        if (moveRatioX != 0 && moveRatioY != 0)
            GameManager.manager.screenMove += ScreenMove;
        else Debug.LogError("moveRatioX or moveRatioY is 0");
    }
    void ScreenMove(Vector3 move)
    {
        Vector3 vec = new Vector3(move.x / moveRatioX, 0, move.y / moveRatioY);
        transform.position += vec;
    }
}
