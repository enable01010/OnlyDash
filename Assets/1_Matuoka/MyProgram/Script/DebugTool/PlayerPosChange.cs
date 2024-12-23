#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static PlayerPosChange;

public class PlayerPosChange : DebugToolBase
{
    #region Field

    public enum PlayerPrefsEnum
    {
        PosName = 0,
        PlayerPosX,
        PlayerPosY,
        PlayerPosZ,
        CameraEulerAngleX,
        CameraEulerAngleY,
    }

    [SerializeField] private Transform content;
    [SerializeField] private InputField inputField;
    [SerializeField] private Button savePosButton;

    public class SavePosData
    {
        public string saveName;
        public Vector3 playerPos;
        public Vector3 cameraEulerAngle;
        public GameObject buttonObj;
        public Button button;
        public Text buttonText;
    }

    private List<SavePosData> savePosData = new();
    private GameObject baseButton;

    #endregion


    #region DebugToolBaseMethod

    public override void DebugToolAwake()
    {
        SetUiAction();
        DataLoad();
    }

    //public override void DebugToolStart()
    //{
        
    //}

    public override void DebugToolOpen()
    {
        // キャラクターの移動がバグるため暫定対応で移動用のコンポーネントを無効化
        Player.instance.GetController().enabled = false;
    }

    public override void DebugToolClose()
    {
        // キャラクターの移動がバグるため暫定対応で移動用のコンポーネントを有効化
        Player.instance.GetController().enabled = true;
    }

    #endregion


    #region CustomMethod

    /// <summary>
    /// UIのボタンが押された際の処理の設定
    /// </summary>
    private void SetUiAction()
    {
        // ポジションセーブ機能の新規登録用ボタン
        savePosButton.onClick.AddListener(SaveButton);
    }

    /// <summary>
    /// データのロード(ボタンの生成)
    /// </summary>
    private void DataLoad()
    {
        baseButton = content.GetChild(0).gameObject;

        int index = 0;
        while (SavePosDataManager.TryLoad(index, out var data))
        {
            // ボタンの生成
            InstantiateButton(data);

            // リストに登録
            savePosData.Add(data);

            index++;
        }
    }

    /// <summary>
    /// 新たなセーブポイントの追加(ボタンに設定する関数)
    /// </summary>
    public void SaveButton()
    {
        int index = savePosData.Count;

        // セーブするデータの作成
        SavePosData data = new()
        {
            saveName = inputField.text,
            playerPos = Player.instance.transform.position,
            cameraEulerAngle = Player.instance.DebugCameraAngleGet()
        };

        // セーブ実施
        data.Save(index);

        // ボタンの生成
        InstantiateButton(data);

        //リストに登録
        savePosData.Add(data);

        // 入力欄を空にする
        inputField.text = "";
    }


    /// <summary>
    /// ボタンの生成
    /// </summary>
    /// <param name="data"> セーブされているデータ </param>
    private void InstantiateButton(SavePosData data)
    {
        // ボタン生成
        GameObject @obj = Instantiate(baseButton, content);
        @obj.SetActive(true);

        // ボタンの関数設定
        Button @button = @obj.GetComponent<Button>();
        @button.onClick.AddListener(() => PlayerWarp(data.playerPos, data.cameraEulerAngle));

        // ボタンのText変更
        Text @text = @obj.GetComponentInChildren<Text>();
        @text.text = data.saveName;

        // Deleteボタンの関数設定
        obj.transform.GetChild(1).GetComponent<Button>().onClick
            .AddListener(() => DeleteButton(data.saveName));

        // Listに追加
        data.buttonObj = @obj;
        data.button = @button;
        data.buttonText = @text;
    }

    /// <summary>
    /// セーブを消す(ボタンに設定する関数)
    /// </summary>
    /// <param name="name"></param>
    public void DeleteButton(string name)
    {
        // データをリストから削除
        int index = 0;
        for (; index < savePosData.Count; index++)
        {
            if (savePosData[index].saveName.Equals(name))
            {
                break;
            }
        }
        Destroy(savePosData[index].buttonObj);// ボタンの削除
        savePosData.RemoveAt(index);

        // PlayerPrefsの末尾を削除
        SavePosDataManager.Delete(savePosData.Count);

        // PlayerPrefsの更新(上書き)
        for (int i = 0; i < savePosData.Count; i++)
        {
            savePosData[i].Save(i);
        }
    }

    /// <summary>
    /// Playerをワープさせる
    /// </summary>
    /// <param name="playerPos"></param>
    /// <param name="cameraAngle"></param>
    private void PlayerWarp(Vector3 playerPos, Vector3 cameraAngle)
    {
        Player.instance.transform.position = playerPos;
        Player.instance.DebugCameraAngleSet(cameraAngle);
        // TODO:プレイヤーの回転をする場合ここ
    }

    #endregion
}

/// <summary>
/// デバッグ用のプレイヤー位置情報データ処理をする用のクラス
/// </summary>
public static class SavePosDataManager
{

    /// <summary>
    /// 指定されたインデックスにデータがある場合取得
    /// </summary>
    /// <param name="index">インデックス</param>
    /// <param name="answer">取得したデータ</param>
    /// <returns>成功したか</returns>
    public static bool TryLoad(int index, out SavePosData answer)
    {
        // return出来ないので一時的に入れ込む
        answer = null;

        // 名前を取得
        string name = PlayerPrefs.GetString(PlayerPrefsEnum.PosName.ToString() + index);

        // 保存データが無ければ返却
        if (string.IsNullOrEmpty(name)) return false;


        answer = new SavePosData();

        // 名前の登録
        answer.saveName = name;

        // Playerの位置を取得
        Vector3 @Vector = Vector3.zero;
        @Vector.x = PlayerPrefs.GetFloat(PlayerPrefsEnum.PlayerPosX.ToString() + index);
        @Vector.y = PlayerPrefs.GetFloat(PlayerPrefsEnum.PlayerPosY.ToString() + index);
        @Vector.z = PlayerPrefs.GetFloat(PlayerPrefsEnum.PlayerPosZ.ToString() + index);
        answer.playerPos = @Vector;

        // Cameraの角度を取得
        @Vector.x = PlayerPrefs.GetFloat(PlayerPrefsEnum.CameraEulerAngleX.ToString() + index);
        @Vector.y = PlayerPrefs.GetFloat(PlayerPrefsEnum.CameraEulerAngleY.ToString() + index);
        @Vector.z = 0;
        answer.cameraEulerAngle = @Vector;

        return true;
    }

    /// <summary>
    /// クラス内の情報でデータを保存
    /// </summary>
    /// <param name="data">保存するデータ</param>
    /// <param name="index">インデックス</param>
    public static void Save(this SavePosData data, int index)
    {
        PlayerPrefs.SetString(PlayerPrefsEnum.PosName.ToString() + index, data.saveName);

        // Playerの位置の保存
        PlayerPrefs.SetFloat(PlayerPrefsEnum.PlayerPosX.ToString() + index, data.playerPos.x);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.PlayerPosY.ToString() + index, data.playerPos.y);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.PlayerPosZ.ToString() + index, data.playerPos.z);

        // Cameraの位置の保存
        PlayerPrefs.SetFloat(PlayerPrefsEnum.CameraEulerAngleX.ToString() + index, data.cameraEulerAngle.x);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.CameraEulerAngleY.ToString() + index, data.cameraEulerAngle.y);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// 指定したインデックスのデータを削除する関数
    /// </summary>
    /// <param name="index">インデックス</param>
    public static void Delete(int index)
    {
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.PosName.ToString() + index);
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.PlayerPosX.ToString() + index);
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.PlayerPosY.ToString() + index);
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.PlayerPosZ.ToString() + index);
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.CameraEulerAngleX.ToString() + index);
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.CameraEulerAngleY.ToString() + index);
    }
}

#endif