using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections;

public class MyWindow : EditorWindow
{
    //�����ݒ�p�̃t�H���_�[�𐶐�����
    [MenuItem("�J�X�^���G�f�B�^�[/CreateFolders",priority = 1)]
    public static void CreateFolder()
    {
        Debug.Log("CreateFolders�̏��������s���܂�");

        //�쐬����t�H���_�[�̃p�X���m�F
        Debug.Log(Application.dataPath);
        string folderPath = Application.dataPath;

        //�쐬����t�H���_�[�̃p�X�𐶐�
        folderPath = Path.Combine(folderPath, @"MyProgram");

        //�t�H���_�[�����łɐ�������Ă��邩�m�F
        bool IsCreated = Directory.Exists(folderPath);
        if(IsCreated == true)
        {
            Debug.Log(folderPath + "�͂��łɍ쐬����Ă��܂�");
        }
        else
        {
            Debug.Log("�����ݒ�̃t�H���_�[�𐶐����܂�");
            Directory.CreateDirectory(folderPath);
            Directory.CreateDirectory(Path.Combine(folderPath, @"Prefab"));
            Directory.CreateDirectory(Path.Combine(folderPath, @"Script"));
            Directory.CreateDirectory(Path.Combine(folderPath, @"Image"));
            Directory.CreateDirectory(Path.Combine(folderPath, @"Music"));
            Directory.CreateDirectory(Path.Combine(folderPath, @"Animation"));
            Directory.CreateDirectory(Path.Combine(folderPath, @"Material"));
            Directory.CreateDirectory(Path.Combine(folderPath, @"TileMap"));

            if(Directory.Exists(folderPath))
            {
                Debug.Log("�t�H���_�[�̐����ɐ������܂���");
            }

            AssetDatabase.Refresh();
        }
    }

    private void OnGUI()
    {
        
    }

}
