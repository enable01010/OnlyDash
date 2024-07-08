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
    private AudioSource[] audioSourcePlayOneShot;

    private bool[] isPlayingBGM;
    private float[] startTimeBGM;

    [Header("PlayOneShot�̐�(�����d�Ȃ�)")]
    [SerializeField] private int playOneShotLength = 10;
    int playOneShotCount = 0;

    [Header("���̃v���O������AudioMixer��Volum�̍ő�l��0")]
    [SerializeField] private float audioMixerMaxVolume = 15;

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


        // BGM��Loop�p
        isPlayingBGM = new bool[bgmLength];
        startTimeBGM = new float[bgmLength];
        for (int i = 0; i < bgmLength; i++)
        {
            isPlayingBGM[i] = false;
            startTimeBGM[i] = 0f;
        }


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

            audioSourceBGM[i] = obj.GetComponent<AudioSource>();
            audioSourceBGM[i].clip = audioClipBGM[i];
            audioSourceBGM[i].outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[1];
            //audioSourceBGM[i].loop = true;
        }


        // SE Play(�����d�Ȃ炸�A�㏑�������)�pAudioSource�̐���
        audioSourcePlay = new AudioSource[soundLength];
        for (int i = 0; i < soundLength; i++)
        {
            GameObject obj = Instantiate(audioObjectPrefab);
            obj.transform.SetParent(parentPlay.transform);
            obj.name = ((SoundFxName)Enum.ToObject(typeof(SoundFxName), i)).ToString();

            audioSourcePlay[i] = obj.GetComponent<AudioSource>();
            audioSourcePlay[i].clip = audioClipSE[i];
            audioSourcePlay[i].outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[2];
        }


        // SE PlayOneShot(�����d�Ȃ�)�pAudioSource�̐���
        audioSourcePlayOneShot = new AudioSource[playOneShotLength];
        for (int i = 0; i < playOneShotLength; i++)
        {
            GameObject obj = Instantiate(audioObjectPrefab);
            obj.transform.SetParent(parentPlayOneShot.transform);
            obj.name = "PlayOneShot_" + i;

            audioSourcePlayOneShot[i] = obj.GetComponent<AudioSource>();
            audioSourcePlayOneShot[i].clip = audioClipSE[i];
            audioSourcePlayOneShot[i].outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[2];
        }


        // audioObjectPrefab�̍폜
        Destroy(audioObjectPrefab);
    }

    #endregion

    #region �X���C�_�[����AudioMixer�̉��ʂ�ς���

    public void SetAudioMixerMaster(float value)
    {
        // db�ɕϊ�
        float volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f) + audioMixerMaxVolume;

        audioMixer.SetFloat("Master", volume);
    }

    public void SetAudioMixerBGM(float value)
    {
        // db�ɕϊ�
        float volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f) + audioMixerMaxVolume;

        audioMixer.SetFloat("BGM", volume);
    }

    public void SetAudioMixerSE(float value)
    {
        // db�ɕϊ�
        float volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f) + audioMixerMaxVolume;

        audioMixer.SetFloat("SE", volume);
    }

    #endregion

    #region ���𗬂��E�~�߂�

    public void PlayBGM(int number, Vector3 position, float volume, float startTime, bool is3D)
    {
        bool isMissing = true;
        if (audioClipBGM[number] != null)
        {
            isMissing = false;

            AudioSource audio = audioSourceBGM[number];
            audio.spatialBlend = is3D ? 1.0f : 0.0f;
            audio.transform.position = position;
            audio.volume = Mathf.Clamp01(volume);
            audio.Play();
            audio.time = startTime;// Play�̌�ɐݒ�

            isPlayingBGM[number] = true;
            startTimeBGM[number] = startTime;
        }

#if UNITY_EDITOR
        if (isMissing == true)
        {
            string name = ((BGMName)Enum.ToObject(typeof(BGMName), number)).ToString();
            Debug.Log(name + "�͂���܂���");
        }
#endif
    }

    public void PlaySE(int number, Vector3 position, float volume, float startTime, bool is3D)
    {
        if(audioClipSE[number] != null)
        {
            AudioSource audio = audioSourcePlay[number];
            audio.spatialBlend = is3D ? 1.0f : 0.0f;
            audio.transform.position = position;
            audio.volume = Mathf.Clamp01(volume);
            audio.Play();
            audio.time = startTime;// Play�̌�ɐݒ�

            return;
        }

#if UNITY_EDITOR
        string name = ((SoundFxName)Enum.ToObject(typeof(SoundFxName), number)).ToString();
        Debug.Log(name + "�͂���܂���");
#endif
    }

    public void PlayOneShotSE(int number, Vector3 position, float volume, float startTime, bool is3D)
    {
        if (audioClipSE[number] != null)
        {
            AudioSource audio = audioSourcePlayOneShot[playOneShotCount];
            audio.clip = audioClipSE[number];
            audio.spatialBlend = is3D ? 1.0f : 0.0f;
            audio.transform.position = position;
            audio.volume = Mathf.Clamp01(volume);
            audio.Play();
            audio.time = startTime;// Play�̌�ɐݒ�

            audio.gameObject.name = audioClipSE[number].name;

            playOneShotCount = (playOneShotCount + 1) % playOneShotLength;

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
            isPlayingBGM[i] = false;
        }

        for (int i = 0; i < audioSourcePlay.Length; i++)
        {
            audioSourcePlay[i].Stop();
        }

        for (int i = 0; i < audioSourcePlayOneShot.Length; i++)
        {
            audioSourcePlayOneShot[i].Stop();
        }
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < audioSourceBGM.Length; i++)
        {
            audioSourceBGM[i].Stop();
            isPlayingBGM[i] = false;
        }
    }

    public void StopBGM(int number)
    {
        audioSourceBGM[number].Stop();
        isPlayingBGM[number] = false;
    }

    #endregion

    #region �X���C�_�[��AudioMixer�̒l�ɕϊ�

    public void SliderValueChange(Slider sliderMaster, Slider sliderBGM, Slider sliderSE)
    {
        // AudioMixer�̉��ʎ擾
        audioMixer.GetFloat("Master", out float masterVolume);
        audioMixer.GetFloat("BGM", out float bgmVolume);
        audioMixer.GetFloat("SE", out float seVolume);


        // �������ʐݒ�
        sliderMaster.value = Mathf.Pow(10f, (masterVolume - audioMixerMaxVolume) / 20f);
        sliderBGM.value = Mathf.Pow(10f, (bgmVolume - audioMixerMaxVolume) / 20f);
        sliderSE.value = Mathf.Pow(10f, (seVolume - audioMixerMaxVolume) / 20f);
    }

    #endregion

    #endregion


    private void Update()
    {
        for (int i = 0; i < bgmLength; i++)
        {
            if (isPlayingBGM[i] == true)
            {
                if (audioSourceBGM[i].isPlaying == false)
                {
                    audioSourceBGM[i].Play();
                    audioSourceBGM[i].time = startTimeBGM[i];// Play�̌�ɐݒ�
                }
            }
        }
    }
}

public class LibSound
{
    #region Singleton�֌W�Ȃ�

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

        GameObject tempObject = UnityEngine.Object.Instantiate(prefab);

        module = tempObject.GetComponent<LibSoundModule>();

        module.Init();

        UnityEngine.Object.DontDestroyOnLoad(tempObject);
    }

    #endregion

    #region CustomMethod

    #region �X���C�_�[����AudioMixer�̉��ʂ�ς���

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

    public static void PlayBGM3D(BGMName sound, Vector3 position, float volume = 0.5f, float startTime = 0)
    {
        instance.module.PlayBGM((int)sound, position, volume, startTime, true);
    }

    public static void PlayBGM2D(BGMName sound, float volume = 0.5f, float startTime = 0)
    {
        instance.module.PlayBGM((int)sound, Vector3.zero, volume, startTime, false);
    }

    public static void PlaySE3D(SoundFxName sound, Vector3 position, float volume = 0.5f, float startTime = 0)
    {
        instance.module.PlaySE((int)sound, position, volume, startTime, true);
    }

    public static void PlaySE2D(SoundFxName sound, float volume = 0.5f, float startTime = 0)
    {
        instance.module.PlaySE((int)sound, Vector3.zero, volume, startTime, false);
    }

    public static void PlayOneShotSE3D(SoundFxName sound, Vector3 position, float volume = 0.5f, float startTime = 0)
    {
        instance.module.PlayOneShotSE((int)sound, position, volume, startTime, true);
    }

    public static void PlayOneShotSE2D(SoundFxName sound, float volume = 0.5f, float startTime = 0)
    {
        instance.module.PlayOneShotSE((int)sound, Vector3.zero, volume, startTime, false);
    }

    public static void StopAll()
    {
        instance.module.StopAll();
    }

    public static void StopAllBGM()
    {
        instance.module.StopAllBGM();
    }

    public static void StopBGM(BGMName sound)
    {
        instance.module.StopBGM((int)sound);
    }

    #endregion

    #region ���𗬂��@Builder�p�^�[��

    // PlayBGM_Builder�p�^�[��
    static public PlayBGM_Builder PlayBGM_BuildStart()
    {
        return new PlayBGM_Builder();
    }

    public class PlayBGM_Builder
    {
        BGMName sound = 0;
        Vector3 position = Vector3.zero;
        float volume = 0.5f;
        float startTime = 0;
        bool is3D = false;

        public PlayBGM_Builder SetSound(BGMName sound)
        {
            this.sound = sound;
            return this;
        }
        public PlayBGM_Builder SetPosition(Vector3 position)
        {
            this.position = position;
            return this;
        }
        public PlayBGM_Builder SetVolume(float volume)
        {
            this.volume = volume;
            return this;
        }
        public PlayBGM_Builder SetStartTime(float startTime)
        {
            this.startTime = startTime;
            return this;
        }
        public PlayBGM_Builder SetIs3D(bool is3D)
        {
            this.is3D = is3D;
            return this;
        }

        public void PlayBGM()
        {
            instance.module.PlayBGM((int)sound, position, volume, startTime, is3D);
        }
    }


    // PlaySE_Builder�p�^�[��
    static public PlaySE_Builder PlaySE_BuildStart()
    {
        return new PlaySE_Builder();
    }

    public class PlaySE_Builder
    {
        SoundFxName sound = 0;
        Vector3 position = Vector3.zero;
        float volume = 0.5f;
        float startTime = 0;
        bool is3D = false;

        public PlaySE_Builder SetSound(SoundFxName sound)
        {
            this.sound = sound;
            return this;
        }
        public PlaySE_Builder SetPosition(Vector3 position)
        {
            this.position = position;
            return this;
        }
        public PlaySE_Builder SetVolume(float volume)
        {
            this.volume = volume;
            return this;
        }
        public PlaySE_Builder SetStartTime(float startTime)
        {
            this.startTime = startTime;
            return this;
        }
        public PlaySE_Builder SetIs3D(bool is3D)
        {
            this.is3D = is3D;
            return this;
        }

        public void PlaySE()
        {
            instance.module.PlaySE((int)sound, position, volume, startTime, is3D);
        }
    }


    // PlayOneShotSE_Builder�p�^�[��
    static public PlayOneShotSE_Builder PlayOneShotSE_BuildStart()
    {
        return new PlayOneShotSE_Builder();
    }

    public class PlayOneShotSE_Builder
    {
        SoundFxName sound = 0;
        Vector3 position = Vector3.zero;
        float volume = 0.5f;
        float startTime = 0;
        bool is3D = false;

        public PlayOneShotSE_Builder SetSound(SoundFxName sound)
        {
            this.sound = sound;
            return this;
        }
        public PlayOneShotSE_Builder SetPosition(Vector3 position)
        {
            this.position = position;
            return this;
        }
        public PlayOneShotSE_Builder SetVolume(float volume)
        {
            this.volume = volume;
            return this;
        }
        public PlayOneShotSE_Builder SetStartTime(float startTime)
        {
            this.startTime = startTime;
            return this;
        }
        public PlayOneShotSE_Builder SetIs3D(bool is3D)
        {
            this.is3D = is3D;
            return this;
        }

        public void PlayOneShotSE()
        {
            instance.module.PlayOneShotSE((int)sound, position, volume, startTime, is3D);
        }
    }

    #endregion

    #region �X���C�_�[��AudioMixer�̒l�ɕϊ�

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
    �Ղ̊��t,
    �Q�[�W��2,
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


