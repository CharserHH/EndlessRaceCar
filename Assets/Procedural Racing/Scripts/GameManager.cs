using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text scoreLabel;
    public Text timerLabel;
    public Car car;
    public Animator gameOverAnimator;
    public Text gameOverScoreLabel;
    public Text bestScoreLabel;
    public AudioSource bgm;
    public AudioSource gameOverSound;
    private float time;
    private int score;
    private bool gameOver;
    private AudioSource scoreAudio;
    // Start is called before the first frame update
    void Start()
    {
        scoreAudio = GetComponent<AudioSource>();
        UpdateScore(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameOver){
            UpdateTimer();
        } else {
            if (Input.GetKeyDown(KeyCode.Return)){
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        }
    }
    void UpdateTimer(){
        time += Time.deltaTime;
        int timer = (int) time;

        int second = timer % 60;
        int minute = timer / 60;

        string timeString = minute.ToString("00") + ":" + second.ToString("00");

        timerLabel.text = "Time: " + timeString;
    }
    public void UpdateScore(int point){
        if (point > 0){
            scoreAudio.Play();
        }
        score += point;
        scoreLabel.text = "Score: " + score;
    }
    public void GameOver(){
        if (gameOver){
            return;
        }
        SetScore();
        car.FallApart();
        gameOver = true;

        bgm.Stop();
        gameOverSound.Play();

        gameOverAnimator.SetTrigger("Game Over");

        foreach (BasicMovement movement in GameObject.FindObjectsOfType<BasicMovement>()){
            movement.moveSpeed = 0;
            movement.rotationSpeed = 0;
        }
    }

    void SetScore(){
        if (score > PlayerPrefs.GetInt("BestScore")){
            PlayerPrefs.SetInt("BestScore", score);
        }

        gameOverScoreLabel.text = "Score: " + score;
        bestScoreLabel.text = "Best: " + PlayerPrefs.GetInt("BestScore");
    }
}
