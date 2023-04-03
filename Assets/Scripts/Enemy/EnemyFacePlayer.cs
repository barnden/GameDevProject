using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFacePlayer : MonoBehaviour
{
    [SerializeField]private GameObject playerRef;
    private Vector3 currScale;
    public bool doneSpawning;
    // Start is called before the first frame update
    private void Awake()
    {
        currScale = transform.localScale;
        doneSpawning = false;
        playerRef = GameObject.FindGameObjectWithTag("Player");
        
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
        if(doneSpawning)
        {
            if (playerRef.transform.position.x < transform.position.x)
            {
                currScale.x = -1;
            }
            else
            {
                currScale.x = 1;
            }
        }
    }

    // properly facing base after spawning animation has occured
    private void LateUpdate()
    {
        if(doneSpawning)
        {
            this.transform.localScale = currScale;
        }
       
    }
    // not called, in beginning of move animation, donespawning is set to true
    public void DoneSpawning()
    {
        doneSpawning = true;
    }
}
