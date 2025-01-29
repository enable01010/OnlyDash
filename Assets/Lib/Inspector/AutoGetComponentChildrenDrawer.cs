#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// AutoGetComponentChildAttribute�����Ă�ϐ����C���X�y�N�^�[�ɕ\������p�̃N���X
/// PropertyDrawer���p������Ƃ��̕ӂ��������
/// </summary>
[CustomPropertyDrawer(typeof(AutoGetComponentChildAttribute))]
public class AutoGetComponentChildDrawer : PropertyDrawer
{
    /// <summary>
    /// �C���X�y�N�^�[�̕`�揈��
    /// Update�݂����ɖ��t���[���ʂ�悤�Ȋ֐��̂��ߒ���
    /// �����Ƒ΍􂵂Ȃ��Ɩ��t���[��GetComponent�݂����Ȏ��ɂȂ�
    /// </summary>
    /// <param name="position">�`��ʒu</param>
    /// <param name="property">�Ώۂ̃I�u�W�F�N�g</param>
    /// <param name="label">�m���</param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // �V���A���C�Y�ɃR���|�[�l���g��ݒ�
        GetOrComponentChild(property);

        // �W���̃v���p�e�B�t�B�[���h��`��
        // �C���X�y�N�^�[�ɕ\�����Ȃ��ꍇ�͂����̂R�s������
        EditorGUI.BeginDisabledGroup(true);// ���͏o���Ȃ����̕\���J�n
        EditorGUI.PropertyField(position, property, label);�@// �Ώۂ̕ϐ���\��
        EditorGUI.EndDisabledGroup();// ���͂Ȃ����̕\���I��
    }

    /// <summary>
    /// �V���A���C�Y�ɑ΂��āA�Ώۂ̃N���X����ꍞ��
    /// </summary>
    /// <param name="property">�Ώۂ̃I�u�W�F�N�g</param>
    private void GetOrComponentChild(SerializedProperty property)
    {
        // AutoGetComponentChild�ɃV���A���C�Y�t�B�[���h�����邩�m�F
        if (property.propertyType != SerializedPropertyType.ObjectReference) return;
        // �V���A���C�Y�����ݒ�̏ꍇ�ɂ̂ݎ��s
        if (property.objectReferenceValue != null) return;
        // �Ώۂ�GameObject�̏ꍇ�ɂ̂ݎ��s
        MonoBehaviour targetObject = property.serializedObject.targetObject as MonoBehaviour;
        if (targetObject == null) return;

        // �ϐ����擾
        string name = property.name;
        // �擪�̋L������菜��(@�͔F������Ă��Ȃ�)
        if (name[0] == '_') name = name.Remove(0, 1);
        // �擪�̕�����啶���ɕς���
        char firstStr = char.ToUpper(name[0]);
        name = name.Remove(0, 1);
        name = firstStr + name;

        // �q�I�u�W�F�N�g��S�Ď擾
        List<Transform> children = LibTransform.GetAllChildren(targetObject.transform);
        GameObject nameObj = null;
        // ���O��v�I�u�W�F�N�g��T��
        foreach (Transform child in children)
        {
            if (child.name.CompareTo(name) == 0)
            {
                // ���O��v�I�u�W�F�N�g����
                nameObj = child.gameObject;
                break;
            }
        }
        // ���O��v�I�u�W�F�N�g���Ȃ��Ȃ琶��
        if (nameObj == null)
        {
            nameObj = new(name);
            nameObj.transform.parent = targetObject.transform;
            nameObj.transform.localPosition = Vector3.zero;
            nameObj.transform.localEulerAngles = Vector3.zero;
            nameObj.transform.localScale = Vector3.one;
        }

        // ���łɂ��Ă���R���|�[�l���g���擾
        var component = nameObj.GetComponent(fieldInfo.FieldType);
        if (component == null)
        {
            // ���Ă��Ȃ������ꍇ�R���|�[�l���g��ǉ�
            component = nameObj.gameObject.AddComponent(fieldInfo.FieldType);
            if (component == null)
            {
                Debug.LogError(nameObj.name + "��AutoGetComponentChild�Ɏ��s���܂����B\nMonoBehaviour�ȊO�̃N���X�̃A�g���r���[�g���ݒ肳��Ă��܂��B\n���G�f�B�^���d���Ȃ�̂ł��̃G���[�͕K���������Ă�������");
                return;
            }
        }

        // �V���A���C�Y�̏����X�V
        if (component != null)
        {
            property.objectReferenceValue = component;// �ϐ��ɑ��
            property.serializedObject.ApplyModifiedProperties(); // �v���p�e�B���X�V
        }
    }
}
#endif


/// <summary>
/// �A�g���r���[�g���쐬����p�̃N���X
/// PropertyAttribute���p������ƃA�g���r���[�g�ɐݒ�ł���
/// AttributeTargets.Field�͕ϐ��ɕt�������ĈӖ��AMethod�Ƃ�Class�Ƃ�����͂��A�m��񂯂�
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class AutoGetComponentChildAttribute : PropertyAttribute
{
}