using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiesPooling : MonoBehaviour
{
    public static ZombiesPooling instance;
    public GameObject zombie_01_Prefab;
    private int poolSize = 20;
    private Queue<GameObject> zombies_01_Pool = new Queue<GameObject>();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        for(int i = 0; i < poolSize; i++)
        {
            GameObject zombie_01 = Instantiate(zombie_01_Prefab);
            zombie_01.transform.SetParent(transform);
            zombie_01.SetActive(false);
            zombies_01_Pool.Enqueue(zombie_01);
        }
    }

    public GameObject Get_Zombie_01()
    {
        if (zombies_01_Pool.Count > 0)
        {
            GameObject zombie_01 = zombies_01_Pool.Dequeue();
            zombie_01.SetActive(true);
            return zombie_01;
        }

        GameObject new_zombie_01 = Instantiate(zombie_01_Prefab);
        return new_zombie_01;
        
    }

    public void ReturnPool_Zombie_01(GameObject zombie_01)
    {
        zombie_01.SetActive(false);
        zombies_01_Pool.Enqueue(zombie_01);
    }
}
