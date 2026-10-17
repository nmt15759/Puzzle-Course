using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text scoreText;
    public GameObject RoundOverScreen;
    public TMP_Text winScore;
    public TMP_Text winText;
    public GameObject winStars1, winStars2, winStars3;
    private Board theBoard;
    public string LevelSelect;
    public GameObject pauseScreen;
    public TMP_Text objectiveText;
    public TMP_Text movesText;
    public TMP_Text comboText;
    private Coroutine comboRoutine;


    private void Awake()
    {
        theBoard = Object.FindAnyObjectByType<Board>();
    }
    void Start()
    {
        winStars1.SetActive(false);
        winStars2.SetActive(false);
        winStars3.SetActive(false);

        if (comboText != null)
        {
            comboText.gameObject.SetActive(false);
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpause();
        }
    }
    public void PauseUnpause()
    {
        if (!pauseScreen.activeInHierarchy)
        {
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            pauseScreen.SetActive(false);
            Time.timeScale = 1f;
        }
    }
    public void ShuffleBoard()
    {
        theBoard.ShuffleBoard();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void GoToLevelSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(LevelSelect);
    }
    public void TryAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

 
    public void ShowCombo(int combo)
    {
        if (comboText == null)
        {
            return;
        }

        comboText.text = "Combo x" + combo + "!";

        if (comboRoutine != null)
        {
            StopCoroutine(comboRoutine);
        }
        comboRoutine = StartCoroutine(ComboPop());
    }

    private IEnumerator ComboPop()
    {
        comboText.gameObject.SetActive(true);
        Transform t = comboText.transform;

        float time = 0f;
        float popDuration = 0.25f;
        while (time < popDuration)
        {
            time += Time.deltaTime;
            float s = Mathf.Lerp(1.6f, 1f, time / popDuration);
            t.localScale = new Vector3(s, s, 1f);
            yield return null;
        }
        t.localScale = Vector3.one;

        yield return new WaitForSeconds(1f);

        comboText.gameObject.SetActive(false);
        comboRoutine = null;
    }
}