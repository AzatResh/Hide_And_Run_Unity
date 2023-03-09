using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject Gamelose;
    public GameObject GameWin;
    bool gameisOver;
    // Start is called before the first frame update
    void Start()
    {
        Guard.GuardHasSpottedPlayer+=Lose;
        FindObjectOfType<Player>().OnEndOfLevel+=Win;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameisOver){
            if(Input.GetKeyDown(KeyCode.Space)){
                SceneManager.LoadScene(0);
                
            }
        }
    }

    void Win(){
       GameOver(GameWin);
    }

    void Lose(){
        GameOver(Gamelose);
    }

    void GameOver(GameObject gameOverUI){
        gameOverUI.SetActive(true);
        gameisOver=true;
        Guard.GuardHasSpottedPlayer-=Lose;
        FindObjectOfType<Player>().OnEndOfLevel-=Win;
    }
}
