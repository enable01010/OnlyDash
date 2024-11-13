using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTiles : MonoBehaviour, I_GeneralColliderUser
{
    [Header("��������")]
    [SerializeField] private float PADDING_LEFT = 0.5f;
    [SerializeField] private float PADDING_RIGHT = 0.5f;
    [SerializeField] private float PADDING_TOP = 0.5f;
    [SerializeField] private float PADDING_BOTTOM = 0.5f;

    [SerializeField, Tooltip("x_z")] private Vector2 SPACE = new Vector3(0.5f, 0.5f);

    [SerializeField, Range(2, 6)] private int TILE_COUNT_X = 3;
    [SerializeField, Range(2, 6)] private int TILE_COUNT_Z = 3;

    [SerializeField] private Color TILE_COLOR_OFF;
    [SerializeField] private Color TILE_COLOR_ON;

    [SerializeField] private GameObject frame;
    [SerializeField] private GameObject tileParent;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField, ReadOnly] private List<GameObject> tilesObj = new();
    [SerializeField, ReadOnly] private List<Tile> tilesTile = new();
    [SerializeField, ReadOnly] private List<bool> tilesIsOn = new();


    void Start()
    {
        InitTiles();
    }

# if UNITY_EDITOR
    private void OnValidate()
    {
        // �N�������Ă΂��
        InstantiateTilesEditor();
    }
# endif

    /// <summary>
    /// �������Ĉʒu����(Editor�p)
    /// </summary>
    private void InstantiateTilesEditor()
    {
        // tiles��null������
        tilesObj.RemoveAll(item => item == null);

        // ���Ȃ��Ȃ瑝�₷
        if (tilesObj.Count < TILE_COUNT_X * TILE_COUNT_Z)
        {
            while (tilesObj.Count != TILE_COUNT_X * TILE_COUNT_Z)
            {
                GameObject obj = Instantiate(tilePrefab, tileParent.transform);
                //obj.transform.parent = tileParent.transform;// Awake�֌W�Ń_���I�I
                obj.name = "Tile(" + tilesObj.Count + ")";
                tilesObj.Add(obj);
            }
        }

        // �\���E��\���̐؂�ւ�
        for (int i = 0; i < tilesObj.Count; i++)
        {
            tilesObj[i].SetActive(i < TILE_COUNT_X * TILE_COUNT_Z);
        }

        SetPositionTiles();
    }

    /// <summary>
    /// ��������
    /// </summary>
    private void InitTiles()
    {
        // �S�ď���
        tilesObj.Clear();
        foreach (Transform child in tileParent.transform)
        {
            Destroy(child.gameObject);
        }

        // �������Ĉʒu����
        InstantiateTilesEditor();

        // Tile�擾�E�����ݒ�
        foreach (GameObject tileObj in tilesObj)
        {
            Tile tile = tileObj.GetComponent<Tile>();
            tile.InitColor(TILE_COLOR_OFF, TILE_COLOR_ON);
            tile.ChangeIsOn(false);
            tilesTile.Add(tile);

            tilesIsOn.Add(true);
        }
    }

    /// <summary>
    /// �ʒu����
    /// </summary>
    private void SetPositionTiles()
    {
        // ���ォ��E��Ɍ������Ĕz�u�@���̌㉺�i�ɔz�u
        float frameWidth = this.transform.localScale.x;
        float frameHeight = this.transform.localScale.z;

        float tileWidth = (frameWidth - (PADDING_LEFT + PADDING_RIGHT + SPACE.x * (TILE_COUNT_X - 1))) / TILE_COUNT_X;
        float tileHeight = (frameHeight - (PADDING_TOP + PADDING_BOTTOM + SPACE.y * (TILE_COUNT_Z - 1))) / TILE_COUNT_Z;
        Vector3 tileScale = new Vector3(tileWidth / frameWidth, tilePrefab.transform.localScale.y, tileHeight / frameHeight);

        Vector3 leftUpPos = frame.transform.position;
        leftUpPos.x = leftUpPos.x - frameWidth / 2f + PADDING_LEFT + tileWidth / 2f;
        leftUpPos.z = leftUpPos.z + frameHeight / 2f - PADDING_TOP - tileHeight / 2f;

        for (int i = 0; i < TILE_COUNT_Z; i++)
        {
            for (int j = 0; j < TILE_COUNT_X; j++)
            {
                GameObject obj = tilesObj[i * TILE_COUNT_X + j];
                obj.transform.position = leftUpPos;
                obj.transform.AddX_Position((tileWidth + SPACE.x) * j);
                obj.transform.AddZ_Position(-(tileHeight + SPACE.y) * i);

                obj.transform.localScale = tileScale;
            }
        }
    }

    public virtual void OnEnter_GeneralCollider(Collider other, GeneralColliderAttribute attribute)
    {
        if (other.TryGetComponent<I_TileHit>(out _))
        {
            // �A�g���r���[�g���w��̌^�ɃL���X�g
            var tileAttribute = attribute as TilesGeneralColliderAttribute;
            if (tileAttribute != null)
            {
                // �^�C���̐؂�ւ�
                Tile tempTile = tileAttribute.tile;
                tempTile.ChangeIsOn();

                // �S�Ĉ�v���Ă�����
                if (CheckColor()) AllOff();
            }
        }
    }

    /// <summary>
    /// �S�Ĉ�v���Ă�����true
    /// </summary>
    /// <returns></returns>
    private bool CheckColor()
    {
        for(int i = 0; i < tilesTile.Count; i++)
        {
            if (tilesTile[i].isOn != tilesIsOn[i])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// �S�Ẵ^�C��������
    /// </summary>
    private void AllOff()
    {
        foreach (Tile tile in tilesTile)
        {
            tile.ChangeIsOn(false);
        }
    }
}