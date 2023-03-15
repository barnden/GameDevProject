using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum Stats
{
    UNDEFINED, // this position is chosen since c# will default to 0 if something went wrong
    HEALTH,
    SPEED,
    FIRERATE
};