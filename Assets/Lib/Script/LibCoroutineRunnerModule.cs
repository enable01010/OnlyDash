using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibCoroutineRunner
{
    static LibCoroutineRunner _instnace;
    LibCoroutineRunnerModule obj;

    public static LibCoroutineRunner instance
    {
        get
        {
            _instnace = _instnace ?? new LibCoroutineRunner();
            return _instnace;
        }
    }

    private LibCoroutineRunner()
    {
        obj = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/CoroutineRunner")).GetComponent<LibCoroutineRunnerModule>();
        GameObject.DontDestroyOnLoad(obj.gameObject);
    }

    /// <summary>
    /// �R���[�`�����J�n����
    /// </summary>
    /// <param name="routine">�R���[�`����</param>
    /// <param name="callback">�R���[�`���I���R�[���o�b�N</param>
    public static void StartCoroutine(IEnumerator routine, Action callback = null)
    {
        instance.obj.StartCoroutine(routine, callback);
    }

    /// <summary>
    /// �ғ����̃R���[�`����S�Ĕj������
    /// </summary>
    public static void Dispose()
    {
        instance.obj.Dispose();
    }

    /// <summary>
    /// ����̃R���[�`����~
    /// </summary>
    /// <param name="routine">�R���[�`��</param>
    public static void StopCoroutine(IEnumerator routine)
    {
        instance.obj.StopCoroutine(routine);
    }

    /// <summary>
    /// �R���[�`����1���ȏ���s����
    /// </summary>
    /// <returns>true: ���s��</returns>
    public static bool IsRunning()
    {
        return instance.obj.IsRunning();
    }

    /// <summary>
    /// �ꎞ��~
    /// </summary>
    public static void Pause()
    {
        instance.obj.Pause();
    }

    /// <summary>
    /// �ĊJ
    /// </summary>
    public static void Resume()
    {
        instance.obj.Resume();
    }
}

public class LibCoroutineRunnerModule : MonoBehaviour
{
    private Dictionary<IEnumerator, Action> m_callbacks = new Dictionary<IEnumerator, Action>();
    private List<IEnumerator> m_routines = new List<IEnumerator>();
    private bool m_isPaused = false;

    /// <summary>
    /// �R���[�`�����J�n����
    /// </summary>
    /// <param name="routine">�R���[�`����</param>
    /// <param name="callback">�R���[�`���I���R�[���o�b�N</param>
    public void StartCoroutine(IEnumerator routine, Action callback = null)
    {
        if (callback != null)
        {
            m_callbacks.Add(routine, callback);
        }
        m_routines.Add(routine);
        routine.MoveNext(); // 1�t���[���x�����Ȃ��悤�ɑ����s
    }

    /// <summary>
    /// �ғ����̃R���[�`����S�Ĕj������
    /// </summary>
    public void Dispose()
    {
        m_routines.Clear();
    }

    /// <summary>
    /// ����̃R���[�`����~
    /// </summary>
    /// <param name="routine">�R���[�`��</param>
    public void StopCoroutine(IEnumerator routine)
    {
        m_routines.Remove(routine);
    }

    /// <summary>
    /// �X�V
    /// </summary>
    private void Update()
    {
        if (m_isPaused)
        {
            return;
        }

        for (int i = 0, len = m_routines.Count; i < len; i++)
        {
            IEnumerator routine = m_routines[i];
            object current = routine.Current;

            if (routine.MoveNext() == false)
            {
                if (m_callbacks.TryGetValue(routine, out Action callback))
                {
                    callback();
                }

                m_callbacks.Remove(routine);
                routine = null;
                m_routines.RemoveAt(i);
                i--;
                len--;
            }
        }
    }

    /// <summary>
    /// �R���[�`����1���ȏ���s����
    /// </summary>
    /// <returns>true: ���s��</returns>
    public bool IsRunning()
    {
        return m_routines.Count > 0;
    }

    /// <summary>
    /// �ꎞ��~
    /// </summary>
    public void Pause()
    {
        m_isPaused = true;
    }

    /// <summary>
    /// �ĊJ
    /// </summary>
    public void Resume()
    {
        m_isPaused = false;
    }
}
