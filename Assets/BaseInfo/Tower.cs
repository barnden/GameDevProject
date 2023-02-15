using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private Sprite[] sprites = new Sprite[3]; //Need 3

    public GameObject parent;
    public bool placing;
    private float towerBaseOffsetZ = -0.5f;

    private Platform platform;
    private GameObject towerBase;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        platform = GameObject.Find("Platform").GetComponent<Platform>();

        sprites[0] = Resources.Load<Sprite>("Sprites/Towers/Bases/tiny0");
        sprites[1] = Resources.Load<Sprite>("Sprites/Towers/Bases/tiny0");
        sprites[2] = Resources.Load<Sprite>("Sprites/Towers/Bases/tiny0");
        
        towerBase = new GameObject("Tower Base");
        spriteRenderer = towerBase.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[0]; //Tower base sprites

        placing = true;
    }

    private void Start()
    {
        Debug.Log(parent);
        towerBase.transform.parent = parent.transform;
    }

    private void Update()
    {
        if (placing)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 towerPos = new Vector3(mousePos.x, mousePos.y, towerBaseOffsetZ);

            if (platform.pointInBase(mousePos))
            {
                platform.setCursorPos(mousePos);
                platform.enableRing();
                
                Tuple<Vector2, float, int> snapPos = platform.getSnap();
                towerPos = new Vector3(snapPos.Item1.x, snapPos.Item1.y, towerBaseOffsetZ);

                float snapPosDeg = snapPos.Item2 / (2.0f * Mathf.PI) * 360;
                towerBase.transform.eulerAngles = new Vector3(0.0f, 0.0f, snapPosDeg);

                int ringNum = snapPos.Item3;
                spriteRenderer.sprite = sprites[ringNum];
            }
            else
            {
                towerBase.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                platform.disableRing();
            }
            
            towerBase.transform.position = towerPos;
        }
    }

    public GameObject getTowerBase()
    {
        return towerBase;
    }

    public Tuple<Vector3, bool> place()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if (platform.pointInBase(mousePos) && !platform.towerExists())
        {
            platform.disableRing();
            platform.place();
            placing = false;
            return new Tuple<Vector3, bool>(towerBase.transform.position, true);
        }

        return new Tuple<Vector3, bool>(new Vector3(), false);
    }

    private void OnDestroy()
    {
        Destroy(towerBase);
    }
}
