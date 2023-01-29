using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBullet : MonoBehaviour
{
    public float bSpeed;
    public float blifeTime;
    public Vector2 direction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        blifeTime -= Time.deltaTime;
        if (blifeTime <= 0)
        {
            this.gameObject.SetActive(false);
        }
        transform.Translate(Time.deltaTime * bSpeed * direction);
    }
}
