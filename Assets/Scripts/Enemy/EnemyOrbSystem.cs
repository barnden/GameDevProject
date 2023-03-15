using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOrbSystem : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject orb;

    public void DropOrb()
    {
        GameObject drop = Instantiate(orb);
        drop.transform.position = transform.position;

        // FIXME: Remove this
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
