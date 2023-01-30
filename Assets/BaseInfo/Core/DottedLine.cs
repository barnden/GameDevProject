using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DottedLine : MonoBehaviour
{
    [SerializeField] GameObject source;
    [SerializeField] GameObject destination;
    [SerializeField] Color lineColor;

    private GameObject line;
    private LineRenderer _renderer;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
