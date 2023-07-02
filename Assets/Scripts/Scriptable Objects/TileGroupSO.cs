using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu()]
public class TileGroupSO : ScriptableObject
{
    public TileBase[] tileGroupList;
    public string tileGroupName;
}
