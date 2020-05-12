using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public List<PowerUp> availablePowerUps;
    public List<PowerUp> deployedPowerUps;
    public float spawnableRadius;
    public int numOfPowerUps;
    public List<Transform> availableSpawnPoints;
    public List<Transform> occupiedSpawnPoints;
    [Range(1,2)]
    public float velocityPowerFactor;
    public float strengthPowerFactor;
    bool player1;
    // Start is called before the first frame update
    void Start()
    {
        deployedPowerUps = new List<PowerUp>();
        occupiedSpawnPoints = new List<Transform>();
        foreach(PowerUp power in availablePowerUps) 
        {
            switch(power.powerUpType) 
            {
                case PowerUp.IPower.VELOCITY:
                    power.SetPowerToGive(velocityPowerFactor);
                    break;
                case PowerUp.IPower.POWER:
                    power.SetPowerToGive(strengthPowerFactor);
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
            availablePowerUps[0].transform.position = transform.position;
        if(Input.GetKeyDown(KeyCode.S))
            availablePowerUps[4].transform.position = transform.position;
        if(Input.GetKeyDown(KeyCode.V))
            availablePowerUps[7].transform.position = transform.position;
    }

    public void SpawnPowerUpTransform()
    {
        if(numOfPowerUps<=availablePowerUps.Count)
        {            
            for(int i = 1; i<=numOfPowerUps; ++i)
            {
               int rndIndex = Random.Range(0,availableSpawnPoints.Count-1);
               int rnd = Random.Range(0,availablePowerUps.Count-1);  
               Transform rndPosition = availableSpawnPoints[rndIndex];
               PowerUp currentPower = availablePowerUps[rnd];
               currentPower.transform.position = rndPosition.position;
               currentPower.SetUpSpawner(availableSpawnPoints[rndIndex].transform);
               availableSpawnPoints.Remove(rndPosition);
               occupiedSpawnPoints.Add(rndPosition);                           
               availablePowerUps.Remove(currentPower);
               deployedPowerUps.Add(currentPower);              
            }
        }
        else
        {
            Debug.Log("Too Many PowerUps");
            Vector3 rndPosition = Random.insideUnitSphere * spawnableRadius;
            rndPosition.y = 0.5f;
            int rnd = Random.Range(0,2);
            Instantiate(deployedPowerUps[rnd].transform.gameObject, this.transform);
        }
    }
    public void MovePowerUp()
    {
        for(int i = 0; i<deployedPowerUps.Count;++i)
        {
            int index = Random.Range(0,availableSpawnPoints.Count-1);
            occupiedSpawnPoints.Remove(deployedPowerUps[i].GetSpawnerTransform());
            availableSpawnPoints.Add(deployedPowerUps[i].GetSpawnerTransform());
            deployedPowerUps[i].transform.position = availableSpawnPoints[index].position;           
            deployedPowerUps[i].SetUpSpawner(availableSpawnPoints[index]);
        }   
    }
    public void GetPlayerScored(bool l_player1) 
    {
        player1=l_player1;
    }
    public void SetToAvailable(PowerUp currentPower) 
    {
        availablePowerUps.Add(currentPower);
        deployedPowerUps.Remove(currentPower);        
        occupiedSpawnPoints.Remove(currentPower.GetSpawnerTransform());
        availableSpawnPoints.Add(currentPower.GetSpawnerTransform());
        currentPower.ReturnToInitpos();
    }
    public void RemoveFromField() 
    {
        foreach(PowerUp power in deployedPowerUps) 
        {           
            power.ReturnToInitpos();
            availablePowerUps.Add(power);
        }
        deployedPowerUps.Clear();
    }
    public int GetNumDeployedPowers()
    {
        return deployedPowerUps.Count;
    }
}
