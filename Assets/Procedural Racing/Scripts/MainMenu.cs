using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || 
            (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()))
        {
            if (Input.touchCount > 0 && 
                Input.GetTouch(0).phase == TouchPhase.Began && 
                EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)){
                return;
            }
            StartGame();
        }
    }

    public void StartGame(){
        StartCoroutine(LoadSceneMode("Game"));
    }

    IEnumerator LoadSceneMode(string scene){
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(scene);
    }
}
