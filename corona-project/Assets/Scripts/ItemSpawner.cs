using System;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{
    public GameObject TP;
    public GameObject Mask;
    public GameObject BasePleb;

    [Header("Spawning Area")] public GameObject spawningPlane;

    private float timer;
    private float timer2;
    private float timer3;

    private Vector3 size;
    
    private void Start()
    {
        size = spawningPlane.GetComponent<MeshRenderer>().bounds.size;
    }

    private void Update()
    {
        if (!GameManager.Instance.running)
            return;
        
        if (timer >= 0.5f)
        {
            GameObject o = GameObject.Instantiate(TP);
            o.transform.position = new Vector3(Random.value * size.x / 2 * (Random.value > 0.5 ? -1 : 1), 20, Random.value * size.z / 2 * (Random.value > 0.5 ? -1 : 1));
            timer = 0;
        }

        if (timer2 >= 10f)
        {
            GameObject o = GameObject.Instantiate(Mask);
            o.transform.position = new Vector3(Random.value * size.x / 2 * (Random.value > 0.5 ? -1 : 1), 5, Random.value * size.z / 2 * (Random.value > 0.5 ? -1 : 1));
            timer2 = 0;
        }

        if (timer3 >= 5f)
        {
            GameObject o = GameObject.Instantiate(BasePleb);
            o.transform.position = new Vector3(Random.value * size.x / 2 * (Random.value > 0.5 ? -1 : 1), 10, Random.value * size.z / 2 * (Random.value > 0.5 ? -1 : 1));
            o.transform.parent = GameManager.Instance.basePlebs;
            timer3 = 0;
        }

        timer += Time.deltaTime;
        timer2 += Time.deltaTime;
        timer3 += Time.deltaTime;
    }
}