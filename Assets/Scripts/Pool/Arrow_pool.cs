using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_pool : MonoBehaviour
{
    [SerializeField] private GameObject arrow_prefab;
    private Queue<GameObject> arrow_pool = new Queue<GameObject>();
    private int pool_size = 20;
    public static Arrow_pool instance;
    private void Start()
    {
        Spawn_Arrow_Pool();
    }
    private void Spawn_Arrow_Pool()
    {
        for(int i = 0; i < pool_size; i++)
        {
            GameObject arrow = Instantiate(arrow_prefab);
            arrow.SetActive(false);
            arrow_pool.Enqueue(arrow);
        }
    }

    public GameObject Get_Arrow()
    {
        if (arrow_pool.Count > 0)
        {
            GameObject arrow = arrow_pool.Dequeue();
            arrow.SetActive(true);
            return arrow;
        }
        GameObject new_arrow = Instantiate(arrow_prefab);
        return new_arrow;

    }
    public void Return_Arrow_Pool(GameObject arrow)
    {
        arrow_pool.Enqueue(arrow);
        arrow.SetActive(false);
    }
}
