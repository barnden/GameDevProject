using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEBullet : BaseBullet
{
    public float bSize;

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

        // transform.Translate(Time.deltaTime * bSpeed * direction);
        float scale = (1 - (blifeTime / bMaxLifeTime));
        scale *= scale * bSize;
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}
