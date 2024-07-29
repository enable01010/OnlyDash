using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

public class DebugEditor : EditorWindow
{

    #region UIBuilder

    static private VisualTreeAsset rootVisualTreeAsset;
    static private VisualTreeAsset logVisualTreeAsset;
    static private VisualTreeAsset infoVisualTreeAsset;
    static private StyleSheet rootStyleSheet;
    ScrollView view;

    static private VisualTreeAsset _rootVisualTreeAsset
    {
        get
        {
            if (rootVisualTreeAsset == null)
                rootVisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Lib/LibDebug/Debug.uxml");

            return rootVisualTreeAsset;
        }
    }

    static private VisualTreeAsset _logVisualTreeAsset
    {
        get
        {
            if(logVisualTreeAsset == null) 
                logVisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Lib/LibDebug/Log.uxml");

            return logVisualTreeAsset;
        }
    }

    static private VisualTreeAsset _infoVisualTreeAsset
    {
        get
        {
            if (infoVisualTreeAsset == null)
                infoVisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Lib/LibDebug/Info.uxml");

            return infoVisualTreeAsset;
        }
    }

    static private StyleSheet _rootStyleSheet
    {
        get
        {
            if (rootStyleSheet == null)
                rootStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Lib/LibDebug/Debug.uxml");

            return rootStyleSheet;
        }
    }

    #endregion

    private List<LogInfo> logList = new List<LogInfo>();
    private List<string> rayList;

    DebugUser user = DebugUser.All;
    DebugWindowMenu menu = DebugWindowMenu.Log;

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
    
    #endregion

    #region �����ݒ�

    private void SheetSetting()
    {
        _rootVisualTreeAsset.CloneTree(rootVisualElement);
        rootVisualElement.styleSheets.Add(_rootStyleSheet);

        view = rootVisualElement.Q<ScrollView>("LogArea");
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
        ViewReset();
        switch(menu)
        {
            case DebugWindowMenu.Log:
                logList.Clear();
                break;
        }
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
        ViewReset();
        user = DebugUser.All;
        LogVeiw();
    }

    private void TaniButtonOnClick()
    {
        UnityEngine.Debug.Log("�J�{�^���������ꂽ��");
        ViewReset();
        user = DebugUser.Taniyama;
        LogVeiw();
    }

    private void MathuButtonOnClick()
    {
        UnityEngine.Debug.Log("���{�^���������ꂽ��");
        ViewReset();
        user = DebugUser.Matuoka;
        LogVeiw();
    }

    #endregion

    #region Lib�f�o�b�N����Ăяo���p

    public void Log(object obj,DebugUser user)
    {
#if UNITY_EDITOR
        const int LAPPER_COUNT = 2;//�Ăяo���֐���LibDebug.Log������

        //�Ăяo�������̒���
        StackFrame caller = new StackFrame(LAPPER_COUNT, true);
        string filePath = caller.GetFileName();
        int line = caller.GetFileLineNumber();

        LogListUpdate(obj, filePath, line, user);
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

        //���X�g�̍X�V
        bool isSame = CheckIsSameLogInfo(obj, filePath, line, user, out LogInfo info);
        info.Add(obj);
        if (isSame == false) logList.Add(info);

        //�����ڂ̍X�V
        if(menu == DebugWindowMenu.Log)
        {   
            if(isSame == false && info.CheckOutputUser(user) == true) AddEditor(info);
            if (info.CheckOutputUser(user) == true) info.RefreshEditor(obj);
        }
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
        VisualElement logElement = DebugEditor._logVisualTreeAsset.CloneTree();
        view.Add(logElement);
        info.AddElement(logElement);
    }

    public class LogInfo
    {
        string filePath;
        int line;
        object lastObj;
        DebugUser user;
        List<OutputData> outputs;
        int count = 0;
        VisualElement element;
        bool isOpen = false;
        ScrollView items;

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
            count++;
            bool isSame = CheckIsSameOutputData(output,out OutputData data);

            data.AddCount();

            if(isSame == false)
            {
                outputs.Add(data);
            }
            
            if(isOpen == true)
            {
                VisualElement infoElement = DebugEditor._infoVisualTreeAsset.CloneTree();
                items.Add(infoElement);

                Label label = infoElement.Q<Label>("Label");
                label.text = data.output.ToString();
            }
        }

        public void AddElement(VisualElement element)
        {
            this.element = element;
            Button old = element.Q<Button>("old");
            old.clicked += ChangeViewOld;
            Button script = element.Q<Button>("script");
            script.clicked += OpenScript;
            items = element.Q<ScrollView>("newItems");
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
            lastObj = obj;
            Label output = element.Q<Label>("Message");
            output.text = obj.ToString();
            Label count = element.Q<Label>("Count");
            count.text = this.count.ToString();
        }

        public void RefreshEditor()
        {
            Label output = element.Q<Label>("Message");
            output.text = lastObj.ToString();
            Label count = element.Q<Label>("Count");
            count.text = this.count.ToString();
        }

        private void ChangeViewOld()
        {
            if(isOpen == false)
            {
                //OpenEditor();

                foreach(OutputData data in outputs)
                {
                    VisualElement infoElement = DebugEditor._infoVisualTreeAsset.CloneTree();
                    items.Add(infoElement);

                    Label label = infoElement.Q<Label>("Label");
                    label.text = data.output.ToString();
                }
            }
            else
            {
                //CloseEditor();
                items.Clear();
            }

            isOpen =! isOpen;
        }

        private void OpenScript()
        {
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(filePath,line);
        }

        public bool CheckOutputUser(DebugUser user)
        {
            if (user == DebugUser.All) return true;
            return this.user == user;
        }

        public class OutputData
        {
            int count = 0;
            int timing;
            public object output { get; private set; }

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

    #region �S�̓I�Ɏg���֐�

    private void ViewReset()
    {
        view.Clear();
    }

    private void LogVeiw()
    {
        foreach(LogInfo log in logList)
        {
            if (log.CheckOutputUser(user) == false) continue;
            AddEditor(log);
            log.RefreshEditor();
        }
    }

    #endregion
}

public enum DebugWindowMenu
{
    Log,
    Ray,
    Method,
}