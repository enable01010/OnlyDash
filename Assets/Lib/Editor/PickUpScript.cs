using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class PickUpScript : EditorWindow
{
    private List<MonoScript> script;
    private Vector2 scrollPos = Vector2.zero;

    [MenuItem("�J�X�^���G�f�B�^�[/Pick Up Script",false,3)]
    static void ScriptSwitcherOpen()
    {
        //�I�����ꂽ��E�C���h�E���J��
        GetWindow<PickUpScript>("PickUpScript");
    }

    private void OnEnable()
    {
        if (script == null)
        {
            script = new List<MonoScript>();
            Load();
        }
    }

    private static string FilePath => $"{Application.persistentDataPath}/_PickUpScriptLauncher.sav";

    private void OnGUI()
    {
        //��s�J�n
        EditorGUILayout.BeginHorizontal();

        //�X�N���v�g�p�̌������̐ݒu
        var sceneAsset = EditorGUILayout.ObjectField(null, typeof(MonoScript), false) as MonoScript;
        if (sceneAsset != null && script.IndexOf(sceneAsset) < 0)
        {
            script.Add(sceneAsset);
            Save();
        }

        EditorGUILayout.EndHorizontal();
        //��s�I��

        GuiLine();

        //���X�g�̒��g���\��
        this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos);
        for (var i = 0; i < script.Count; ++i)
        {
            var scene = script[i];
            EditorGUILayout.BeginHorizontal();
            var path = AssetDatabase.GetAssetPath(scene);

            //�폜�̃{�^��
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                script.Remove(scene);
                Save();
                --i;
            }
            else
            {
                if (GUILayout.Button("O", GUILayout.Width(20)))
                {
                    //���̃V�[���̈ʒu��Project��ŕ\��������
                    EditorGUIUtility.PingObject(scene);
                }

                if (GUILayout.Button(i > 0 ? "��" : "�@", GUILayout.Width(20)) && i > 0)
                {
                    //�V�[���̏ꏊ�����ւ���
                    script[i] = script[i - 1];
                    script[i - 1] = scene;
                    Save();
                }

                EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
                if (GUILayout.Button(Path.GetFileNameWithoutExtension(path)))
                {
                    //�X�N���v�g���J��
                    System.Diagnostics.Process.Start(Path.Combine(Directory.GetParent(Application.dataPath).FullName, path));
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    private void Save()
    {
        var guids = new List<string>();
        foreach (var scene in script)
        {
            string guid;
            long localId;
            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(scene, out guid, out localId))
            {
                guids.Add(guid);
            }
        }

        var content = string.Join("\n", guids.ToArray());
        File.WriteAllText(FilePath, content);
    }

    private void Load()
    {
        script.Clear();
        if (File.Exists(FilePath))
        {
            string content = File.ReadAllText(FilePath);
            foreach (var guid in content.Split(new char[] { '\n' }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var scene = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                if (scene != null)
                    script.Add(scene);
            }
        }
    }

    private void GuiLine(int height = 1)
    {
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(height) });
    }
}