using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestManager : MonoBehaviour
{

    public static bool transitioned;
    public GameObject buttonCotainer;
    [SerializeField]private Animator transitionAnim;
    [SerializeField]private AnimationClip start, end;
    [SerializeField]private int sceneNum;

    [Header("Pause Menu")]
    [SerializeField]private GameObject pauseMenu, exitBall;
    private bool isPaused, exit, restarting, buttonConfig;
    [SerializeField]private AnimationClip mainMenuTransition;
    
    // Start is called before the first frame update
    
    private void Start() {
        buttonCotainer = GameObject.Find("ContentArea");
        exitBall.SetActive(false);
        buttonCotainer.SetActive(false);
    }

    private void Update() {


        if (Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;
        }

        if (isPaused)
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }else
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }

        if (exit)
        {
            StartCoroutine(ExitTransition());
        }

        if (restarting)
        {
            StartCoroutine(Transition(SceneManager.GetActiveScene().buildIndex));
            restarting = false;
        }

        Debug.Log(isPaused);

    }

    private void OnTriggerEnter2D(Collider2D other) {
        try
        {
            StartCoroutine(Transition(sceneNum));
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    private IEnumerator Transition(int scene){
        transitionAnim.SetTrigger("Transition");
        transitioned = true;
        yield return new WaitForSeconds(start.length);
        SceneManager.LoadScene(scene);
        transitioned = false;
    }

    private IEnumerator ExitTransition(){
        exit = false;
        exitBall.SetActive(true);
        transitioned = true;
        yield return new WaitForSeconds(mainMenuTransition.length);
        SceneManager.LoadScene(0);
        transitioned = false;
    }

    public void Exit(){
        isPaused = false;
        if (!exit)
        {
            exit = true;
        }
    }

    public void Restart(){
        isPaused = false;
        if(!restarting){
            restarting = true;
        }
    }

    public void ButtonConfig() 
    {
        buttonConfig = false;
		if (!buttonConfig)
		{
            buttonCotainer.SetActive(true);
		}
    }

    public void ButtonExit() 
    {
        buttonConfig = true;
		if (buttonConfig)
		{
            buttonCotainer.SetActive(false);
		}
    }
    
    
    
}
