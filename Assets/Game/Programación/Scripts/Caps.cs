using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caps : MonoBehaviour, IRestartGameElement
{
    public enum TPlayers
    {
        PLAYERONE,
        PLAYERTWO
    }

    public enum TBall
    {
        NONE,
        DEAD,
        WITHBALL
    }

    public TPlayers m_myTeam;
    public TBall m_ball;

    //REFERENCES

    public GameManager m_gameManager;

    //BASIC VARIABLES CAPS

    public Rigidbody m_rigidBody;
    public float m_force = 20;
    public float m_maxColForce = 12;
    public float m_detectionRadius;
    public Color deathColor;
    public LayerMask capsLayer;
    public float explotionForce;
    public float powerDistance;
    public Transform myShieldForce;
    float powerUpFactor;
    bool shieldOn;
    bool usePower;
    List<Rigidbody> capsNeightboors;

    Vector3 m_direction;
    PowerUp.IPower m_powerUpType;
    //bool m_ableToMove = true;
    //bool m_isMyTurn;
    float m_currentForce;

    public int m_deadToAliveTurn = 0;

    public AudioSource clash;

    //RESTART VARIABLES

    Vector3 m_defaultPos = Vector3.zero;

    //HEALTH VARIABLES

    public HealthBar m_healthBar;

    private int m_maxHealth = 100;
    private int m_currentHealth;

    //FOR POWER UP
    MeshRenderer m_meshRenderer;
    bool speedOn;
    public Material m_ElectricityMat;

    void Start()
    {
        StartPlayer();
    }

    public void StartPlayer()
    {
        m_rigidBody = GetComponent<Rigidbody>();

        m_defaultPos = transform.position;

        m_gameManager.AddRestartGameElement(this);

        capsNeightboors = new List<Rigidbody>();

        m_currentHealth = m_maxHealth;

        m_powerUpType = PowerUp.IPower.NONE;

        m_healthBar.SetMaxHealth(m_maxHealth);

        myShieldForce.gameObject.SetActive(false);

        m_meshRenderer = GetComponent<MeshRenderer>();

        clash = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {        
        if (collision.transform.tag == "CapsPlayerOne" || collision.transform.tag == "CapsPlayerTwo")
        {
            clash.Play();
            if (collision.gameObject.GetComponent<Caps>().m_rigidBody.velocity != Vector3.zero && collision.transform == m_gameManager.lastObjectSelected)
            {
                //Debug.Log(collision.transform.name);
                collision.transform.gameObject.GetComponent<Caps>().m_rigidBody.velocity = Vector3.zero;
                Vector3 colDirection = collision.transform.position - transform.position;
                colDirection.Normalize();
                transform.GetComponent<Rigidbody>().AddForce(-colDirection * m_force, ForceMode.Impulse);               
            }
            if(m_ball == TBall.WITHBALL)
            {
                if(collision.transform.GetComponent<Caps>().m_ball != TBall.DEAD)
                {
                    if(collision.transform.tag != transform.tag)
                    {
                        if(!shieldOn)
                        {
                            //m_gameManager.ChangeColorOneCap(this.transform);

                            m_gameManager.playerWithBall = collision.transform;

                            collision.transform.GetComponent<Caps>().SetWithBall(collision.transform);
                            
                            transform.GetComponent<MeshRenderer>().material.SetColor("_BaseColor",deathColor);                            

                            m_deadToAliveTurn += 1;

                            TakeDamage(34);

                            SetDead();
                        }
                        shieldOn = false;
                        myShieldForce.gameObject.SetActive(false);
                        GivePowerUp(PowerUp.IPower.NONE);
                    }
                }
            }
        }
    }

   
    
    //SATRT CAPS MOVEMENT SYSTEM

    private void OnMouseDrag()
    {
        if (this.transform == m_gameManager.lastObjectSelected && this.m_ball != TBall.DEAD)
        {
            m_currentForce = m_direction.magnitude * 2;
            Mathf.Clamp(m_currentForce, 0f, m_force);
            if(m_powerUpType!=PowerUp.IPower.NONE && Input.GetMouseButtonDown(1)) 
               ApplyPowerUp();             
            //LineRenderer
            m_gameManager.line.gameObject.SetActive(true);
            LineRendererUpdate();
            if(usePower && m_powerUpType==PowerUp.IPower.POWER)
                UseStrength();
        }
    }
    
    private void OnMouseUp()
    {
        if (this.transform == m_gameManager.lastObjectSelected && this.m_ball != TBall.DEAD)
        {
            //m_moving = true; //Si la fuerza es cero false, sino true.
            if(m_powerUpType == PowerUp.IPower.VELOCITY && usePower)
            {
               m_gameManager.lastObjectSelected.GetComponent<Rigidbody>().AddForce(m_currentForce*powerUpFactor * m_direction, ForceMode.Impulse);
                ResetPowerUp();
                DisableElectricityMaterial();
            }
            else 
                m_gameManager.lastObjectSelected.GetComponent<Rigidbody>().AddForce(m_currentForce * m_direction, ForceMode.Impulse);
                
            m_gameManager.SetChangeTurnState();            

            //LineRenderer
            m_gameManager.line.gameObject.SetActive(false);
        }
    }

    //END CAPS MOVEMENT SYSTEM

    //EXPLOTE FUNCTIONS
    public void Explote()
    {        
        if(CanExplode())
        {
            foreach(Rigidbody body in capsNeightboors) 
            {
                Vector3 direction = (transform.position-body.transform.position).normalized;
                body.AddForce(-direction*explotionForce, ForceMode.Impulse);
            }
            capsNeightboors.Clear();
        }
        capsNeightboors.Clear();
    }

    bool CanExplode()
    {        
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_detectionRadius, capsLayer);
                   
            foreach(Collider col in colliders) 
            {

                if(col.transform != transform)
                    capsNeightboors.Add(col.transform.GetComponent<Rigidbody>());
            }
        return capsNeightboors.Count>=3;
    }
    //START LINE RENDERER FUNCTIONS

    public void LineRendererUpdate()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var direction = Vector3.zero;

        if (Physics.Raycast(ray, out hit))
        {
            var chapaPos = new Vector3(transform.position.x, 0.1f, transform.position.z);
            var mousePos = new Vector3(hit.point.x, 0.1f, hit.point.z);
            m_gameManager.DistLine = (mousePos - chapaPos).magnitude;

            if (m_gameManager.DistLine < m_gameManager.MaxDistLine)
            {
                m_gameManager.line.SetPosition(0, mousePos);
                m_gameManager.line.SetPosition(1, chapaPos);
                direction = -(mousePos - chapaPos);
                m_direction = direction;
                direction = direction.normalized;
            }
            else
            {
                float orientationMouse = m_gameManager.VectorToOrientation(mousePos - chapaPos);

                Vector3 MaxMousePos = (mousePos - chapaPos).normalized * (m_gameManager.MaxDistLine - 1);
                MaxMousePos += chapaPos + m_gameManager.OrientationToVector(orientationMouse);
                MaxMousePos = new Vector3(MaxMousePos.x, 0.1f, MaxMousePos.z);

                m_gameManager.line.SetPosition(0, MaxMousePos);
                m_gameManager.line.SetPosition(1, chapaPos);
                direction = -(MaxMousePos - chapaPos);
                m_direction = direction;
                direction = direction.normalized;
            }
        }
    }

    //END LINE RENDERER FUNCTIONS

    //POWERUP FUNCTIONS

    void ApplyPowerUp() 
    {        
        switch(m_powerUpType)
        {
            case PowerUp.IPower.POWER:

                usePower = true;
            
                break;
            case PowerUp.IPower.SHIELD:

                shieldOn = true;
                myShieldForce.gameObject.SetActive(true);
                
                break;
            case PowerUp.IPower.VELOCITY:
                speedOn = true;                
                usePower = true;

                break;
        }
    }

    public void ResetPowerUp() 
    {
        usePower = false;
        m_powerUpType=PowerUp.IPower.NONE;
    }
    void UseStrength() 
    {
        RaycastHit hitCap;        
        if( Physics.Raycast(transform.position,m_direction,out hitCap,powerDistance,capsLayer))
        {
           float deltaY = transform.position.y-hitCap.transform.position.y;
           Vector3 capDirection;
           if(deltaY>=0)
              capDirection = Vector3.forward;
           else
              capDirection = Vector3.back;
           hitCap.rigidbody.AddForce(capDirection*powerUpFactor,ForceMode.Impulse);           
        }
        ResetPowerUp();
    }

    //START OTHER FUNCTIONS

    public void SetWithBall(Transform cap)
    {
        m_ball = TBall.WITHBALL;
        GameManager.instance.ball.SetParent(cap);
        GameManager.instance.ball.localPosition = Vector3.forward*0.2f;
    }

    public void SetNone()
    {
        m_ball = TBall.NONE;
    }

    public void SetDead()
    {
        m_ball = TBall.DEAD;
    }

    public string checkStatus()
    {
        if (m_ball == TBall.WITHBALL)
        {
            return "WithBall";
        }
        else if (m_ball == TBall.NONE)
        {
            return "None";
        }
        else
        {
            return "Dead";
        }
    }

    //END OTHER FUCTIONS

    //START DAMEGE FUNTCTION

    public void TakeDamage(int damge)
    {
        if (m_currentHealth - damge <= 0)
        {
            m_currentHealth = 0;
            m_gameManager.DestroyDeathCap(this);
            Destroy(this.gameObject);
        }
        else
        {
            m_currentHealth -= damge;
        }

        m_healthBar.SetHealthBar(m_currentHealth);
    }

    //END DAMAGE FUNCTIONS

    //START VFX FUNCTION

    public void VFXStart(ParticleSystem particles, Vector3 position)
    {
        //Cada sistema de partículas se destruye despues de finalizar.
    }

    //END VFX FUNCTION

    //SATRT RESTART FUNCTION

    public void RestartGame()
    {
        transform.position = m_defaultPos;

        m_rigidBody.velocity = Vector3.zero;

        SetNone(); //
    }

    //END RESTART FUNCTION

    //START POWER UP FUNCTIONS

    private void Update()
    {
        checkLastPlayerStatus();
    }

    public void SetElectricityMaterial()
    {
        Material[] materials = new Material[2];

        materials[0] = m_meshRenderer.materials[0];

        materials[1] = m_ElectricityMat;

        m_meshRenderer.materials = materials;
    }

    public void DisableElectricityMaterial()
    {
        Material[] materials = new Material[1];

        materials[0] = m_meshRenderer.materials[0];

        m_meshRenderer.materials = materials;
    }

    //GETTERS

    public bool HasShieldOn() 
    {
        return shieldOn;
    }
    public bool HasSpeedOn()
    {
        return speedOn;
    }
    public bool HasUsedPowerUp() 
    {
        return usePower;
    }

    //SETTERS

    public void GivePowerUp(PowerUp.IPower powerUpType) 
    {
        m_powerUpType = powerUpType;
        if(m_powerUpType == PowerUp.IPower.SHIELD)
            ApplyPowerUp();
        else if (m_powerUpType == PowerUp.IPower.VELOCITY)
        {
           SetElectricityMaterial();
        }else if(m_powerUpType == PowerUp.IPower.NONE)
        {
            usePower=false;
            shieldOn=false;
            DisableElectricityMaterial();
            myShieldForce.gameObject.SetActive(false);
        }
    }
    public void DisableSpeed()
    {
        DisableElectricityMaterial();
        speedOn = false;
    }
    public void DisableShield() 
    {
        shieldOn = false;
    }

    public void SetPowerFactor(float factor) 
    {
        powerUpFactor = factor;
    }

    //END POWER UP FUNCTIONS

    public void checkLastPlayerStatus()
    {
        if (m_gameManager.m_deathCapsPlayerOne == 2 || m_gameManager.m_deathCapsPlayerTwo == 2)
        {
            Debug.Log("Checking");
            if (m_ball == TBall.DEAD)
            {
                m_gameManager.ChangeTurn();
            }
        }
    }
}

//END CAPS
