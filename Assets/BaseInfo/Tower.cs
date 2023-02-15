using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private Sprite[] tinySprites = new Sprite[6]; //Need 6
    private Sprite[] mediumSprites = new Sprite[3]; //Need 3
    private Sprite[] giantSprites = new Sprite[2]; //Need 2

    public enum TowerSize{Tiny, Medium, Giant};
    public TowerSize towerSize = TowerSize.Medium;

    public GameObject parent;
    public bool placing;

    private Platform platform;
    private GameObject towerBase;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        platform = GameObject.Find("Platform").GetComponent<Platform>();

        tinySprites[0] = Resources.Load<Sprite>("Sprites/Towers/Bases/tiny0");
        tinySprites[1] = Resources.Load<Sprite>("Sprites/Towers/Bases/tiny0");
        tinySprites[2] = Resources.Load<Sprite>("Sprites/Towers/Bases/tiny0");
        tinySprites[3] = Resources.Load<Sprite>("Sprites/Towers/Bases/tiny0");
        tinySprites[4] = Resources.Load<Sprite>("Sprites/Towers/Bases/tiny0");
        tinySprites[5] = Resources.Load<Sprite>("Sprites/Towers/Bases/tiny0");

        mediumSprites[0] = Resources.Load<Sprite>("Sprites/Towers/Bases/tiny0");
        mediumSprites[1] = Resources.Load<Sprite>("Sprites/Towers/Bases/tiny0");
        mediumSprites[2] = Resources.Load<Sprite>("Sprites/Towers/Bases/tiny0");

        giantSprites[0] = Resources.Load<Sprite>("Sprites/Towers/Bases/tiny0");
        giantSprites[1] = Resources.Load<Sprite>("Sprites/Towers/Bases/tiny0");

        towerBase = new GameObject("Tower Base");
        spriteRenderer = towerBase.AddComponent<SpriteRenderer>();
        towerBase.transform.parent = parent.transform;
        switch (towerSize)
        {
            case TowerSize.Tiny: spriteRenderer.sprite = tinySprites[0]; break;
            case TowerSize.Medium: spriteRenderer.sprite = mediumSprites[0]; break;
            case TowerSize.Giant: spriteRenderer.sprite = giantSprites[0]; break;
        }
        placing = true;
    }

    private void Update()
    {
        if (placing)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 towerPos = new Vector3(mousePos.x, mousePos.y, 0.0f);

            if (platform.pointInBase(mousePos))
            {
                platform.setCursorPos(mousePos);

                switch (towerSize)
                {
                    case TowerSize.Tiny: platform.setTowerRadius(0.5f); break;
                    case TowerSize.Medium: platform.setTowerRadius(1.0f); break;
                    case TowerSize.Giant: platform.setTowerRadius(1.5f); break;
                }

                platform.enableRing();

                Tuple<Vector2, float, int> snapPos = platform.getSnap();

                towerPos = new Vector3(snapPos.Item1.x, snapPos.Item1.y, 0.0f);

                float snapPosDeg = snapPos.Item2 / (2.0f * Mathf.PI) * 360;
                towerBase.transform.eulerAngles = new Vector3(0.0f, 0.0f, snapPosDeg);

                int ringNum = snapPos.Item3;
                switch (towerSize)
                {
                    case TowerSize.Tiny: spriteRenderer.sprite = tinySprites[ringNum]; break;
                    case TowerSize.Medium: spriteRenderer.sprite = mediumSprites[ringNum]; break;
                    case TowerSize.Giant: spriteRenderer.sprite = giantSprites[ringNum]; break;
                }
            }
            else
            {
                towerBase.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                platform.disableRing();
            }
            
            towerBase.transform.position = towerPos;
        }
    }

    public Vector3 place()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (platform.pointInBase(mousePos))
        {
            placing = false;
            platform.disableRing();
            return towerBase.transform.position;
        }
        return new Vector3();
    }

    private void OnDestroy()
    {
        Destroy(towerBase);
    }
}
