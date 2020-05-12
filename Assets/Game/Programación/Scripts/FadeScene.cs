using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScene : MonoBehaviour {

    //Variables
    public Image m_colorFadeImage;
    public Animation m_anim;
    public AnimationClip m_FadeIn;
    public AnimationClip m_FadeOut;

    public GameObject m_colorFade;

    private void Awake()
    {
        m_colorFade.SetActive(false);
    }

    public void FadeIn()
    {
        m_colorFade.SetActive(true);

        SetFadeInAnimation();
    }

    public void FadeOut()
    {
        m_colorFade.SetActive(true);

        SetFadeOutAnimation();
    }

    public void SetFadeInAnimation()
    {
        m_anim.CrossFade(m_FadeIn.name);
    }

    public void SetFadeOutAnimation()
    {
        m_anim.CrossFade(m_FadeOut.name);
    }
}
