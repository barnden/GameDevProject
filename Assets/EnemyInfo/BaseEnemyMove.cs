using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyMove : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject playerToFollow;
    public float baseEnemySpeed;

    private void OnEnable()
    {
        playerToFollow = GameObject.FindGameObjectWithTag("Player");
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(baseEnemySpeed * Time.deltaTime * (playerToFollow.transform.position - transform.position).normalized);
    }
}
