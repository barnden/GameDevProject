using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOrbSystem : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject orb;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Bullet"))
        {
            GameObject orbDropped = Instantiate(orb);
            orbDropped.transform.position = transform.position;
            
            collision.gameObject.SetActive(false);
            Destroy(gameObject);
        } else if (collision.CompareTag("AOE"))
        {
            // TODO: Do AOE effect like slow instead of killing
            GameObject orbDropped = Instantiate(orb);
            orbDropped.transform.position = transform.position;

            Destroy(gameObject);
        }
    }
}
