using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSE : MonoBehaviour
{

    public BGMName bGMName = BGMName.BGM1;
    public SoundFxName soundFxName1 = SoundFxName.‹Õ‚ÌŠŠ‘t;
    public SoundFxName soundFxName2 = SoundFxName.‹Õ‚ÌŠŠ‘t;
    public SoundFxName soundFxName3 = SoundFxName.‹Õ‚ÌŠŠ‘t;
    public SoundFxName soundFxName4 = SoundFxName.‹Õ‚ÌŠŠ‘t;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void aaa1()
    {
        LibSound.StopAll();
    }


    public void aaa2()
    {
        LibSound.PlayBGM(bGMName, 3f);
    }

    public void bbb1()
    {
        LibSound.PlaySoloSE(soundFxName1);
    }

    public void bbb2()
    {
        LibSound.PlaySoloSE(soundFxName2);
    }

    public void bbb3()
    {
        LibSound.PlaySoloSE(soundFxName3);
    }

    public void bbb4()
    {
        LibSound.PlaySoloSE(soundFxName4, 0.5f);
    }

    public void ccc1()
    {
        LibSound.PlayTrollSE(soundFxName1);
    }

    public void ccc2()
    {
        LibSound.PlayTrollSE(soundFxName2);
    }

    public void ccc3()
    {
        LibSound.PlayTrollSE(soundFxName3);
    }

    public void ccc4()
    {
        LibSound.PlayTrollSE(soundFxName4, 0.5f);
    }


}
