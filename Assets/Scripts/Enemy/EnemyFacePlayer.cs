using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFacePlayer : MonoBehaviour
{
    private GameObject playerRef;
    private bool flipped = false;
    private Vector3 currScale;
    // Start is called before the first frame update
    void Start()
    {
        flipped = false;
        playerRef = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        currScale = transform.localScale;
        if(playerRef.transform.position.x < transform.position.x)
        {
            currScale.x = Mathf.Abs(currScale.x) * -1 * (flipped? -1:1);
        }
        else
        {
            currScale.x = Mathf.Abs(currScale.x) * (flipped ? -1 : 1);
        }
        transform.localScale = currScale;
    }
}
