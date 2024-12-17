#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;


/// <summary>
/// AutoGetComponentAttribute�����Ă�ϐ����C���X�y�N�^�[�ɕ\������p�̃N���X
/// PropertyDrawer���p������Ƃ��̕ӂ��������
/// </summary>
[CustomPropertyDrawer(typeof(AutoGetComponentAttribute))]
public class AutoGetComponentDrawer : PropertyDrawer
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
        GetOrAddComponent(property);

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
    private void GetOrAddComponent(SerializedProperty property)
    {
        // AutoGetCompornent�ɃV���A���C�Y�t�B�[���h�����邩�m�F
        if (property.propertyType != SerializedPropertyType.ObjectReference) return;
        // �V���A���C�Y�����ݒ�̏ꍇ�ɂ̂ݎ��s
        if (property.objectReferenceValue != null) return;
        // �Ώۂ�GameObject�̏ꍇ�ɂ̂ݎ��s
        MonoBehaviour targetObject = property.serializedObject.targetObject as MonoBehaviour;
        if (targetObject == null) return;


        // ���łɂ��Ă���R���|�[�l���g���擾
        var component = targetObject.GetComponent(fieldInfo.FieldType);
        if (component == null)
        {
            // ���Ă��Ȃ������ꍇ�R���|�[�l���g��ǉ�
            component = targetObject.gameObject.AddComponent(fieldInfo.FieldType);
            if (component == null)
            {
                Debug.LogError(targetObject.name + "��AutoGetComponent�Ɏ��s���܂����B\nMonoBehaviour�ȊO�̃N���X�̃A�g���r���[�g���ݒ肳��Ă��܂��B\n���G�f�B�^���d���Ȃ�̂ł��̃G���[�͕K���������Ă�������");
                return;
            }
        }

        // �V���A���C�Y�̏����X�V
        if (component != null)
        {
            property.objectReferenceValue = component;
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
public class AutoGetComponentAttribute : PropertyAttribute
{
}