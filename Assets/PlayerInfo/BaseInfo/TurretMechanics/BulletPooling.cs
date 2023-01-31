using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPooling : MonoBehaviour
{
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public LayerMask bulletLayer;
    public int amountToPool;
    public bool poolwillGrow;

    void Start()
    {
        int layer = (int)Mathf.Log(bulletLayer.value, 2);
        pooledObjects = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(objectToPool);

            // Collider2D required for MouseDown in Draggable but bullets collide with 
            // Put bullets in a different layer to prevent collision with turrets
            tmp.layer = layer;

            // for organization
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }

    // FIXME more efficient arrays
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
