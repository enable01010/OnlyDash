using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using static Player;
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

    private enum PlayerPrefsEnum
    {
        TimeScale = 0,
        PlayerMove,
        SavePosName,
        SavePosX,
        SavePosY,
        SavePosZ,
        SaveCameraPosX,
        SaveCameraPosY,
        SaveCameraPosZ,
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


    [SerializeField] private GameObject content;
    [SerializeField] private InputField inputField;
    [SerializeField] private Button savePosButton;
    [SerializeField, Range(1, 10)] private int SAVE_LIMIT = 10;
    private List<string> posNames = new();
    public List<Vector3> posPlayerVectors = new();
    public List<Vector3> posCameraVectors = new();
    private List<GameObject> posButtonsObj = new();
    private List<Button> posButtons = new();
    private List<Text> posButtonsText = new();
    private GameObject playerFollowCamera;

    // DebugTool開くときに代入、開いている間Updateで代入
    private Vector3 playerPos;
    private Vector3 cameraPos;

    #endregion


    #region MonoBehaviourMethod

    private void Awake()
    {
        DataLoad();
        SetUiActions();
        posButtonsObj.Add(content.transform.GetChild(0).gameObject);
        SavePosButtonInstantiate();
    }

    private void Start()
    {
        DataSet();
        ResetColorPlayerMoveButton();

        playerFollowCamera = GameObject.Find("PlayerFollowCamera");
        if (isStartOpen) ChangeDebugTool();
    }

    private void Update()
    {
        if (isDebugToolOpen == true) Player.instance.transform.position = playerPos;
        if (isDebugToolOpen == true) playerFollowCamera.transform.position = cameraPos;

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
        playerPos = Player.instance.transform.position;
        cameraPos = playerFollowCamera.transform.position;
    }

    /// <summary>
    /// DebugToolを閉じる処理
    /// </summary>
    private void DebugToolClose()
    {
        DataSave();
        DataSet();
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

    /// <summary>
    /// 起動時のPlayerPrefsからのデータ取得
    /// </summary>
    private void SavePosLoad()
    {
        for (int i = 0; i < SAVE_LIMIT; i++)
        {
            // データがなかったらbreak
            string @tempString = PlayerPrefs.GetString(PlayerPrefsEnum.SavePosName.ToString() + i, "None");
            if (@tempString == "None") break;

            // Listに名前を格納
            posNames.Add(@tempString);

            // ListにPlayerの位置を格納
            Vector3 tempPlayerPos = Vector3.zero;
            tempPlayerPos.x = PlayerPrefs.GetFloat(PlayerPrefsEnum.SavePosX.ToString() + i);
            tempPlayerPos.y = PlayerPrefs.GetFloat(PlayerPrefsEnum.SavePosY.ToString() + i);
            tempPlayerPos.z = PlayerPrefs.GetFloat(PlayerPrefsEnum.SavePosZ.ToString() + i);
            posPlayerVectors.Add(tempPlayerPos);

            // ListにCameraの位置を格納
            Vector3 tempCameraPos = Vector3.zero;
            tempCameraPos.x = PlayerPrefs.GetFloat(PlayerPrefsEnum.SaveCameraPosX.ToString() + i);
            tempCameraPos.y = PlayerPrefs.GetFloat(PlayerPrefsEnum.SaveCameraPosY.ToString() + i);
            tempCameraPos.z = PlayerPrefs.GetFloat(PlayerPrefsEnum.SaveCameraPosZ.ToString() + i);
            posCameraVectors.Add(tempCameraPos);
        }  
    }

    /// <summary>
    /// 起動時のボタン生成
    /// </summary>
    private void SavePosButtonInstantiate()
    {
        for (int i = 0; i < posNames.Count; i++)
        {
            // 起動時にボタンが1個ある
            if (i != 0) posButtonsObj.Add(Instantiate(posButtonsObj[0], content.transform));

            // ボタンの関数設定
            Vector3 tempPlayerPos = posPlayerVectors[i];
            Vector3 tempCameraPos = posCameraVectors[i];
            posButtonsObj[i].GetComponent<Button>().onClick
                .AddListener(() => 
                {
                    SavePosButtonWarp(tempPlayerPos, tempCameraPos);
                });

            // Listに格納
            posButtons.Add(posButtonsObj[i].GetComponent<Button>());
            posButtonsText.Add(posButtonsObj[i].GetComponentInChildren<Text>());

            // ボタンのText変更
            posButtonsText[i].text = posNames[i];

            // Deleteボタンの関数設定
            string name = posNames[i];
            posButtonsObj[i].transform.GetChild(1).GetComponent<Button>().onClick
                .AddListener(() => SavePosDeleteButton(name));
        }

        // 何もセーブがなければボタン非表示
        posButtonsObj[0].SetActive(posNames.Count != 0);
    }

    /// <summary>
    /// 新たなセーブポイントの追加
    /// </summary>
    public void SavePosSaveButton()
    {
        // 名前の保存
        posNames.Add(inputField.text);
        inputField.text = "";
        PlayerPrefs.SetString(PlayerPrefsEnum.SavePosName.ToString() + (posNames.Count - 1), posNames[^1]);

        // Playerの位置の保存
        posPlayerVectors.Add(Player.instance.transform.position);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.SavePosX.ToString() + (posPlayerVectors.Count - 1), posPlayerVectors[^1].x);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.SavePosY.ToString() + (posPlayerVectors.Count - 1), posPlayerVectors[^1].y);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.SavePosZ.ToString() + (posPlayerVectors.Count - 1), posPlayerVectors[^1].z);

        // Cameraの位置の保存
        posCameraVectors.Add(playerFollowCamera.transform.position);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.SaveCameraPosX.ToString() + (posCameraVectors.Count - 1), posCameraVectors[^1].x);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.SaveCameraPosY.ToString() + (posCameraVectors.Count - 1), posCameraVectors[^1].y);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.SaveCameraPosZ.ToString() + (posCameraVectors.Count - 1), posCameraVectors[^1].z);


        PlayerPrefs.Save();

        // ボタンの生成(非表示切り替え)
        posButtonsObj[0].SetActive(true);
        if (posButtonsObj.Count != posNames.Count) posButtonsObj.Add(Instantiate(posButtonsObj[0], content.transform));

        // ボタンの関数設定
        Vector3 tempPlayerPos = posPlayerVectors[^1];
        Vector3 tempCameraPos = posPlayerVectors[^1];
        posButtonsObj[^1].GetComponent<Button>().onClick
            .AddListener(() => 
            {
                SavePosButtonWarp(tempPlayerPos, tempCameraPos);
            });

        // Listに格納
        posButtons.Add(posButtonsObj[^1].GetComponent<Button>());
        posButtonsText.Add(posButtonsObj[^1].GetComponentInChildren<Text>());

        // ボタンのText変更
        posButtonsText[^1].text = posNames[^1];

        // Deleteボタンの関数設定
        string name = posNames[^1];
        posButtonsObj[^1].transform.GetChild(1).GetComponent<Button>().onClick
                .AddListener(() => SavePosDeleteButton(name));
    }

    /// <summary>
    /// セーブを消す
    /// </summary>
    /// <param name="name"></param>
    public void SavePosDeleteButton(string name)
    {
        // データをリストから削除
        int index = posNames.IndexOf(name);

        posNames.RemoveAt(index);
        posPlayerVectors.RemoveAt(index);
        posCameraVectors.RemoveAt(index);


        // PlayerPrefsの末尾を削除
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.SavePosName.ToString() + (posNames.Count - 1));
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.SavePosX.ToString() + (posPlayerVectors.Count - 1));
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.SavePosY.ToString() + (posPlayerVectors.Count - 1));
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.SavePosZ.ToString() + (posPlayerVectors.Count - 1));
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.SaveCameraPosX.ToString() + (posCameraVectors.Count - 1));
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.SaveCameraPosY.ToString() + (posCameraVectors.Count - 1));
        PlayerPrefs.DeleteKey(PlayerPrefsEnum.SaveCameraPosZ.ToString() + (posCameraVectors.Count - 1));


        // PlayerPrefsの更新(上書き)
        for (int i = 0; i < posNames.Count; i++)
        {
            PlayerPrefs.SetString(PlayerPrefsEnum.SavePosName.ToString() + i, posNames[i]);
            PlayerPrefs.SetFloat(PlayerPrefsEnum.SavePosX.ToString() + i, posPlayerVectors[i].x);
            PlayerPrefs.SetFloat(PlayerPrefsEnum.SavePosY.ToString() + i, posPlayerVectors[i].y);
            PlayerPrefs.SetFloat(PlayerPrefsEnum.SavePosZ.ToString() + i, posPlayerVectors[i].z);
            PlayerPrefs.SetFloat(PlayerPrefsEnum.SaveCameraPosX.ToString() + i, posCameraVectors[i].x);
            PlayerPrefs.SetFloat(PlayerPrefsEnum.SaveCameraPosY.ToString() + i, posCameraVectors[i].y);
            PlayerPrefs.SetFloat(PlayerPrefsEnum.SaveCameraPosZ.ToString() + i, posCameraVectors[i].z);
        }


        // ボタンが0個にならないようにする
        if (posButtonsObj.Count == 1)
        {
            posButtonsObj[0].SetActive(false);
            return;
        }

        // ボタンの削除
        Destroy(posButtonsObj[index]);

        posButtonsObj.RemoveAt(index);
        posButtons.RemoveAt(index);
        posButtonsText.RemoveAt(index);
    }

    /// <summary>
    /// ボタンを押したらワープする
    /// </summary>
    /// <param name="playerPos"></param>
    /// <param name="cameraPos"></param>
    private void SavePosButtonWarp(Vector3 playerPos, Vector3 cameraPos)
    {
        //Player.instance.transform.position = playerPos;
        //playerFollowCamera.transform.position = cameraPos;
        this.playerPos = playerPos;
        this.cameraPos = cameraPos;
    }

    #endregion

#else

    private void Awake()
    {
        Destroy(this.gameObject);
    }

#endif


}
