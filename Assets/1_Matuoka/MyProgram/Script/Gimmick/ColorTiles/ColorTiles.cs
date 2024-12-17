using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ColorTiles : MonoBehaviour, I_GeneralColliderUser
{
    [field: SerializeField] public Color TILE_COLOR_OFF { get; private set; }
    [field:SerializeField] public Color TILE_COLOR_ON { get; private set; }

    [SerializeField] private GameObject frame;
    [SerializeField] private GameObject tileParent;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField, ReadOnly] private List<GameObject> tilesObj = new();
    [SerializeField, ReadOnly] private List<Tile> tiles = new();
    [SerializeField, ReadOnly] private List<bool> answers = new();

    void Start()
    {
        InitTiles();
    }

    /// <summary>
    /// �^�C���̏���������
    /// </summary>
    private void InitTiles()
    {
        // Tile�擾�E�����ݒ�
        foreach (GameObject tileObj in tilesObj)
        {
            Tile tile = tileObj.GetComponent<Tile>();
            tile.Init(this);
            tile.SetFirstSetting();
            tiles.Add(tile);

            answers.Add(true);
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
                tempTile.TileHit();

                // �S�Ĉ�v���Ă�����
                if (CheckColor()) AllOff();
            }
        }
    }

    /// <summary>
    /// �񓚂���v���Ă��邩�m�F���鏈��
    /// </summary>
    /// <returns>true:���ׂĈ�v false:��ł���v���Ă��Ȃ�</returns>
    private bool CheckColor()
    {
        for(int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].isOn != answers[i])
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
        foreach (Tile tile in tiles)
        {
            tile.SetFirstSetting();
        }
    }


#if UNITY_EDITOR

    [Header("��������")]
    [SerializeField] private float PADDING_LEFT = 0.5f;
    [SerializeField] private float PADDING_RIGHT = 0.5f;
    [SerializeField] private float PADDING_TOP = 0.5f;
    [SerializeField] private float PADDING_BOTTOM = 0.5f;

    [SerializeField, Tooltip("x_z")] private Vector2 SPACE = new Vector3(0.5f, 0.5f);

    [SerializeField, Range(2, 6)] private int TILE_COUNT_X = 3;
    [SerializeField, Range(2, 6)] private int TILE_COUNT_Z = 3;

    bool isDestroyChildObj = false;

    private void OnValidate()
    {
        // ���s���ɐ��l��ς���ƃo�O�邽�ߎ��s���͏������Ȃ�
        if (EditorApplication.isPlaying == true) return;

        InstantiateTilesEditor();
    }

    /// <summary>
    /// �������Ĉʒu����(Editor�p)
    /// </summary>
    private void InstantiateTilesEditor()
    {
        // ���Ȃ��Ȃ瑝�₷
        int needCount = TILE_COUNT_X * TILE_COUNT_Z;
        while (tilesObj.Count < needCount)
        {
            GameObject obj = Instantiate(tilePrefab, tileParent.transform);
            //obj.transform.parent = tileParent.transform;// Awake�֌W�Ń_���I�I
            obj.name = "Tile(" + tilesObj.Count + ")";
            tilesObj.Add(obj);
        }

        // �����Ȃ����
        if (needCount < tilesObj.Count && isDestroyChildObj == false)
        {
            isDestroyChildObj = true;
            EditorApplication.delayCall += () =>
            {
                isDestroyChildObj = false;

                for (int i = needCount; i < tilesObj.Count; i++)
                {
                    DestroyImmediate(tilesObj[i]);
                }

                tilesObj.RemoveAll(item => item == null);
            };

        }
        
        SetPositionTiles();
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

#endif

}