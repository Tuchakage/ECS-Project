using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float moveSpeed;

    void Update()
    {
        transform.Translate(0, 0, -moveSpeed * Time.deltaTime);
        if (transform.position.z < 0) transform.position = new Vector3(transform.position.x, transform.position.y, 10f);
    }
}
