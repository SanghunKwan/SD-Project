using System;
using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class ProjectileComponent : MonoBehaviour
{
    [SerializeField] float speed;
    public float Speed { get => speed; set => speed = value; }
    [SerializeField] int projectileIndex;
    CObject target;
    Action collisionAction;

    public void Init(CObject getTarget, Action collideAction)
    {
        target = getTarget;
        collisionAction += collideAction;
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vecOffset = transform.position - target.transform.position;
        float vecDistance = Vector3.Distance(transform.position, target.transform.position);
        if (vecDistance < speed)
            OnCollision();
        else
            transform.position -= vecOffset / vecDistance * speed;
    }
    void OnCollision()
    {
        gameObject.SetActive(false);
        target = null;
        collisionAction?.Invoke();
        collisionAction = null;
        InputEffect.e.ReturnProjectileComponent(projectileIndex, this);
    }

}
