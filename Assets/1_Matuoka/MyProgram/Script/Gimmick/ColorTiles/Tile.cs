using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isOn { get; private set; } = false;
    private Material material;
    private ColorTiles owner;

    private void Awake()
    {
        material = this.GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        // ジェネラルコライダーの引数用変数に自身のデータを格納
        GetComponent<GeneralCollider3D>().SetAttribute(new TilesGeneralColliderAttribute(this));
    }

    /// <summary>
    /// ColorTilesを入れる
    /// </summary>
    public void Init(ColorTiles owner)
    {
        this.owner = owner;
    }

    /// <summary>
    /// 切り替えて色を変える
    /// </summary>
    public void TileHit()
    {
        isOn = !isOn;

        if (isOn) ChangeColor(owner.TILE_COLOR_ON);
        else ChangeColor(owner.TILE_COLOR_OFF);
    }

    /// <summary>
    /// 初期状態にする
    /// </summary>
    /// <param name="temp"></param>
    public void SetFirstSetting()
    {
        isOn = false;

        if (isOn) ChangeColor(owner.TILE_COLOR_ON);
        else ChangeColor(owner.TILE_COLOR_OFF);
    }

    /// <summary>
    /// 色を変える
    /// </summary>
    /// <param name="color"></param>
    public void ChangeColor(Color color)
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