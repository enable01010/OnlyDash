using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using static Player;
using static DebugTool;
using UnityEngine.SceneManagement;
/// <summary>
/// SwitchBlock
/// </summary>

public class DebugTool : MonoBehaviour
{
#if UNITY_EDITOR

    #region Field

    [SerializeField] private bool isStartOpen = false;
    private bool isDebugToolOpen = false;
    [SerializeField] private GameObject uiWindow;

    public enum PlayerPrefsEnum
    {
        TimeScale = 0,
        PlayerMove,
        PosName,
        PlayerPosX,
        PlayerPosY,
        PlayerPosZ,
        CameraEulerAngleX,
        CameraEulerAngleY,
    }

    private float timeScale;
    [SerializeField] private Slider timeScaleSlider;
    [SerializeField] private Text timeScaleText;
    [SerializeField] private Button timeScaleResetButton;
    private const int CONSTANT_VELOCITY = 10;
    private const string TIME_SCALE_TEXT_FORMAT = "x {0}倍";

    private enum PlayerMove
    {
        ControlledMove = 0,
        AutoMove,
    }
    private PlayerMove playerMove;
    [SerializeField] private Button[] moveButton;


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


    #region MonoBehaviourMethod

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        DataLoad();
        SetUiActions();
    }

    private void Start()
    {
        DataSet();
        ResetColorPlayerMoveButton();

        if (isStartOpen) ChangeDebugTool();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) == false) return;
        ChangeDebugTool();
    }

    #endregion


    #region CustomMethod

    /// <summary>
    /// データのロード
    /// </summary>
    private void DataLoad()
    {
        timeScale = PlayerPrefs.GetFloat(PlayerPrefsEnum.TimeScale.ToString(), 1f);
        playerMove = (PlayerMove)PlayerPrefs.GetInt(PlayerPrefsEnum.PlayerMove.ToString());
        SavePosLoad();
    }

    /// <summary>
    /// UIのボタンが押された際の処理の設定
    /// </summary>
    private void SetUiActions()
    {
        // タイムスケールのスライダーが変更された際の処理
        timeScaleSlider.onValueChanged.AddListener(value => {
            timeScale = value / CONSTANT_VELOCITY;
            timeScaleText.text = LibFuncUtility.TextFormatBuilder(TIME_SCALE_TEXT_FORMAT, timeScale.ToString("F1"));
        });

        // タイムスケールのリセットボタンが押された際の処理
        timeScaleResetButton.onClick.AddListener(() => timeScaleSlider.value = CONSTANT_VELOCITY);

        // ボタンに関数を登録
        for (int i = 0; i < moveButton.Length; i++)
        {
            int count = i;
            moveButton[i].onClick
                .AddListener(() => PlayerMoveButton((PlayerMove)count));
        }

        // ポジションセーブ機能の新規登録用ボタン
        savePosButton.onClick.AddListener(SavePosSaveButton);
    }

    /// <summary>
    /// データの反映
    /// </summary>
    private void DataSet()
    {
        TimeScaleChange();
        PlayerMoveChange();
    }

    /// <summary>
    /// データのセーブ
    /// </summary>
    private void DataSave()
    {
        PlayerPrefs.SetFloat(PlayerPrefsEnum.TimeScale.ToString(), timeScale);
        PlayerPrefs.SetInt(PlayerPrefsEnum.PlayerMove.ToString(), (int)playerMove);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// DebugToolのON・OFF切り替え
    /// </summary>
    private void ChangeDebugTool()
    {
        if (isDebugToolOpen == false)
        {
            DebugToolOpen();
        }
        else
        {
            DebugToolClose();
        }

        isDebugToolOpen = !isDebugToolOpen;
        uiWindow.SetActive(isDebugToolOpen);
    }

    /// <summary>
    /// DebugToolを開く処理
    /// </summary>
    private void DebugToolOpen()
    {
        Time.timeScale = 0f;
        Player.instance.GetController().enabled = false;
    }

    /// <summary>
    /// DebugToolを閉じる処理
    /// </summary>
    private void DebugToolClose()
    {
        DataSave();
        DataSet();

        Player.instance.GetController().enabled = true;
    }

    /// <summary>
    /// TimeScaleの変更
    /// </summary>
    private void TimeScaleChange()
    {
        Time.timeScale = timeScale;
        timeScaleSlider.value = timeScale * CONSTANT_VELOCITY;
    }

    /// <summary>
    /// 移動方法の変更
    /// </summary>
    private void PlayerMoveChange()
    {
        switch (playerMove)
        {
            case PlayerMove.ControlledMove:
                Player.instance.ChangeMove(new ControlledMove());
                break;

            case PlayerMove.AutoMove:
                Player.instance.ChangeMove(new AutoMove());
                break;

            default:
                Debug.LogError("moveを設定できません");
                break;
        }
    }

    /// <summary>
    /// 移動方法を切り替えるボタン
    /// </summary>
    private void PlayerMoveButton(PlayerMove playerMove)
    {
        this.playerMove = playerMove;
        ResetColorPlayerMoveButton();
    }


    /// <summary>
    /// プレイヤーの移動方法指定ボタンの色を調整する処理
    /// </summary>
    private void ResetColorPlayerMoveButton()
    {
        int length = Enum.GetValues(typeof(PlayerMove)).Length;
        for (int i = 0; i < length; i++)
        {
            Color col = (i == (int)playerMove) ? Color.yellow : Color.white;

            ColorBlock colorBlock = moveButton[i].colors;
            colorBlock.normalColor = col;
            colorBlock.highlightedColor = col;
            colorBlock.selectedColor = col;

            moveButton[i].colors = colorBlock;
        }
    }

    private void InstantiateSaveButton(SavePosData data)
    {
        // ボタン生成
        GameObject @obj = Instantiate(baseButton, content);
        @obj.SetActive(true);

        // ボタンの関数設定
        Button @button = @obj.GetComponent<Button>();
        @button.onClick.AddListener(() => SavePosButtonWarp(data.playerPos, data.cameraEulerAngle));

        // ボタンのText変更
        Text @text = @obj.GetComponentInChildren<Text>();
        @text.text = data.saveName;

        // Deleteボタンの関数設定
        obj.transform.GetChild(1).GetComponent<Button>().onClick
            .AddListener(() => SavePosDeleteButton(data.saveName));

        // Listに追加
        data.buttonObj = @obj;
        data.button = @button;
        data.buttonText = @text;
    }

    /// <summary>
    /// 起動時のボタンの生成
    /// </summary>
    private void SavePosLoad()
    {
        baseButton = content.GetChild(0).gameObject;

        int index = 0;
        while (PlayerPrefsManager.TryLoad(index,out var data))
        {
            // ボタンの生成
            InstantiateSaveButton(data);

            // リストに登録
            savePosData.Add(data);

            index++;
        }
    }

    /// <summary>
    /// 新たなセーブポイントの追加
    /// </summary>
    public void SavePosSaveButton()
    {
        int index = savePosData.Count;

        // セーブするデータの作成
        Vector3 @Vector = Player.instance.transform.position;
        SavePosData data = new SavePosData
        {
            saveName = inputField.text,
            playerPos = Player.instance.transform.position,
            cameraEulerAngle = Player.instance.DebugCameraAngleSGet()
        };

        // セーブ実施
        data.Save(index);

        // ボタンの生成
        InstantiateSaveButton(data);

        //リストに登録
        savePosData.Add(data);

        // 入力欄を空にする
        inputField.text = "";
    }

    /// <summary>
    /// セーブを消す
    /// </summary>
    /// <param name="name"></param>
    public void SavePosDeleteButton(string name)
    {
        // データをリストから削除
        int index = 0;
        for(; index < savePosData.Count; index++)
        {
            if (savePosData[index].saveName.Equals(name))
            {
                break;
            }
        }
        Destroy(savePosData[index].buttonObj);
        savePosData.RemoveAt(index);

        // PlayerPrefsの末尾を削除
        PlayerPrefsManager.Delete(savePosData.Count);

        // PlayerPrefsの更新(上書き)
        for (int i = 0; i < savePosData.Count; i++)
        {
            savePosData[i].Save(i);
        }
    }

    /// <summary>
    /// ボタンを押したらワープする
    /// </summary>
    /// <param name="playerPos"></param>
    /// <param name="cameraPos"></param>
    private void SavePosButtonWarp(Vector3 playerPos, Vector3 cameraAngle)
    {
        Player.instance.transform.position = playerPos;
        Player.instance.DebugCameraAngleSet(cameraAngle);
    }

    #endregion

#else

    private void Awake()
    {
        Destroy(this.gameObject);
    }

#endif
}

#if UNITY_EDITOR

/// <summary>
/// デバッグ用のプレイヤー位置情報データ処理をする用のクラス
/// </summary>
public static class PlayerPrefsManager
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
    public static void Save(this SavePosData data,int index)
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