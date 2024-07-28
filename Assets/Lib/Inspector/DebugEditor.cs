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

    #region 描画更新

    // ヘッダメニュー名/ヘッダ以下のメニュー名
    [MenuItem("カスタムエディター/Debug", false, 5)]
    private static void ShowWindow()
    {
        var window = GetWindow<DebugEditor>("UIElements");
        window.titleContent = new GUIContent("Debug"); // エディタ拡張ウィンドウのタイトル
        window.Show();
    }

    private void CreateGUI()
    {
        //シートの登録
        SheetSetting();

        //ボタンの登録
        ButtonSetting();

        //リストの設定
        ListSetting();
    }

    private void RefreshWindow()
    {

    }

    #endregion

    #region 初期設定

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

    #region ボタンが押された際の処理

    private void ClearButtonOnClick()
    {
        UnityEngine.Debug.Log("クリアボタンが押されたよ");
    }


    private void LogButtonOnClick()
    {
        UnityEngine.Debug.Log("ログボタンが押されたよ");
    }

    private void RayButtonOnClick()
    {
        UnityEngine.Debug.Log("レイボタンが押されたよ");
    }

    private void MethodButtonOnClick()
    {
        UnityEngine.Debug.Log("メソッドボタンが押されたよ");
    }

    private void AllButtonClick()
    {
        UnityEngine.Debug.Log("オールボタンが押されたよ");
    }

    private void TaniButtonOnClick()
    {
        UnityEngine.Debug.Log("谷ボタンが押されたよ");
    }

    private void MathuButtonOnClick()
    {
        UnityEngine.Debug.Log("松ボタンが押されたよ");
    }

    #endregion

    #region Libデバックから呼び出す用

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

    #region Log関係のprivate

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
