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
        LibSound.PlayBGM(bGMName);
    }

    public void bbb1()
    {
        LibSound.PlaySE(soundFxName1, 0.3f);
    }

    public void bbb2()
    {
        LibSound.PlaySE(soundFxName2, 0.3f);
    }

    public void bbb3()
    {
        LibSound.PlaySE(soundFxName3, 0.1f);
    }

    public void bbb4()
    {
        LibSound.PlaySE(soundFxName4, 0.3f);
    }

    public void ccc1()
    {
        LibSound.PlayOneShotSE(soundFxName1);
    }

    public void ccc2()
    {
        LibSound.PlayOneShotSE(soundFxName2);
    }

    public void ccc3()
    {
        LibSound.PlayOneShotSE(soundFxName3);
    }

    public void ccc4()
    {
        LibSound.PlayOneShotSE(soundFxName4, 0.5f);
    }


}
