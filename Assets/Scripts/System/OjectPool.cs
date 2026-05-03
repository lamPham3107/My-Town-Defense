using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private readonly T _prefab;   // redonly la giup cho ta khong the thay doi gia tri cua _prefab sau khi da khoi tao doi tuong OjectPool , gan 1 lan duy nhat sau do khong doi
    private readonly Transform _parent;
    private readonly Queue<T> pool = new();

    public ObjectPool(T prefab , int PoolSize , Transform parent = null)
    {
        _parent = parent;
        _prefab = prefab;
        for (int i = 0; i < PoolSize; i++)
        {
            pool.Enqueue(CreateNew());
        }
    }

    private T CreateNew()
    {
        T obj = Object.Instantiate(_prefab, _parent);
        obj.gameObject.SetActive(false);
        return obj;
    }

    public T Get(Vector3 position , Quaternion rotation)
    {
        T obj;
        if(pool.Count > 0)
        {
           obj = pool.Dequeue();
        }
        else
        {
           obj = CreateNew();
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.gameObject.SetActive(true);
        return obj;

    }

    public void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}
