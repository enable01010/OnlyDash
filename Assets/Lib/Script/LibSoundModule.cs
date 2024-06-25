using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;

internal class LibSoundModule : MonoBehaviour
{
    #region Fields

    [SerializeField] private AudioMixer audioMixer;
    private GameObject audioObjectPrefab;

    private int bgmLength;
    private int soundLength;

    private AudioClip[] audioClipBGM;
    private AudioClip[] audioClipSE;

    private AudioSource[] audioSourceBGM;
    private AudioSource[] audioSourcePlay;
    private AudioSource audioSourcePlayOneShot;

    private Transform[] transformBGM;
    private Transform[] transformPlay;
    private Transform transformPlayOneShot;

    #endregion

    #region CustomMethod

    #region Init

    public void Init()
    {
        // �I�u�W�F�N�g����
        GameObject parentSound = new GameObject("Sound");
        GameObject parentBGM = new GameObject("BGM");
        GameObject parentSE = new GameObject("SE");
        GameObject parentPlay = new GameObject("Play");
        GameObject parentPlayOneShot = new GameObject("PlayOneShot");

        parentSound.transform.SetParent(this.transform);
        parentBGM.transform.SetParent(parentSound.transform);
        parentSE.transform.SetParent(parentSound.transform);
        parentPlay.transform.SetParent(parentSE.transform);
        parentPlayOneShot.transform.SetParent(parentSE.transform);


        // AudioClip��enum���Ƀ\�[�g���ē����
        bgmLength = Enum.GetValues(typeof(BGMName)).Length;
        soundLength = Enum.GetValues(typeof(SoundFxName)).Length;

        audioClipBGM = new AudioClip[bgmLength];
        audioClipSE = new AudioClip[soundLength];

        audioClipBGM = Resources.LoadAll<AudioClip>("Sound/BGM");
        audioClipSE = Resources.LoadAll<AudioClip>("Sound/SE");

        Array.Resize(ref audioClipBGM, bgmLength);// enum�̐��܂Ŕz��g��
        Array.Resize(ref audioClipSE, soundLength);// enum�̐��܂Ŕz��g��

        transformBGM = new Transform[bgmLength];
        transformPlay = new Transform[soundLength];


        // BGM���\�[�g����audioClipBGM�ɓ����
        for (int i = 0; i < bgmLength; i++)
        {
            BGMName bGMName = (BGMName)Enum.ToObject(typeof(BGMName), i);

            for (int j = 0; j < bgmLength; j++)
            {
                if (audioClipBGM[j] != null && audioClipBGM[j].name == bGMName.ToString())
                {
                    AudioClip temp = audioClipBGM[i];
                    audioClipBGM[i] = audioClipBGM[j];
                    audioClipBGM[j] = temp;
                    break;
                }
            }
        }

        // Sound���\�[�g����audioClipSE�ɓ����
        for (int i = 0; i < soundLength; i++)
        {
            SoundFxName soundFxName = (SoundFxName)Enum.ToObject(typeof(SoundFxName), i);

            for (int j = 0; j < soundLength; j++)
            {
                if (audioClipSE[j] != null && audioClipSE[j].name == soundFxName.ToString())
                {
                    AudioClip temp = audioClipSE[i];
                    audioClipSE[i] = audioClipSE[j];
                    audioClipSE[j] = temp;
                    break;
                }
            }
        }


        // audioObjectPrefab�̐���
        audioObjectPrefab = new GameObject("AudioSource");
        audioObjectPrefab.AddComponent<AudioSource>();


        // BGM�pAudioSource�̐���
        audioSourceBGM = new AudioSource[bgmLength];
        for (int i = 0; i < bgmLength; i++)
        {
            GameObject obj = Instantiate(audioObjectPrefab);
            obj.transform.SetParent(parentBGM.transform);
            obj.name = ((BGMName)Enum.ToObject(typeof(BGMName), i)).ToString();

            transformBGM[i] = obj.transform;

            audioSourceBGM[i] = obj.GetComponent<AudioSource>();
            audioSourceBGM[i].clip = audioClipBGM[i];
            audioSourceBGM[i].outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[1];
            audioSourceBGM[i].loop = true;
        }


        // SE Play(�����d�Ȃ炸�A�㏑�������)�pAudioSource�̐���
        audioSourcePlay = new AudioSource[soundLength];
        for (int i = 0; i < soundLength; i++)
        {
            GameObject obj = Instantiate(audioObjectPrefab);
            obj.transform.SetParent(parentPlay.transform);
            obj.name = ((SoundFxName)Enum.ToObject(typeof(SoundFxName), i)).ToString();

            transformPlay[i] = obj.transform;

            audioSourcePlay[i] = obj.GetComponent<AudioSource>();
            audioSourcePlay[i].clip = audioClipSE[i];
            audioSourcePlay[i].outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[2];
        }


        // SE PlayOneShot(�����d�Ȃ�)�pAudioSource�̐���
        GameObject objShot = Instantiate(audioObjectPrefab);
        objShot.transform.SetParent(parentPlayOneShot.transform);
        objShot.name = "PlayOneShot";

        transformPlayOneShot = objShot.transform;

        audioSourcePlayOneShot = objShot.GetComponent<AudioSource>();
        audioSourcePlayOneShot.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[2];


        // audioObjectPrefab�̍폜
        Destroy(audioObjectPrefab);
    }

    #endregion

    #region AudioMixer�̉��ʂ�ς���

    public void SetAudioMixerMaster(float value)
    {
        // db�ɕϊ�
        float volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);

        audioMixer.SetFloat("Master", volume);
    }

    public void SetAudioMixerBGM(float value)
    {
        // db�ɕϊ�
        float volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);

        audioMixer.SetFloat("BGM", volume);
    }

    public void SetAudioMixerSE(float value)
    {
        // db�ɕϊ�
        float volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);

        audioMixer.SetFloat("SE", volume);
    }

    #endregion

    #region ���𗬂��E�~�߂�

    public void PlayBGM(int number, float volume, Vector3 position)
    {
        bool isMissing = true;
        for (int i = 0; i < audioSourceBGM.Length; i++)
        {
            if (i == number)
            {
                if (audioClipBGM[number] != null)
                {
                    AudioSource audio = audioSourceBGM[number];
                    audio.Play();
                    isMissing = false;
                }
            }
            else
            {
                audioSourceBGM[i].Stop();
            }
        }

#if UNITY_EDITOR
        if (isMissing == true)
        {
            string name = ((BGMName)Enum.ToObject(typeof(BGMName), number)).ToString();
            Debug.Log(name + "�͂���܂���");
        }
#endif
    }

    public void PlaySE(int number, float volume, float startTime, Vector3 position)
    {
        if(audioClipSE[number] != null)
        {
            AudioSource audio = audioSourcePlay[number];
            audio.Play();
            audio.time = startTime;// Play�̌�ɐݒ�

            return;
        }

#if UNITY_EDITOR
        string name = ((SoundFxName)Enum.ToObject(typeof(SoundFxName), number)).ToString();
        Debug.Log(name + "�͂���܂���");
#endif
    }

    public void PlayOneShotSE(int number, float volume, float startTime, Vector3 position)
    {
        if (audioClipSE[number] != null)
        {
            AudioSource audio = audioSourcePlayOneShot;
            audio.time = startTime;////////////////////////////////////////�_��
            audio.PlayOneShot(audioClipSE[number]);

            return;
        }

#if UNITY_EDITOR
        string name = ((SoundFxName)Enum.ToObject(typeof(SoundFxName), number)).ToString();
        Debug.Log(name + "�͂���܂���");
#endif
    }

    public void StopAll()
    {
        for (int i = 0; i < audioSourceBGM.Length; i++)
        {
            audioSourceBGM[i].Stop();
        }

        for (int i = 0; i < audioSourcePlay.Length; i++)
        {
            audioSourcePlay[i].Stop();
        }

        audioSourcePlayOneShot.Stop();
    }

    #endregion

    #region �X���C�_�[��AudioMixer�̒l�ɕϊ�

    //public float AudioMixerGetFloat(string name)
    //{
    //    audioMixer.GetFloat(name, out float sliderMasterValue);

    //    return sliderMasterValue;
    //}

    public void SliderValueChange(Slider sliderMaster, Slider sliderBGM, Slider sliderSE)
    {
        // AudioMixer�̉��ʎ擾
        audioMixer.GetFloat("Master", out float masterVolume);
        audioMixer.GetFloat("BGM", out float bgmVolume);
        audioMixer.GetFloat("SE", out float seVolume);


        // �������ʐݒ�
        sliderMaster.value = Mathf.Pow(10f, masterVolume / 20f);
        sliderBGM.value = Mathf.Pow(10f, bgmVolume / 20f);
        sliderSE.value = Mathf.Pow(10f, seVolume / 20f);
    }

    #endregion

    #endregion
}

public class LibSound
{
    private static LibSound instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new LibSound();
            }
            return m_Instance; 
        }
    }

    private static LibSound m_Instance;

    private LibSoundModule module;

    private LibSound() 
    {
        GameObject prefab = (GameObject)Resources.Load("Prefabs/SoundModule");

        GameObject tempObject = UnityEngine.Object.Instantiate(prefab);//////////////////////////////////////////

        module = tempObject.GetComponent<LibSoundModule>();

        module.Init();

        UnityEngine.Object.DontDestroyOnLoad(tempObject);////////////////////////////////////////////////////////
    }

    #region CustomMethod

    #region AudioMixer�̉��ʂ�ς���

    public static void SetAudioMixerMaster(float value)
    {
        instance.module.SetAudioMixerMaster(value);
    }

    public static void SetAudioMixerBGM(float value)
    {
        instance.module.SetAudioMixerBGM(value);
    }

    public static void SetAudioMixerSE(float value)
    {
        instance.module.SetAudioMixerSE(value);
    }

    #endregion

    #region ���𗬂��E�~�߂�

    public static void PlayBGM(BGMName sound, float volume, Vector3 position)
    {
        instance.module.PlayBGM((int)sound, volume, position);
    }
    public static void PlayBGM(BGMName sound)
    {
        instance.module.PlayBGM((int)sound, 0, Vector3.zero);
    }

    public static void PlaySE(SoundFxName sound, float volume, float startTime, Vector3 position)
    {
        instance.module.PlaySE((int)sound, volume, startTime, position);
    }

    public static void PlaySE(SoundFxName sound, float startTime = 0)
    {
        instance.module.PlaySE((int)sound, 0, startTime, Vector3.zero);
    }

    public static void PlayOneShotSE(SoundFxName sound, float volume, float startTime, Vector3 position)
    {
        instance.module.PlayOneShotSE((int)sound, volume, startTime, position);
    }

    public static void PlayOneShotSE(SoundFxName sound, float startTime = 0)
    {
        instance.module.PlayOneShotSE((int)sound, 0, startTime, Vector3.zero);
    }

    public static void StopAll()
    {
        instance.module.StopAll();
    }

    #endregion

    #region �X���C�_�[��AudioMixer�̒l�ɕϊ�

    //public static float AudioMixerGetFloat(string name)
    //{
    //    return instance.module.AudioMixerGetFloat(name);
    //}

    public static void SliderValueChange(Slider sliderMaster, Slider sliderBGM, Slider sliderSE)
    {
        instance.module.SliderValueChange(sliderMaster, sliderBGM, sliderSE);
    }

    #endregion

    #endregion
}

public enum BGMName
{
    BGM1 = 0,
    BGM2,
    BGM3,
}

public enum SoundFxName
{
    CharacterReadyJump = 0,
    CharacterJump,
    CharacterLanding,
    �Ղ̊��t,
    CharacterCollision,
    CharacterReadyWallKick,
    CharacterWallKick,
    �Q�[�W��2,
    TutorialOpen,
    TutorialClose,
    SettingOpen,
    SettingClose,
    GoalOpen,
    ����[��2,
    SceneLoad,
    Star,
    ResultTime,
    ResultTimeStop,
    Cheer,
    Window,
    ����{�^��������51,
    ClearImage,
    Goal,
}


