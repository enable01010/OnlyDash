using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DebugEditor : EditorWindow
{
    [SerializeField] private VisualTreeAsset _rootVisualTreeAsset;
    [SerializeField] private StyleSheet _rootStyleSheet;
    [SerializeField] private VisualTreeAsset _log;
    [SerializeField] private VisualTreeAsset _info;
    private List<LogInfo> logList;
    private List<string> rayList;

    #region �`��X�V

    // �w�b�_���j���[��/�w�b�_�ȉ��̃��j���[��
    [MenuItem("�J�X�^���G�f�B�^�[/Debug", false, 5)]
    private static void ShowWindow()
    {
        var window = GetWindow<DebugEditor>("UIElements");
        window.titleContent = new GUIContent("Debug"); // �G�f�B�^�g���E�B���h�E�̃^�C�g��
        window.Show();
    }

    private void CreateGUI()
    {
        //�V�[�g�̓o�^
        SheetSetting();

        //�{�^���̓o�^
        ButtonSetting();

        //���X�g�̐ݒ�
        ListSetting();
    }

    private void RefreshWindow()
    {

    }

    #endregion

    #region �����ݒ�

    private void SheetSetting()
    {
        _rootVisualTreeAsset.CloneTree(rootVisualElement);
        rootVisualElement.styleSheets.Add(_rootStyleSheet);
    }

    private void ButtonSetting()
    {
        Button clearButton = rootVisualElement.Q<Button>("Clear");
        clearButton.clicked += ClearButtonOnClick;

        Button logButton = rootVisualElement.Q<Button>("Log");
        logButton.clicked += LogButtonOnClick;

        Button rayButton = rootVisualElement.Q<Button>("Ray");
        rayButton.clicked += RayButtonOnClick;

        Button methodButton = rootVisualElement.Q<Button>("Method");
        methodButton.clicked += MethodButtonOnClick;

        Button allButton = rootVisualElement.Q<Button>("All");
        allButton.clicked += AllButtonClick;

        Button taniButton = rootVisualElement.Q<Button>("Tani");
        taniButton.clicked += TaniButtonOnClick;

        Button mathuButton = rootVisualElement.Q<Button>("Mathu");
        mathuButton.clicked += MathuButtonOnClick;
    }

    private void ListSetting()
    {
        logList = new List<LogInfo>();
    }

    #endregion

    #region �{�^���������ꂽ�ۂ̏���

    private void ClearButtonOnClick()
    {
        UnityEngine.Debug.Log("�N���A�{�^���������ꂽ��");
    }


    private void LogButtonOnClick()
    {
        UnityEngine.Debug.Log("���O�{�^���������ꂽ��");
    }

    private void RayButtonOnClick()
    {
        UnityEngine.Debug.Log("���C�{�^���������ꂽ��");
    }

    private void MethodButtonOnClick()
    {
        UnityEngine.Debug.Log("���\�b�h�{�^���������ꂽ��");
    }

    private void AllButtonClick()
    {
        UnityEngine.Debug.Log("�I�[���{�^���������ꂽ��");
    }

    private void TaniButtonOnClick()
    {
        UnityEngine.Debug.Log("�J�{�^���������ꂽ��");
    }

    private void MathuButtonOnClick()
    {
        UnityEngine.Debug.Log("���{�^���������ꂽ��");
    }

    #endregion

    #region Lib�f�o�b�N����Ăяo���p

    public void Log(object obj,DebugUser user, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0)
    {
#if UNITY_EDITOR
        LogListUpdate(obj, filePath, line, user);
        RefreshWindow();
#endif
    }

    public void Ray()
    {

    }

    #endregion

    #region Log�֌W��private

    private void LogListUpdate(object obj,string filePath,int line,DebugUser user)
    {
        if (logList == null) return;

        bool isSame = CheckIsSameLogInfo(obj, filePath, line, user, out LogInfo info);
        info.Add(obj);
        
        if (isSame == false)
        {
            logList.Add(info);
            AddEditor(info);
        }

        info.RefreshEditor(obj);
    }

    private bool CheckIsSameLogInfo(object output,string filePath,int line,DebugUser user ,out LogInfo info)
    {
        foreach (LogInfo log in logList)
        {
            if (log.GetIsSameGroup(filePath, line) == true)
            {
                info = log;
                return true;
            }
        }

        info = new LogInfo(filePath, line, user);
        return false;
    }

    private void AddEditor(LogInfo info)
    {
        ScrollView view = rootVisualElement.Q<ScrollView>("LogArea");
        VisualElement logElement = _log.CloneTree();
        view.Add(logElement);
        info.AddElement(logElement);
    }

    public class LogInfo
    {
        string filePath;
        int line;
        DebugUser user;
        List<OutputData> outputs;
        int count = 0;
        VisualElement element;

        private LogInfo() { }

        public LogInfo(string filePath,int line,DebugUser user)
        {
            this.filePath = filePath;
            this.line = line;
            this.user = user;
            outputs = new List<OutputData>();
        }


        public bool GetIsSameGroup(string filePath, int line)
        {
            bool answer = false;
            bool isSameFilePath = this.filePath.Equals(filePath);
            bool isSameLine = this.line == line;
            answer = (isSameFilePath && isSameLine);
            return answer;
        }

        public void Add(object output)
        {
            bool isSame = CheckIsSameOutputData(output,out OutputData data);

            data.AddCount();

            if(isSame == false)
            {
                outputs.Add(data);
            }
        }

        public void AddElement(VisualElement element)
        {
            this.element = element;
            Button old = element.Q<Button>("old");
            old.clicked += ChangeViewOld;
            Button script = element.Q<Button>("script");
            script.clicked += OpenScript;
        }

        private bool CheckIsSameOutputData(object output,out OutputData outputData)
        {
            foreach(OutputData data in outputs)
            {
                if(data.GetIsSmaeOutput(output) == true)
                {
                    outputData = data;
                    return true;
                }
            }

            outputData = new OutputData(count, output);
            return false;
        }

        public void RefreshEditor(object obj)
        {
            Label output = element.Q<Label>("Message");
            output.text = obj.ToString();
            Label count = element.Q<Label>("Count");
            count.text = this.count.ToString();
        }

        private void ChangeViewOld()
        {

        }

        private void OpenScript()
        {
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(filePath,line);
        }

        public class OutputData
        {
            int count = 0;
            int timing;
            object output;

            public OutputData(int timing, object output)
            {
                this.timing = timing;
                this.output = output;
            }

            public bool GetIsSmaeOutput(object output)
            {
                return this.output == output;
            }

            public void AddCount()
            {
                count++;
            }
        }
    }

    #endregion
}
