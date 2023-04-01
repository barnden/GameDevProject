using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngineInternal;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Platform : MonoBehaviour
{
    [System.Serializable]
    public class Towers
    {
        public GameObject[] towerArr;
    }

    [SerializeField] private CoreData coreData;
    [SerializeField] private float coreRadius;
    [SerializeField] private float[] baseRadii;
    [SerializeField] private Towers[] towers; //towers[ringNum].towerArr[towerNum]
    [SerializeField] private Sprite[] baseSprites;
    
    private Dictionary<(int ringNum, int towerNum), GameObject> placedTowers = new Dictionary<(int ringNum, int towerNum), GameObject>();

    private void Update()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = baseSprites[coreData.getLevel()];
    }

    public Tuple<Vector2, int, int> getSnap(Vector2 cursorPos)
    {
        if (pointInBase(cursorPos) && !pointInCore(cursorPos))
        {
            float closestDist = float.PositiveInfinity;
            GameObject closestTowerPos = null;
            int ringNum = -1;
            int towerNum = -1;
            for (int ring = 0; ring <= coreData.getLevel(); ring++)
            {
                for(int tower = 0; tower < towers[ring].towerArr.Length; tower++) {
                    float dist = Vector2.Distance(cursorPos, towers[ring].towerArr[tower].transform.position);
                    if(dist < closestDist)
                    {
                        closestDist = dist;
                        closestTowerPos = towers[ring].towerArr[tower];
                        ringNum = ring;
                        towerNum = tower;
                    }
                }
            }

            return new Tuple<Vector2, int, int>(closestTowerPos.transform.position, ringNum, towerNum);
        }

        return null;
    }

    public float getBaseRadius()
    {
        return baseRadii[coreData.getLevel()];
    }

    public bool towerExists(int ringNum, int towerNum)
    {
        return placedTowers.ContainsKey((ringNum, towerNum));
    }

    public GameObject getTower(int ringNum, int towerNum)
    {
        return placedTowers[(ringNum, towerNum)];
    }

    public void place(GameObject obj, int ringNum, int towerNum)
    {
        placedTowers.Add((ringNum, towerNum), obj);
    }

    public GameObject delete(int ringNum, int towerNum)
    {
        GameObject obj = placedTowers[(ringNum, towerNum)];
        placedTowers.Remove((ringNum, towerNum));
        return obj;
    }

    public bool pointInBase(Vector2 point)
    {
        Vector2 corePosition = gameObject.transform.position;
        float pointRadius = Vector2.Distance(point, corePosition);
        return pointRadius < getBaseRadius();
    }

    public bool pointInCore(Vector2 point)
    {
        Vector2 corePosition = gameObject.transform.position;
        float pointRadius = Vector2.Distance(point, corePosition);
        return pointRadius < coreRadius;
    }
}
