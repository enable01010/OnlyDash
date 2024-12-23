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
        // �L�����N�^�[�̈ړ����o�O�邽�ߎb��Ή��ňړ��p�̃R���|�[�l���g�𖳌���
        Player.instance.GetController().enabled = false;
    }

    public override void DebugToolClose()
    {
        // �L�����N�^�[�̈ړ����o�O�邽�ߎb��Ή��ňړ��p�̃R���|�[�l���g��L����
        Player.instance.GetController().enabled = true;
    }

    #endregion


    #region CustomMethod

    /// <summary>
    /// UI�̃{�^���������ꂽ�ۂ̏����̐ݒ�
    /// </summary>
    private void SetUiAction()
    {
        // �|�W�V�����Z�[�u�@�\�̐V�K�o�^�p�{�^��
        savePosButton.onClick.AddListener(SaveButton);
    }

    /// <summary>
    /// �f�[�^�̃��[�h(�{�^���̐���)
    /// </summary>
    private void DataLoad()
    {
        baseButton = content.GetChild(0).gameObject;

        int index = 0;
        while (SavePosDataManager.TryLoad(index, out var data))
        {
            // �{�^���̐���
            InstantiateButton(data);

            // ���X�g�ɓo�^
            savePosData.Add(data);

            index++;
        }
    }

    /// <summary>
    /// �V���ȃZ�[�u�|�C���g�̒ǉ�(�{�^���ɐݒ肷��֐�)
    /// </summary>
    public void SaveButton()
    {
        int index = savePosData.Count;

        // �Z�[�u����f�[�^�̍쐬
        SavePosData data = new()
        {
            saveName = inputField.text,
            playerPos = Player.instance.transform.position,
            cameraEulerAngle = Player.instance.DebugCameraAngleGet()
        };

        // �Z�[�u���{
        data.Save(index);

        // �{�^���̐���
        InstantiateButton(data);

        //���X�g�ɓo�^
        savePosData.Add(data);

        // ���͗�����ɂ���
        inputField.text = "";
    }


    /// <summary>
    /// �{�^���̐���
    /// </summary>
    /// <param name="data"> �Z�[�u����Ă���f�[�^ </param>
    private void InstantiateButton(SavePosData data)
    {
        // �{�^������
        GameObject @obj = Instantiate(baseButton, content);
        @obj.SetActive(true);

        // �{�^���̊֐��ݒ�
        Button @button = @obj.GetComponent<Button>();
        @button.onClick.AddListener(() => PlayerWarp(data.playerPos, data.cameraEulerAngle));

        // �{�^����Text�ύX
        Text @text = @obj.GetComponentInChildren<Text>();
        @text.text = data.saveName;

        // Delete�{�^���̊֐��ݒ�
        obj.transform.GetChild(1).GetComponent<Button>().onClick
            .AddListener(() => DeleteButton(data.saveName));

        // List�ɒǉ�
        data.buttonObj = @obj;
        data.button = @button;
        data.buttonText = @text;
    }

    /// <summary>
    /// �Z�[�u������(�{�^���ɐݒ肷��֐�)
    /// </summary>
    /// <param name="name"></param>
    public void DeleteButton(string name)
    {
        // �f�[�^�����X�g����폜
        int index = 0;
        for (; index < savePosData.Count; index++)
        {
            if (savePosData[index].saveName.Equals(name))
            {
                break;
            }
        }
        Destroy(savePosData[index].buttonObj);// �{�^���̍폜
        savePosData.RemoveAt(index);

        // PlayerPrefs�̖������폜
        SavePosDataManager.Delete(savePosData.Count);

        // PlayerPrefs�̍X�V(�㏑��)
        for (int i = 0; i < savePosData.Count; i++)
        {
            savePosData[i].Save(i);
        }
    }

    /// <summary>
    /// Player�����[�v������
    /// </summary>
    /// <param name="playerPos"></param>
    /// <param name="cameraAngle"></param>
    private void PlayerWarp(Vector3 playerPos, Vector3 cameraAngle)
    {
        Player.instance.transform.position = playerPos;
        Player.instance.DebugCameraAngleSet(cameraAngle);
        // TODO:�v���C���[�̉�]������ꍇ����
    }

    #endregion
}

/// <summary>
/// �f�o�b�O�p�̃v���C���[�ʒu���f�[�^����������p�̃N���X
/// </summary>
public static class SavePosDataManager
{

    /// <summary>
    /// �w�肳�ꂽ�C���f�b�N�X�Ƀf�[�^������ꍇ�擾
    /// </summary>
    /// <param name="index">�C���f�b�N�X</param>
    /// <param name="answer">�擾�����f�[�^</param>
    /// <returns>����������</returns>
    public static bool TryLoad(int index, out SavePosData answer)
    {
        // return�o���Ȃ��̂ňꎞ�I�ɓ��ꍞ��
        answer = null;

        // ���O���擾
        string name = PlayerPrefs.GetString(PlayerPrefsEnum.PosName.ToString() + index);

        // �ۑ��f�[�^��������Εԋp
        if (string.IsNullOrEmpty(name)) return false;


        answer = new SavePosData();

        // ���O�̓o�^
        answer.saveName = name;

        // Player�̈ʒu���擾
        Vector3 @Vector = Vector3.zero;
        @Vector.x = PlayerPrefs.GetFloat(PlayerPrefsEnum.PlayerPosX.ToString() + index);
        @Vector.y = PlayerPrefs.GetFloat(PlayerPrefsEnum.PlayerPosY.ToString() + index);
        @Vector.z = PlayerPrefs.GetFloat(PlayerPrefsEnum.PlayerPosZ.ToString() + index);
        answer.playerPos = @Vector;

        // Camera�̊p�x���擾
        @Vector.x = PlayerPrefs.GetFloat(PlayerPrefsEnum.CameraEulerAngleX.ToString() + index);
        @Vector.y = PlayerPrefs.GetFloat(PlayerPrefsEnum.CameraEulerAngleY.ToString() + index);
        @Vector.z = 0;
        answer.cameraEulerAngle = @Vector;

        return true;
    }

    /// <summary>
    /// �N���X���̏��Ńf�[�^��ۑ�
    /// </summary>
    /// <param name="data">�ۑ�����f�[�^</param>
    /// <param name="index">�C���f�b�N�X</param>
    public static void Save(this SavePosData data, int index)
    {
        PlayerPrefs.SetString(PlayerPrefsEnum.PosName.ToString() + index, data.saveName);

        // Player�̈ʒu�̕ۑ�
        PlayerPrefs.SetFloat(PlayerPrefsEnum.PlayerPosX.ToString() + index, data.playerPos.x);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.PlayerPosY.ToString() + index, data.playerPos.y);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.PlayerPosZ.ToString() + index, data.playerPos.z);

        // Camera�̈ʒu�̕ۑ�
        PlayerPrefs.SetFloat(PlayerPrefsEnum.CameraEulerAngleX.ToString() + index, data.cameraEulerAngle.x);
        PlayerPrefs.SetFloat(PlayerPrefsEnum.CameraEulerAngleY.ToString() + index, data.cameraEulerAngle.y);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// �w�肵���C���f�b�N�X�̃f�[�^���폜����֐�
    /// </summary>
    /// <param name="index">�C���f�b�N�X</param>
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