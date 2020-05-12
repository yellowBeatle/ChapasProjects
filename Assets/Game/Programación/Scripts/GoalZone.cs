using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class GoalZone : MonoBehaviour
{
    public enum TZones
    {
        LEFTZONE,
        RIGHTZONE
    }
    public TZones m_zones;

    public GameManager m_gameManager;

    public VisualEffect vfx;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "CapsPlayerOne" && other.transform == m_gameManager.playerWithBall)
        {
            if (m_zones == TZones.RIGHTZONE)
            {
                m_gameManager.updateScore(false, 1);
            }

            if (m_zones == TZones.LEFTZONE)
            {
                m_gameManager.updateScore(true, 1);
            }
            StartCoroutine(TimeFireworks());

        }
        else if (other.transform.tag == "CapsPlayerTwo" && other.transform == m_gameManager.playerWithBall)
        {
            if (m_zones == TZones.LEFTZONE)
            {
                m_gameManager.updateScore(true, 1);
            }

            if (m_zones == TZones.RIGHTZONE)
            {
                m_gameManager.updateScore(false, 1);
            }
            StartCoroutine(TimeFireworks());
        }
        
    }

    IEnumerator TimeFireworks()
    {
        //vfx.gameObject.transform.position = this.transform.GetChild(0).transform.position;

        StartVFX();

        yield return new WaitForSeconds(4.5f);

        StopVFX();
    }

    void StartVFX()
    {
        vfx.enabled = true;
        vfx.Play();
    }

    void StopVFX()
    {
        //vfx.enabled = false;
        //vfx.Reinit();
        vfx.Stop();
    }
}
