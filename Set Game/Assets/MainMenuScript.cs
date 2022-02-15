using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class MainMenuScript : MonoBehaviour
{
    public float endScore;
    public TextMeshProUGUI scoreDisplay;
    
    private List<int> scorePartitions;

    public void StartSpeedGame(){
        SceneManager.LoadScene(3);
    }
    public void StartClassicGame(){
        SceneManager.LoadScene(1);
    }
    public void ReturnToMenu(){
        SceneManager.LoadScene(0);
    }
    public void QuitGame(){
        Application.Quit();
    }

    void DisplayScore(){
        VertexGradient level1 = new VertexGradient(Color.magenta, Color.magenta, Color.blue, Color.blue);
        VertexGradient level2 = new VertexGradient(Color.white, Color.white, Color.blue, Color.blue);
        VertexGradient level3 = new VertexGradient(Color.cyan, Color.cyan, Color.blue, Color.blue);
        VertexGradient level4 = new VertexGradient(Color.yellow, Color.yellow, Color.blue, Color.blue);
        VertexGradient level5 = new VertexGradient(Color.red, Color.red, Color.blue, Color.blue);

        endScore = PlayerData.time;
        if (endScore < scorePartitions[0]){
            scoreDisplay.colorGradient = level5;
        }
        else if (endScore < scorePartitions[1]){
            scoreDisplay.colorGradient = level4;
        }
        else if (endScore < scorePartitions[2]){
            scoreDisplay.colorGradient = level3;
        }
        else if (endScore < scorePartitions[3]){
            scoreDisplay.colorGradient = level2;
        }
        else{
            scoreDisplay.colorGradient = level1;
        }
        scoreDisplay.text = endScore + "s";
    }

    void Start(){
        if (SceneManager.GetActiveScene().buildIndex == 2){
            scorePartitions = new List<int>{250, 300, 350, 400, 450};
        }
        else if (SceneManager.GetActiveScene().buildIndex == 4){
            scorePartitions = new List<int>{10, 30, 45, 60, 100};
        }
        else{
            scorePartitions = new List<int>{0, 0, 0, 0, 0};
        }
        DisplayScore();
    }
}
