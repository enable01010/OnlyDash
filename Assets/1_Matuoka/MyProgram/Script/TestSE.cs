using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSE : MonoBehaviour
{

    public BGMName bGMName = BGMName.BGM1;
    public SoundFxName soundFxName1;// = SoundFxName.‹Õ‚ÌŠŠ‘t;
    public SoundFxName soundFxName2;// = SoundFxName.‹Õ‚ÌŠŠ‘t;
    public SoundFxName soundFxName3;// = SoundFxName.‹Õ‚ÌŠŠ‘t;
    public SoundFxName soundFxName4;// = SoundFxName.‹Õ‚ÌŠŠ‘t;

    [SerializeField] Slider sliderMaster;
    [SerializeField] Slider sliderBGM;
    [SerializeField] Slider sliderSE;

    int a = 0;
    int b = 0;

    // Start is called before the first frame update
    void Start()
    {
        LibSound.SliderValueChange(sliderMaster, sliderBGM, sliderSE);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            LibSound.PlayBGM2D(bGMName, 0.5f, 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            LibSound.StopAll();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LibSound.StopAllBGM();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LibSound.StopBGM(bGMName);
        }


        if (Input.GetKeyDown(KeyCode.A))
        {
            LibSound.PlaySE2D(soundFxName3);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            LibSound.PlaySE3D(soundFxName3, Vector3.zero);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            LibSound.PlaySE3D(soundFxName3, LibVector.Add_X(Vector3.zero, a += 5));
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            LibSound.PlaySE3D(soundFxName3, LibVector.Add_X(Vector3.zero, a -= 15));
        }


        if (Input.GetKeyDown(KeyCode.Q))
        {
            LibSound
                .PlayOneShotSE_BuildStart()
                .SetSound(soundFxName1)
                //.SetPosition(LibVector.Add_X(Vector3.zero, a += 5))
                //.SetVolume(0.5f)
                .SetStartTime(0.1f)
                //.SetIs3D(true)
                .PlayOneShotSE();

        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            LibSound
                .PlayOneShotSE_BuildStart()
                .SetSound(soundFxName2)
                //.SetPosition(LibVector.Add_X(Vector3.zero, a += 5))
                //.SetVolume(0.5f)
                .SetStartTime(0.1f)
                //.SetIs3D(true)
                .PlayOneShotSE();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            LibSound.PlayOneShotSE2D(soundFxName3, 0.5f);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            LibSound.PlayOneShotSE2D(soundFxName4, 0.5f);
        }
    }


    public void aaa1()
    {
        LibSound.StopAll();
    }


    //public void aaa2()
    //{
    //    LibSound.PlayBGM(bGMName);
    //}

    //public void bbb1()
    //{
    //    LibSound.PlaySE(soundFxName1, 0.3f);
    //}

    //public void bbb2()
    //{
    //    LibSound.PlaySE(soundFxName2, 0.3f);
    //}

    //public void bbb3()
    //{
    //    LibSound.PlaySE(soundFxName3, 0.1f);
    //}

    //public void bbb4()
    //{
    //    LibSound.PlaySE(soundFxName4, 0.3f);
    //}

    //public void ccc1()
    //{
    //    LibSound.PlayOneShotSE(soundFxName1);
    //}

    //public void ccc2()
    //{
    //    LibSound.PlayOneShotSE(soundFxName2);
    //}

    //public void ccc3()
    //{
    //    LibSound.PlayOneShotSE(soundFxName3);
    //}

    //public void ccc4()
    //{
    //    LibSound.PlayOneShotSE(soundFxName4, 0.5f);
    //}

    public void SetAudioMixerMaster(float value)
    {
        LibSound.SetAudioMixerMaster(value);
    }

    public void SetAudioMixerBGM(float value)
    {
        LibSound.SetAudioMixerBGM(value);
    }

    public void SetAudioMixerSE(float value)
    {
        LibSound.SetAudioMixerSE(value);
    }
}
