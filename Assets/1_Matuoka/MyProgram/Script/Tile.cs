using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isOn { get; private set; } = false;
    private Material material;
    private Color offColor;
    private Color onColor;

    private void Awake()
    {
        material = this.GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        // ジェネラルコライダーの引数用変数に自身のデータを格納
        GetComponent<GeneralCollider3D>().SetAttribute(new TilesGeneralColliderAttribute(this));
    }

    public void InitColor(Color offColor, Color onColor)
    {
        this.offColor = offColor;
        this.onColor = onColor;
    }

    public void ChangeIsOn()
    {
        isOn = !isOn;

        if (isOn) ChangeMaterial(onColor);
        else ChangeMaterial(offColor);
    }

    public void ChangeIsOn(bool temp)
    {
        isOn = temp;

        if (isOn) ChangeMaterial(onColor);
        else ChangeMaterial(offColor);
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