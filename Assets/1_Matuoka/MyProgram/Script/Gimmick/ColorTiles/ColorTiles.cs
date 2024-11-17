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
    /// タイルの初期化処理
    /// </summary>
    private void InitTiles()
    {
        // Tile取得・初期設定
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
            // アトリビュートを指定の型にキャスト
            var tileAttribute = attribute as TilesGeneralColliderAttribute;
            if (tileAttribute != null)
            {
                // タイルの切り替え
                Tile tempTile = tileAttribute.tile;
                tempTile.TileHit();

                // 全て一致していたら
                if (CheckColor()) AllOff();
            }
        }
    }

    /// <summary>
    /// 回答が一致しているか確認する処理
    /// </summary>
    /// <returns>true:すべて一致 false:一つでも一致していない</returns>
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
    /// 全てのタイルを消す
    /// </summary>
    private void AllOff()
    {
        foreach (Tile tile in tiles)
        {
            tile.SetFirstSetting();
        }
    }


#if UNITY_EDITOR

    [Header("初期生成")]
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
        // 実行中に数値を変えるとバグるため実行中は処理しない
        if (EditorApplication.isPlaying == true) return;

        InstantiateTilesEditor();
    }

    /// <summary>
    /// 生成して位置調整(Editor用)
    /// </summary>
    private void InstantiateTilesEditor()
    {
        // 少ないなら増やす
        int needCount = TILE_COUNT_X * TILE_COUNT_Z;
        while (tilesObj.Count < needCount)
        {
            GameObject obj = Instantiate(tilePrefab, tileParent.transform);
            //obj.transform.parent = tileParent.transform;// Awake関係でダメ！！
            obj.name = "Tile(" + tilesObj.Count + ")";
            tilesObj.Add(obj);
        }

        // 多いなら消す
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
    /// 位置調整
    /// </summary>
    private void SetPositionTiles()
    {
        // 左上から右上に向かって配置　その後下段に配置
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