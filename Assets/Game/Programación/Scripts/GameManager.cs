using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public interface IRestartGameElement
{
    void RestartGame();
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum TState
    {
        START,
        SELECT,
        ATTACK,
        CHANGETURN,
        WAIT,
        RESTART,
        END
    }

    public TState m_State;

    List<IRestartGameElement> m_RestartGameElements = new List<IRestartGameElement>();

    //BASIC PLAYER VARIABLES

    //public Caps[] playersTeamPlayer1;
    //public Caps[] playersTeamPlayer2;
    public List<Caps> playersTeamPlayer1 = new List<Caps>();
    public List<Caps> playersTeamPlayer2 = new List<Caps>();
    

    public Transform playerWithBall;
    public Transform lastObjectSelected;

    //LINE RENDERER VARIABLES

    public LineRenderer line;
    public float DistLine;
    public float MaxDistLine = 0.5f;

    //TURN VARIABLES

    public bool turnPlayerOne = false;

    public bool turnPlayerTwo = false;

    //OTHER VARIABLES

    public float m_CurrentTime = 0f;
    

    //DIALOG VARIABLES

    public TextMeshProUGUI m_turnText;

    public TextMeshProUGUI m_attackText;

    public TextMeshProUGUI m_scorePlayerOne;

    public TextMeshProUGUI m_scorePlayerTwo;

    public TextMeshProUGUI m_goalText;

    public TextMeshProUGUI m_WinDialog;

    public TextMeshProUGUI m_playerOneTextColor;

    public TextMeshProUGUI m_playerTwoTextColor;

    public bool once = true;

    //SCORE VARIABLES

    public int m_totalScorePlayerOne = 0;

    public int m_totalScorePlayerTwo = 0;

    public int m_deathCapsPlayerOne = 0;

    public int m_deathCapsPlayerTwo = 0;

    public bool m_score = false;

    //FOR WAIT STATE VARIABLES

    public float m_timeToWaitAfterStop = 0.25f; //0.25f

    private float m_currentWaitTime = 0f;

    //SECENE MANAGER

    SceneController scene;

    //FADE MANAGER

    FadeScene fade;

    //SPAWN MANAGER

    public PowerUpSpawner powerSpawner;

    //SPAWN VARIABLES

    int currentTurnsToSpawn;

    int currentTurnsToMove;

    bool hasSpawned;

    public int maxTurnsToSpawn;

    public int turnsToMovePower;

    public Transform ball;

    //SOUND VARIABLES

    public AudioSource m_fireworksSFX;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
          
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        m_CurrentTime += Time.deltaTime;
        switch (m_State)
        {
            case TState.START:
                UpdateStartState();
                break;

            case TState.SELECT:
                UpdateSelectionState();
                break;

            case TState.ATTACK:
                UpdateAttackState();
                break;

            case TState.CHANGETURN:
                UpdateChangeTurnState();
                break;
            case TState.WAIT:
                UpdateWaitState();
                break;
            case TState.RESTART:
                UpdateRestartState();
                break;

            case TState.END:
                UpdateEndState();
                break;
        }

        //AlwaysRed();

        CheckWinCondition();
    }

    //FUNCTIONS

    public void StartGame()
    {
        StartLineRenderer();

        DetermineFirstPlayer();

        AssingBall();

        SetCapBallType();

        //SetCapsTurns();

        IniCounter();

        StartCoroutine(TimeShowingDialog(m_turnText, turnDialog()));
    }

    //START FUNCTIONS REFERRED STARTGAME()

    public void StartLineRenderer()
    {
        line = FindObjectOfType<LineRenderer>();
        line.gameObject.SetActive(false);
    }

    public void DetermineFirstPlayer()
    {
        int randomValue = Random.Range(0, 2);

        switch (randomValue)
        {
            case 0:
                turnPlayerOne = true;
                break;

            case 1:
                turnPlayerTwo = true;
                break;
        }
    }

    public void AssingBall()
    {
        if (turnPlayerOne)
        {
            int randomValue = Random.Range(0, playersTeamPlayer1.Count);

            for (int i = 0; i < playersTeamPlayer1.Count; i++)
            {
                if (playersTeamPlayer1[i] == playersTeamPlayer1[randomValue])
                {
                    ball.SetParent(playersTeamPlayer1[i].transform);
                    ball.localPosition = Vector3.forward * 0.2f;
                    playerWithBall = playersTeamPlayer1[i].transform;
                    return;
                }
            }
        }
        else
        {
            int randomValue = Random.Range(0, playersTeamPlayer2.Count);

            for (int i = 0; i < playersTeamPlayer2.Count; i++)
            {
                if (playersTeamPlayer2[i] == playersTeamPlayer2[randomValue])
                {
                   ball.SetParent(playersTeamPlayer2[i].transform);
                   ball.localPosition = Vector3.forward * 0.2f;
                   playerWithBall = playersTeamPlayer2[i].transform;
                   return;
                }
            }
        }
    }

    public void SetCapBallType()
    {
        if (turnPlayerOne)
        {
            foreach (Caps player in playersTeamPlayer1)
            {
                if (player.transform == playerWithBall.transform)
                {
                    player.SetWithBall(player.transform);
                }
            }
        }
        else
        {
            foreach (Caps player in playersTeamPlayer2)
            {
                if (player.transform == playerWithBall.transform)
                {
                    if (player.transform == playerWithBall)
                    {
                        player.SetWithBall(player.transform);
                    }
                }
            }
        }
    }

    public void IniCounter()
    {
        m_scorePlayerOne.text = "" + m_totalScorePlayerOne;

        m_scorePlayerTwo.text = "" + m_totalScorePlayerTwo;
    }

    //END FUNCTIONS REFERRED STARTGAME()

    //START FUNCTIONS REFERRED UPDATE()

    //SATRT SET

    public void SetStartState()
    {
        m_State = TState.START;
    }

    public void SetSelectionState()
    {
        m_State = TState.SELECT;
    }

    public void SetAttackState()
    {
        m_State = TState.ATTACK;
        m_CurrentTime=0.0f;
    }

    public void SetChangeTurnState()
    {        
        m_State = TState.CHANGETURN;

        ChangeTurn();

        //ChangeColorOneCap(lastObjectSelected);
        if(hasSpawned)
            currentTurnsToMove++;
        currentTurnsToSpawn++;

        if(currentTurnsToSpawn>=maxTurnsToSpawn)
        {
            currentTurnsToSpawn = 0;
            powerSpawner.SpawnPowerUpTransform();
            hasSpawned = true;
        }
        if(currentTurnsToMove>=turnsToMovePower && powerSpawner.GetNumDeployedPowers()>0)
        {
            currentTurnsToMove = 0;
            powerSpawner.MovePowerUp();
        }

        DeadToAlive();

        SetWaitState();
    }

    public void SetWaitState()
    {
        m_State = TState.WAIT;

        m_currentWaitTime = 0f;
    }

    public void SetResatrtState()
    {
        m_State = TState.RESTART;
    }

    public void SetEndState()
    {
        m_State = TState.END;
    }

    //END SET

    //START UPDATE

    public void UpdateStartState()
    {
        SetSelectionState();
    }

    public void UpdateSelectionState()
    {
        SelectCapWithMouse();
    }

    public void UpdateAttackState()
    {        
        /*if(lastObjectSelected.GetComponent<Rigidbody>().velocity == Vector3.zero && m_CurrentTime>=0.5f)
            SetChangeTurnState();*/
    }

    public void UpdateChangeTurnState()
    {

    }

    public void UpdateWaitState()
    {
        m_currentWaitTime += Time.deltaTime;

        //////////

        lastObjectSelected.GetComponent<Caps>().Explote();

        if (lastObjectSelected.GetComponent<Caps>().HasUsedPowerUp())
            lastObjectSelected.GetComponent<Caps>().ResetPowerUp();

        StartCoroutine(TimeShowingDialog(m_turnText, turnDialog()));

        SetSelectionState();

        //////////

        //if ((lastObjectSelected.GetComponent<Rigidbody>().velocity.magnitude <= 0f) && m_currentWaitTime >= m_timeToWaitAfterStop)
        //{
        //    m_timeToWaitAfterStop = 0.25f;
        //    SetSelectionState();

        //    lastObjectSelected.GetComponent<Caps>().Explote();

        //    if (lastObjectSelected.GetComponent<Caps>().HasUsedPowerUp())
        //        lastObjectSelected.GetComponent<Caps>().ResetPowerUp();

        //    lastObjectSelected = null;

        //    StartCoroutine(TimeShowingDialog(m_turnText, turnDialog()));
        //}
    }

    private void UpdateEndState()
    {
        
    }

    private void UpdateRestartState()
    {
       
    }

    
    public void SelectCapWithMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastObjectSelected = null;
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                CanSelectThis(hitInfo);
            }
        }
    }

    public void CanSelectThis(RaycastHit hitInfo)
    {//SI HE MARCADO UN GOL NO PUEDO SELECCIONAR.
        if(!m_score)
        {
            if (turnPlayerOne && hitInfo.transform.gameObject.tag == "CapsPlayerOne" && hitInfo.transform.gameObject.GetComponent<Caps>().checkStatus() != "Dead") //&& hitInfo.transform.gameObject.GetComponent<Caps>().checkStatus() != "Dead"
            {
                OnSelectedCap(hitInfo);
            }
            else if (turnPlayerTwo && hitInfo.transform.gameObject.tag == "CapsPlayerTwo" && hitInfo.transform.gameObject.GetComponent<Caps>().checkStatus() != "Dead")
            {
                OnSelectedCap(hitInfo);
            }
        }
    }

    public void OnSelectedCap(RaycastHit hitInfo)
    {
        /*if (hitInfo.transform.gameObject.GetComponent<Renderer>().material.GetColor("_BaseColor") != Color.green)
        {
            hitInfo.transform.gameObject.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.green);
        }*/
        
        if (lastObjectSelected != null)
        {
            //ChangeColorOneCap(lastObjectSelected); 
        }

        if (hitInfo.transform == lastObjectSelected)
        {
            lastObjectSelected = null;
        }
        else
        {
            lastObjectSelected = hitInfo.transform;
        }
        //Debug.Log(lastObjectSelected);
    }

    //END SELECTION PROCESS


    //START OTHER FUNCTIONS

    /*public void AlwaysRed()
    {
        if (playerWithBall.GetComponent<Renderer>().material.GetColor("_BaseColor") != Color.green)
        {
            playerWithBall.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.red);
        }
    }*/

    /*public void ChangeColorOneCap(Transform cap)
    {
        cap.gameObject.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.white);
    }*/

    //END OTHER FUNCTIONS

    /*public void ResetColor()
    {
        //Vamos a eliminar la chapa 
        foreach(Caps player in playersTeamPlayer1)
        {
            if(player != null)
            {
                player.gameObject.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.white);
            }
        }

        foreach (Caps player in playersTeamPlayer2)
        {
            if (player != null)
            {
                player.gameObject.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.white);
            }
        }
    }*/

    public void ChangeTurn()
    {
        if (turnPlayerOne)
        {
            turnPlayerOne = false;
            turnPlayerTwo = true;
        }
        else
        {
            turnPlayerTwo = false;
            turnPlayerOne = true;
        }
    }

    public void updateScore(bool player1, int score)
    {
        if (player1)
        {
            m_totalScorePlayerOne = m_totalScorePlayerOne + score;
            m_scorePlayerOne.text = "" + m_totalScorePlayerOne;
            ShowDialog(m_goalText, "BLUE PLAYER SCORE");
            m_goalText.color = Color.blue;
        }
        else
        {
            m_totalScorePlayerTwo = m_totalScorePlayerTwo + score;
            m_scorePlayerTwo.text = "" + m_totalScorePlayerTwo;
            ShowDialog(m_goalText, "RED PLAYER SCORE");
            m_goalText.color = Color.red;
        }
        powerSpawner.GetPlayerScored(player1);
        
        RestartGame();
    }

    public void DeadToAlive()
    {
        foreach (Caps player in playersTeamPlayer1)
        {
            if (player.checkStatus() == "Dead")
            {
                player.m_deadToAliveTurn += 1;

                if (player.m_deadToAliveTurn >= 3) //
                {
                    player.SetNone();
                    player.m_deadToAliveTurn = 0;
                    player.transform.GetComponent<MeshRenderer>().material.SetColor("_BaseColor",Color.white); 
                }
            }
        }

        foreach (Caps player in playersTeamPlayer2)
        {
            if (player.checkStatus() == "Dead")
            {
                player.m_deadToAliveTurn += 1;

                if (player.m_deadToAliveTurn >= 3)
                {
                    player.SetNone();
                    player.m_deadToAliveTurn = 0;
                    player.transform.GetComponent<MeshRenderer>().material.SetColor("_BaseColor",Color.white); 
                }
            }
        }
    }

    //START DIALOG FUNCTIONS

    public void ShowDialog(TextMeshProUGUI text, string plainText)
    {
        text.gameObject.SetActive(true);

        text.text = plainText;
    }

    public void DisableDialog(TextMeshProUGUI text)
    {
        text.gameObject.SetActive(false);

        text.color = Color.white;
    }

    public IEnumerator TimeShowingDialog(TextMeshProUGUI text, string plainText)
    {
        ShowDialog(text, plainText);

        yield return new WaitForSeconds(1f);

        DisableDialog(text);
    }

    IEnumerator AfterGoal(float time)
    {
        m_score = true;

        yield return new WaitForSeconds(time);

        m_score = false;

        DisableDialog(m_goalText);        
    }

    public string turnDialog()
    {
        if (turnPlayerOne)
        {
            m_turnText.color = Color.blue;
            return "BLUE PLAYER'S TURN";
        }
        else
        {
            m_turnText.color = Color.red;
            return "RED PLAYER'S TURN";
        }
    }

    //END DIALOG FUNCTIONS

    //START RESTART

    void RestartGame()
    {
        foreach (IRestartGameElement l_RestartGameElement in m_RestartGameElements)
        {
            l_RestartGameElement.RestartGame();
        }
        foreach(Caps current in playersTeamPlayer1)
        {
            current.transform.GetComponent<MeshRenderer>().material.SetColor("_BaseColor",Color.white);
            current.GivePowerUp(PowerUp.IPower.NONE);
        }
        foreach(Caps current in playersTeamPlayer2)
        {
            current.transform.GetComponent<MeshRenderer>().material.SetColor("_BaseColor",Color.white);
            current.GivePowerUp(PowerUp.IPower.NONE);
        }
        OnRestartEnter();
    }

    public void AddRestartGameElement(IRestartGameElement RestartGameElement)
    {
        m_RestartGameElements.Add(RestartGameElement);
    }

    public void DestroyDeathCap(Caps cap)
    {
        if (cap.tag == "CapsPlayerOne")
        {
            m_deathCapsPlayerOne++;
            playersTeamPlayer1.Remove(cap);
        }
        if (cap.tag == "CapsPlayerTwo")
        {
            m_deathCapsPlayerTwo++;
            playersTeamPlayer2.Remove(cap);
        }
        m_RestartGameElements.Remove(cap);
    }

    public void OnRestartEnter()
    {
        //ResetColor();

        //ChangeTurn();

        AssingBall();

        SetCapBallType();

        powerSpawner.RemoveFromField();

        hasSpawned = false;

        m_timeToWaitAfterStop = 6.5f;

        CheckWinCondition();

        m_fireworksSFX.Play();

        if(m_totalScorePlayerOne < 3 || m_totalScorePlayerTwo < 3)
        {
            StartCoroutine(AfterGoal(m_timeToWaitAfterStop));
        }
    }

    //END RESTART

    //WIN FUNCTION

    public void CheckWinCondition()
    {
        if (m_totalScorePlayerOne >= 3 || m_totalScorePlayerTwo >= 3)
        {
            fade = FindObjectOfType<FadeScene>();

            if (once)
            {
                StartCoroutine(WaitTimeToExit());
            }
        }
        if(m_deathCapsPlayerOne >= 3 || m_deathCapsPlayerTwo >= 3)
        {
            fade = FindObjectOfType<FadeScene>();

            if (once)
            {
                StartCoroutine(WaitTimeToExit());
            }
        }
    }

    IEnumerator WaitTimeToExit()
    {
        m_goalText.gameObject.SetActive(false);
        m_WinDialog.gameObject.SetActive(true);

        once = false;

        if (m_totalScorePlayerOne >= 3 || m_deathCapsPlayerOne >= 3)
        {
            m_WinDialog.text = "BLUE PLAYER WIN";
            m_WinDialog.color = Color.blue;
        }
        else if(m_totalScorePlayerTwo >= 3 || m_deathCapsPlayerTwo >= 3)
        {
            m_WinDialog.text = "RED PLAYER WIN";
            m_WinDialog.color = Color.red;
        }
        
        yield return new WaitForSeconds(6f);

        m_WinDialog.gameObject.SetActive(false);

        fade.FadeIn();

        StartCoroutine(LoadSceneAfterFeade());
    }

    IEnumerator LoadSceneAfterFeade()
    {
        yield return new WaitForSeconds(1f);

        scene = FindObjectOfType<SceneController>();
        Destroy(this);
        scene.LoadScene("MainMenu");
    }

    //START PHYSICS

    public Vector3 OrientationToVector(float alpha)
    {
        alpha = alpha * Mathf.Deg2Rad;

        float cos = Mathf.Cos(alpha);
        float sin = Mathf.Sin(alpha);

        return new Vector3(cos, 0, sin);
    }

    public float VectorToOrientation(Vector3 vector)
    {
        Vector3 direction = vector.normalized;

        float sin = direction.z;
        float cos = direction.x;

        float tan = sin / cos;

        float orientation = Mathf.Atan(tan) * Mathf.Rad2Deg;

        if (cos < 0)
        {
            orientation = orientation + 180;
        }

        return orientation;
    }

    //END PHYSICS


}

//END GAME MANAGER
