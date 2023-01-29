using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPooling : MonoBehaviour
{
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;
    public bool poolwillGrow;

    void Start()
    {
        pooledObjects = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(objectToPool);
            // for organization

            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }


    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {

                return pooledObjects[i];
            }
        }

        if (poolwillGrow)
        {
            GameObject tmp = Instantiate(objectToPool);

            pooledObjects.Add(tmp);
            return tmp;
        }

        return null;
    }
}
