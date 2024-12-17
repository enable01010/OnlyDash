using System;
using System.Diagnostics;
using System.IO;
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
        if (importedAssets.Length == 1 && importedAssets[0].EndsWith(".cs"))
        {
            OpenScriptInVisualStudio(importedAssets[0]);
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
