using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootstrapRef : MonoBehaviour
{
    [SerializeField] private int N;
    [SerializeField] private float rotation = 0f;
    [SerializeField] private float scale = 1f;

    [SerializeField] private Transform original;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < N; i++)
        {
            Vector3 v = new Vector3(Random.Range(-8f, 8f), Random.Range(-5f, 5f), Random.Range(0f, 10f));
            Transform t = Instantiate(original, v, Quaternion.Euler(0, rotation, 0));
            t.localScale = new Vector3(scale, scale, scale);
            t.GetComponent<Move>().moveSpeed = Random.Range(1f, 4f);
        }
    }
}
