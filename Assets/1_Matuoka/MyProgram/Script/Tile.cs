using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isOn = false;
    private Material material;

    private void Awake()
    {
        material = this.GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        // ジェネラルコライダーの引数用変数に自身のデータを格納
        GetComponent<GeneralCollider3D>().SetAttribute(new TilesGeneralColliderAttribute(this));
    }

    public void ChangeIsOn()
    {
        isOn = !isOn;
    }

    public void ChangeMaterial(Color color)
    {
        material.color = color;
    }
}


/// <summary>
/// タイルを押せるオブジェクト用のインターフェース
/// </summary>
public interface I_TileHit
{

}

/// <summary>
/// Tileを使う場合の GeneralColliderの引数
/// </summary>
public class TilesGeneralColliderAttribute: GeneralColliderAttribute
{
    public Tile tile;

    public TilesGeneralColliderAttribute(Tile tile)
    {
        this.tile = tile;
    }
}