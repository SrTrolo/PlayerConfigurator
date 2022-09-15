using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{
    public List<GameObject> allsounds;
    public GameObject Intro, Outro, Loading, exit;
    public float blockCount, outroCount, loadingCount;
    public bool counter;
    // Start is called before the first frame update
    void Start()
    {
        counter = false;
        Outro.SetActive(false);
        Loading.SetActive(false);
        exit.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        AnimControl();
        ExitGame();
    }
    public void OutroBTN()
    {
        GameObject newSound = Instantiate(allsounds[0]);
        Destroy(newSound, 1);
        counter = true;
    }
    void AnimControl()
    {
        blockCount += Time.deltaTime;
        if (blockCount >= 6 || Input.GetKey(KeyCode.Return))
        {
            blockCount = 0;
            Intro.SetActive(false);
        }
        if (counter == true)
        {
            Outro.SetActive(true);

            outroCount += Time.deltaTime;
            if (outroCount >= 3)
            {
                Loading.SetActive(true);
                loadingCount += Time.deltaTime;
                if (loadingCount >= 3)
                {
                    SceneManager.LoadScene(1);
                }

            }
        }
    }
    void ExitGame()
    {
        if(Input.GetKey(KeyCode.Escape))
        {

            exit.SetActive(true);
        }
    }
    public void CloseExit()
    {
        GameObject newSound = Instantiate(allsounds[0]);
        Destroy(newSound, 1);
        exit.SetActive(false);
    }
    public void CloseGame()
    {
        GameObject newSound = Instantiate(allsounds[0]);
        Destroy(newSound, 1);
        Application.Quit();
    }
    
}
