#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.UI;

public class TimeScaleChange : DebugToolBase
{
    #region Field

    public enum PlayerPrefsEnum
    {
        TimeScale = 0,
    }

    private float timeScale;
    [SerializeField] private Slider timeScaleSlider;
    [SerializeField] private Text timeScaleText;
    [SerializeField] private Button timeScaleResetButton;
    private const int CONSTANT_VELOCITY = 10;
    private const string TIME_SCALE_TEXT_FORMAT = "x {0}�{";

    #endregion

    #region DebugToolBaseMethod

    public override void DebugToolAwake()
    {
        SetUiAction();
        DataLoad();
    }

    public override void DebugToolStart()
    {
        DataSet();
    }

    //public override void DebugToolOpen()
    //{
    //    Time.timeScale = 0f;
    //}

    public override void DebugToolClose()
    {
        DataSave();
        DataSet();
    }

    #endregion

    #region CustomMethod

    /// <summary>
    /// UI�̃{�^���������ꂽ�ۂ̏����̐ݒ�
    /// </summary>
    private void SetUiAction()
    {
        // �^�C���X�P�[���̃X���C�_�[���ύX���ꂽ�ۂ̏���
        // �l��ϊ����Ď󂯎��A�e�L�X�g��ς���
        timeScaleSlider.onValueChanged.AddListener(value => {
            timeScale = value / CONSTANT_VELOCITY;
            timeScaleText.text = LibFuncUtility.TextFormatBuilder(TIME_SCALE_TEXT_FORMAT, timeScale.ToString("F1"));
        });

        // �^�C���X�P�[���̃��Z�b�g�{�^���������ꂽ�ۂ̏���
        // �X���C�_�[��������Ԃɖ߂�
        timeScaleResetButton.onClick.AddListener(() => timeScaleSlider.value = CONSTANT_VELOCITY);
    }

    /// <summary>
    /// �f�[�^�̃��[�h
    /// </summary>
    private void DataLoad()
    {
        timeScale = PlayerPrefs.GetFloat(PlayerPrefsEnum.TimeScale.ToString(), 1f);
        timeScaleSlider.value = timeScale * CONSTANT_VELOCITY;
    }

    /// <summary>
    /// �f�[�^�̔��f
    /// </summary>
    private void DataSet()
    {
        Time.timeScale = timeScale;
    }

    /// <summary>
    /// �f�[�^�̃Z�[�u
    /// </summary>
    private void DataSave()
    {
        PlayerPrefs.SetFloat(PlayerPrefsEnum.TimeScale.ToString(), timeScale);
        PlayerPrefs.Save();
    }

    #endregion
}

#endif