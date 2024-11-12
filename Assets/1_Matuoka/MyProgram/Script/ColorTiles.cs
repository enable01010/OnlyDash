using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTiles : MonoBehaviour, I_GeneralColliderUser
{
    [Header("初期生成")]
    [SerializeField] private float PADDING_LEFT = 0.5f;
    [SerializeField] private float PADDING_RIGHT = 0.5f;
    [SerializeField] private float PADDING_TOP = 0.5f;
    [SerializeField] private float PADDING_BOTTOM = 0.5f;

    [SerializeField, Tooltip("x_z")] private Vector2 SPACE = new Vector3(0.5f, 0.5f);

    [SerializeField, Range(2, 6)] private int TILE_COUNT_X = 3;
    [SerializeField, Range(2, 6)] private int TILE_COUNT_Z = 3;

    [SerializeField] private Color tileColorOff;
    [SerializeField] private Color tileColorOn;

    [SerializeField] private GameObject frame;
    [SerializeField] private GameObject tileParent;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField, ReadOnly] private List<GameObject> tilesObj = new();
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
    /// 生成して位置調整(Editor用)
    /// </summary>
    private void InstantiateTilesEditor()
    {
        // tilesのnullを消す
        tilesObj.RemoveAll(item => item == null);

        // 少ないなら増やす
        if (tilesObj.Count < TILE_COUNT_X * TILE_COUNT_Z)
        {
            while (tilesObj.Count != TILE_COUNT_X * TILE_COUNT_Z)
            {
                GameObject obj = Instantiate(tilePrefab, tileParent.transform);
                //obj.transform.parent = tileParent.transform;// Awake関係でダメ！！
                obj.name = "Tile(" + tilesObj.Count + ")";
                tilesObj.Add(obj);
            }
        }

        // 表示・非表示の切り替え
        for (int i = 0; i < tilesObj.Count; i++)
        {
            tilesObj[i].SetActive(i < TILE_COUNT_X * TILE_COUNT_Z);
        }

        SetPositionTiles();
    }

    private void InitTiles()
    {
        // 全て消す
        tilesObj.Clear();
        foreach (Transform child in tileParent.transform)
        {
            Destroy(child.gameObject);
        }

        // 生成して位置調整
        InstantiateTilesEditor();

        // Material・Tile
        tilesMaterial.Clear();
        foreach (GameObject tile in tilesObj)
        {
            Material material = tile.GetComponent<MeshRenderer>().material;
            material.color = tileColorOff;
            tilesMaterial.Add(material);

            tilesTile.Add(tile.GetComponent<Tile>());
        }
    }

    /// <summary>
    /// 位置調整
    /// </summary>
    private void SetPositionTiles()
    {
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
            // アトリビュートを指定の型にキャスト
            var tileAttribute = attribute as TilesGeneralColliderAttribute;
            if (tileAttribute != null)
            {
                Tile tempTile = tileAttribute.tile;
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

        if (CheckColor())
        {
            AllOff();
        }
    }


    private bool CheckColor()
    {
        foreach (Tile tile in tilesTile)
        {
            if (tile.isOn == false)
            {
                return false;
            }
        }

        return true;
    }

    private void AllOff()
    {
        
        foreach (Tile tile in tilesTile)
        {
            tile.isOn = false;
            tile.ChangeMaterial(tileColorOff);
        }
    }
}