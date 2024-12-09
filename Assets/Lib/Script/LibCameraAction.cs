using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// �J��������p�̔ėp�N���X
/// </summary>
public static class LibCameraAction
{
    private static IEnumerator hitStopCoroutine = null;
    private static Vector3 shakeVector;

    /// <summary>
    /// �J�����̃q�b�g�X�g�b�v�����i��ʗh�炷�j
    /// </summary>
    /// <param name="shakeData">�h�炷���߂ɕK�v�ȃf�[�^(���ʂȏ󋵂�����null�̗\��j</param>
    public static void Shake(CameraShake shakeData = null)
    {
        // ���łɃq�b�g�X�g�b�v�̃J�������o���Đ�����Ă���ꍇ�͑O�̏����̏I��������������A�Ďn��������
        if(hitStopCoroutine != null)
        {
            // �I������
            LibCoroutineRunner.StopCoroutine(hitStopCoroutine);
            hitStopCoroutine = null;
        }

        // �R���[�`���f�[�^��ێ�����
        hitStopCoroutine = ShakeCoroutine(shakeData);

        // �J���������̃R���[�`���n��
        LibCoroutineRunner.StartCoroutine(hitStopCoroutine);
    }

    private static IEnumerator ShakeCoroutine(CameraShake shakeData)
    {
        // �V�F�C�N�f�[�^��null�����e���邽�߂Ȃ��ꍇ�͌Œ�l�ŏ�����
        if(shakeData == null)
        {
            shakeData = new CameraShake
            {
                delay = 0f,
                duration = 0.3f,
                shakeStrength = new Vector3(0.1f, 0.1f, 0.1f),
                shakeCurve = AnimationCurve.Linear(0, 1, 1, 0)
            };
        }

        // URP�̃J�����ݒ�
        RenderPipelineManager.beginCameraRendering += MoveCamera;

        // �J�����̈ړ������̂ݎZ�o
        float time = shakeData.duration;
        while (time > 0)
        {
            yield return null;

            time -= Time.deltaTime;
            float delta = Mathf.Clamp01(LibMath.OneMinus(time / shakeData.duration));

            var randomVec = new Vector3(Random.value, Random.value, Random.value);
            var shakeVec = Vector3.Scale(randomVec, shakeData.shakeStrength) * (Random.value > 0.5f ? -1 : 1);
            shakeVector = shakeVec * shakeData.shakeCurve.Evaluate(delta);
        }

        // URP�̃J�����ݒ�폜
        RenderPipelineManager.beginCameraRendering -= MoveCamera;
    }

    private static void MoveCamera(ScriptableRenderContext context, Camera cam)
    {
        cam.transform.localPosition += cam.transform.rotation * shakeVector;
    }
}

[System.Serializable]
public class CameraShake
{
    public float delay = 0.0f;
    public float duration = 1.0f;
    public Vector3 shakeStrength = new Vector3(0.1f, 0.1f, 0.1f);
    public AnimationCurve shakeCurve = AnimationCurve.Linear(0, 1, 1, 0);
}