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

    [SerializeField] private int TILE_COUNT_X = 3;
    [SerializeField] private int TILE_COUNT_Z = 3;

    [SerializeField] private Color tileColorOff;
    [SerializeField] private Color tileColorOn;

    [SerializeField] private GameObject frame;
    [SerializeField] private GameObject tileParent;
    [SerializeField] private GameObject tile;
    [SerializeField, ReadOnly] private List<GameObject> tiles = new();
    [SerializeField, ReadOnly] private List<Material> tilesMaterial = new();
    [SerializeField, ReadOnly] private List<Tile> tilesTile = new();


    void Start()
    {
        InitTiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

# if UNITY_EDITOR
    private void OnValidate()
    {
        InstantiateTilesEditor();
    }
# endif

    /// <summary>
    /// �������Ĉʒu����(Editor�p)
    /// </summary>
    private void InstantiateTilesEditor()
    {
        // tiles��null������
        tiles.RemoveAll(item => item == null);

        // ���Ȃ��Ȃ瑝�₷
        if (tiles.Count < TILE_COUNT_X * TILE_COUNT_Z)
        {
            while (tiles.Count != TILE_COUNT_X * TILE_COUNT_Z)
            {
                GameObject obj = Instantiate(tile, tileParent.transform);
                //obj.transform.parent = tileParent.transform;// Awake�֌W�Ń_���I�I
                obj.name = "Tile(" + tiles.Count + ")";
                tiles.Add(obj);
            }
        }

        // �\���E��\���̐؂�ւ�
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].SetActive(i < TILE_COUNT_X * TILE_COUNT_Z);
        }

        SetPositionTiles();
    }

    private void InitTiles()
    {
        // �S�ď���
        tiles.Clear();
        foreach (Transform child in tileParent.transform)
        {
            Destroy(child.gameObject);
        }

        // �������Ĉʒu����
        InstantiateTilesEditor();

        // Material�ETile
        tilesMaterial.Clear();
        foreach (GameObject tile in tiles)
        {
            Material material = tile.GetComponent<MeshRenderer>().material;
            material.color = tileColorOff;
            tilesMaterial.Add(material);

            tilesTile.Add(tile.GetComponent<Tile>());
        }
    }

    /// <summary>
    /// �ʒu����
    /// </summary>
    private void SetPositionTiles()
    {
        float frameWidth = this.transform.localScale.x;
        float frameHeight = this.transform.localScale.z;

        float tileWidth = (frameWidth - (PADDING_LEFT + PADDING_RIGHT + SPACE.x * (TILE_COUNT_X - 1))) / TILE_COUNT_X;
        float tileHeight = (frameHeight - (PADDING_TOP + PADDING_BOTTOM + SPACE.y * (TILE_COUNT_Z - 1))) / TILE_COUNT_Z;
        Vector3 tileScale = new Vector3(tileWidth / frameWidth, tile.transform.localScale.y, tileHeight / frameHeight);

        Vector3 leftUpPos = frame.transform.position;
        leftUpPos.x = leftUpPos.x - frameWidth / 2f + PADDING_LEFT + tileWidth / 2f;
        leftUpPos.z = leftUpPos.z + frameHeight / 2f - PADDING_TOP - tileHeight / 2f;

        for (int i = 0; i < TILE_COUNT_Z; i++)
        {
            for (int j = 0; j < TILE_COUNT_X; j++)
            {
                GameObject obj = tiles[i * TILE_COUNT_X + j];
                obj.transform.position = leftUpPos;
                obj.transform.AddX_Position((tileWidth + SPACE.x) * j);
                obj.transform.AddZ_Position(-(tileHeight + SPACE.y) * i);

                obj.transform.localScale = tileScale;
            }
        }
    }

    public virtual void OnEnter_GeneralCollider(Collider other, Transform generalCollider)
    {
        if (other.TryGetComponent<Player>(out _))// �C���I�I�I�I�I�I�I�I�I
        {
            Tile tempTile = generalCollider.GetComponent<Tile>();
            tempTile.ChangeIsOn();

            if (tempTile.isOn == true)
            {
                tempTile.ChangeMaterial(tileColorOn);
            }
            else
            {
                tempTile.ChangeMaterial(tileColorOff);
            }
        }
    }
}
