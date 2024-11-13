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
    /// ColorTilesの色を入れる
    /// </summary>
    /// <param name="offColor"></param>
    /// <param name="onColor"></param>
    public void InitColor(ColorTiles owner)
    {
        this.owner = owner;
    }

    /// <summary>
    /// 切り替えて色を変える
    /// </summary>
    public void ChangeIsOn()
    {
        isOn = !isOn;

        if (isOn) ChangeMaterial(owner.TILE_COLOR_ON);
        else ChangeMaterial(owner.TILE_COLOR_OFF);
    }

    /// <summary>
    /// 引数の状態に切り替えて色を変える
    /// </summary>
    /// <param name="temp"></param>
    public void ChangeIsOn(bool temp)
    {
        isOn = temp;

        if (isOn) ChangeMaterial(owner.TILE_COLOR_ON);
        else ChangeMaterial(owner.TILE_COLOR_OFF);
    }

    /// <summary>
    /// Materialの色を変える
    /// </summary>
    /// <param name="color"></param>
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