using System;
using TMPro;
using UnityEngine;
using System.Collections;

public class GameloopProgression : MonoBehaviour
{
    public GameObject enemySpawn;
    public GameObject playerSpawn;
    public SpriteRenderer background;

    [SerializeField] private Enemy[] enemies;
    [SerializeField] private Sprite[] backgrounds;
    [SerializeField] private UINavigation navigation;
    [SerializeField] private GameUI gameui;
    [SerializeField] private float totalTime;

    private Enemy currentEnemy;
    private int index = 0;
    private float currentTime;
    private bool timerRunning;

    public void ResetGame()
    {
        Time.timeScale = 1;
        navigation.ToSelectScene();
    }
    void Start()
    {
        SpawnEnemy();
        StartTimer();
    }

    private void Update()
    {
        if (!timerRunning) return;

        currentTime -= Time.deltaTime;
        gameui.SetTimerText(currentTime);

        if (currentTime <= 0f)
        {
            timerRunning = false;
            gameui.SetResultsText("You lost!");
            gameui.ToggleResultsMenu();
        }
    }

    private void OnDisable()
    {
        UnsubscribeCurrent();
    }
    
    private void SpawnEnemy()
    {
        if (index >= enemies.Length)
        {
            timerRunning = false;
            gameui.SetResultsText("You won!");
            StartCoroutine(ShowResultsAfterDelay());
            return;
        }
        
        UnsubscribeCurrent();
        currentEnemy = Instantiate(enemies[index], enemySpawn.transform.position, Quaternion.identity);
        
        if (currentEnemy != null && currentEnemy.isDefeated != null)
            currentEnemy.isDefeated.ChangeEvent += OnEnemyDefeated;
        
        if (index < backgrounds.Length)
            switch (index)
            {
                case 0: background.sprite = backgrounds[index];
                    background.transform.position = new Vector3(0, 2, 1);
                    background.transform.localScale = new Vector3(9, 6, 2);
                    break;
                case 1: background.sprite = backgrounds[index];
                    background.transform.position = new Vector3(0, 5, 1);
                    background.transform.localScale = new Vector3(7, 5, 1);
                    break;
                case 2: background.sprite = backgrounds[index];
                    background.transform.position = new Vector3(0, 2, 1);
                    break;
                case 3: background.sprite = backgrounds[index];
                    background.transform.position = new Vector3(0, 3, 1);
                    break;
                case 4: background.sprite = backgrounds[index];
                    background.transform.position = new Vector3(0, 6, 1);
                    break;
            }

        index++;
    }

    private void OnEnemyDefeated(bool defeated)
    {
        if (!defeated) return;
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            player.transform.position = playerSpawn.transform.position;
        
        SpawnEnemy();
    }

    private void UnsubscribeCurrent()
    {
        if (currentEnemy != null && currentEnemy.isDefeated != null)
            currentEnemy.isDefeated.ChangeEvent -= OnEnemyDefeated;
    }

    private void StartTimer()
    {
        currentTime = totalTime;
        timerRunning = true;
        gameui.SetTimerText(currentTime);
    }
    
    private IEnumerator ShowResultsAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        gameui.ToggleResultsMenu();
    }
}