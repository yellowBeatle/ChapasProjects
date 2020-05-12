using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum IPower 
    {
        SHIELD, VELOCITY, POWER , NONE  
    }
    public IPower powerUpType;
    float powerToGive;
    Vector3 initPos;
    Transform currentSpawner;
    public AudioSource picked;
    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.position;

        picked = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "CapsPlayerOne" || other.transform.tag == "CapsPlayerTwo") 
        {
            picked.Play();
            GameManager.instance.powerSpawner.SetToAvailable(this);
            GivePowerToTeam(other.transform.tag);
        }
    }
    public void ReturnToInitpos() 
    {
        transform.position = initPos;
    }
    public void SetPowerToGive(float power) 
    {
        powerToGive = power;
    }
    public void SetUpSpawner(Transform spawnerPower)
    {
        currentSpawner=spawnerPower;
    }
    public Transform GetSpawnerTransform()
    {
        return currentSpawner;
    }
    void GivePowerToTeam(string capTeam)
    {
        if(capTeam == "CapsPlayerOne")
        {
            foreach(Caps cap in GameManager.instance.playersTeamPlayer1)
            {
                cap.GivePowerUp(powerUpType);
                cap.SetPowerFactor(powerToGive);
            }
        }else if(capTeam == "CapsPlayerTwo")
        {
            foreach(Caps cap in GameManager.instance.playersTeamPlayer2)
            {
                cap.GivePowerUp(powerUpType);
                cap.SetPowerFactor(powerToGive);
            }
        }        
    }
}
