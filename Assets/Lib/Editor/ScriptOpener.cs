using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;

public class ScriptOpener : AssetPostprocessor
{
    static void OnPostprocessAllAssets(
         string[] importedAssets,
         string[] deletedAssets,
         string[] movedAssets,
         string[] movedFromAssetPaths)
    {
        foreach (string asset in importedAssets)
        {
            // C#�X�N���v�g���쐬���ꂽ�ꍇ
            if (asset.EndsWith(".cs"))
            {
                OpenScriptInVisualStudio(asset);
            }
        }
    }

    // Visual Studio�ŃX�N���v�g���J������
    private static void OpenScriptInVisualStudio(string scriptPath)
    {
        //����N���̂ݎ��{
        if (EditorPrefs.GetBool(scriptPath, false)) return;
        EditorPrefs.SetBool(scriptPath, true);

        // Unity�̃v���W�F�N�g�t�H���_����\�����[�V�����t�@�C���̃p�X�𐶐�
        string projectPath = Application.dataPath;
        string projectDirectoryName = new DirectoryInfo(projectPath).Parent.Name;
        string solutionPath = projectDirectoryName + ".sln";


        // Unity�̐ݒ肩��Visual Studio�̃p�X���擾
        string externalScriptEditor = EditorPrefs.GetString("kScriptsDefaultApp");

        if (!string.IsNullOrEmpty(externalScriptEditor) && File.Exists(solutionPath))
        {
            // �����Ƃ��ăX�N���v�g�t�@�C���݂̂��w��
            string arguments = IsVisualStudioRunning() ? $"/Edit \"{scriptPath}\"" : $"\"{solutionPath}\" \"{scriptPath}\"";

            // ProcessStartInfo���g����Visual Studio���N��
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = externalScriptEditor,
                Arguments = arguments,
                UseShellExecute = true,
                CreateNoWindow = false
            };

            // Visual Studio���N��
            Process.Start(startInfo);
        }
    }

    private static bool IsVisualStudioRunning()
    {
        // Visual Studio�̃v���Z�X�����擾
        string[] visualStudioProcessNames = { "devenv", "vs" ,"sln", "VisualStudio" };

        // ���݂̃v���Z�X���擾
        Process[] processes = Process.GetProcesses();

        // �e�v���Z�X���m�F
        int i = 0;
        while(i < processes.Length)
        {
            try
            {
                Process process = processes[i];
                foreach (string processName in visualStudioProcessNames)
                {
                    if (process.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            catch(Exception e)
            {
                
            }
            i++;
        }

        return false;
    }
}
