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
        // �W�F�l�����R���C�_�[�̈����p�ϐ��Ɏ��g�̃f�[�^���i�[
        GetComponent<GeneralCollider3D>().SetAttribute(new TilesGeneralColliderAttribute(this));
    }

    /// <summary>
    /// ColorTiles�̐F������
    /// </summary>
    /// <param name="offColor"></param>
    /// <param name="onColor"></param>
    public void InitColor(ColorTiles owner)
    {
        this.owner = owner;
    }

    /// <summary>
    /// �؂�ւ��ĐF��ς���
    /// </summary>
    public void ChangeIsOn()
    {
        isOn = !isOn;

        if (isOn) ChangeMaterial(owner.TILE_COLOR_ON);
        else ChangeMaterial(owner.TILE_COLOR_OFF);
    }

    /// <summary>
    /// �����̏�Ԃɐ؂�ւ��ĐF��ς���
    /// </summary>
    /// <param name="temp"></param>
    public void ChangeIsOn(bool temp)
    {
        isOn = temp;

        if (isOn) ChangeMaterial(owner.TILE_COLOR_ON);
        else ChangeMaterial(owner.TILE_COLOR_OFF);
    }

    /// <summary>
    /// Material�̐F��ς���
    /// </summary>
    /// <param name="color"></param>
    public void ChangeMaterial(Color color)
    {
        material.color = color;
    }
}


/// <summary>
/// �^�C����������I�u�W�F�N�g�p�̃C���^�[�t�F�[�X
/// </summary>
public interface I_TileHit
{

}

/// <summary>
/// Tile���g���ꍇ�� GeneralCollider�̈���
/// </summary>
public class TilesGeneralColliderAttribute: GeneralColliderAttribute
{
    public Tile tile;

    public TilesGeneralColliderAttribute(Tile tile)
    {
        this.tile = tile;
    }
}