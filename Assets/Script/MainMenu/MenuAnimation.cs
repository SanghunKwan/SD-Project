using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimation : MonoBehaviour
{
    [SerializeField] Transform destination;
    Vector3 direction;
    [SerializeField] float speed;
    private void Start()
    {
        direction = (destination.position - transform.position) * speed * 0.01f;
    }
    public void MoveStart()
    {
        StartCoroutine(Move());
    }
    IEnumerator Move()
    {
        while (Vector3.Distance(transform.position, destination.position) > 4)
        {
            transform.position += direction * Time.deltaTime;
            yield return null;
        }
    }
}
