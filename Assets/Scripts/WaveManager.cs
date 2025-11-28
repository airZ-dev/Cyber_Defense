using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


public class WaveManager : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] private int[] enemyCount;
    [SerializeField] private GameObject[] enemyTypeWaves;

    [Header("Характеристики")]
    [SerializeField] private float enemiesPerSecond = 0.5f;
    [SerializeField] private float timeBtwWave = 5f;
    [SerializeField] private float spawnScaleDuration = 1.0f;
    [SerializeField] private float _despawnScaleDuration = 1.0f;

    [Header("Ивенты")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();
    public static UnityEvent<int> onEnemyReachedEnd = new UnityEvent<int>();

    [Header("Ссылки")]
    [SerializeField] private TextMeshProUGUI txt;

    private int currentWave = 1;
    private float timeSinceLastSpawn;
    public static WaveManager instance;
    public float despawnScaleDuration { get { return _despawnScaleDuration; } private set { } }
    private int enemiesAlive;
    private int enemiesLeftToSpawn;
    private bool isSpawning = false;
    private bool firstWave = true;
    private float currentTimebtwWaves;


    private void Awake()
    {
        instance = this;
        currentTimebtwWaves = timeBtwWave;
        onEnemyDestroy.AddListener(EnemyDestroed);
    }
    private void OnGUI()
    {
        txt.text = "текущая волна: 0/" + enemyTypeWaves.Length + ".\nВрагов: " + enemiesAlive +
        ".\nДо следующей волны " + (firstWave ? 0 : Mathf.RoundToInt(currentTimebtwWaves))  + "сек.";
    }

    private void Update()
    {
        if (!firstWave)
        {
            if (!isSpawning)
            {
                currentTimebtwWaves -= Time.deltaTime;
                return;
            }

            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn >= (1f / enemiesPerSecond) && enemiesLeftToSpawn > 0)
            {
                SpawnEnemy();
                --enemiesLeftToSpawn;
                ++enemiesAlive;
                timeSinceLastSpawn = 0f;
            }

            if (enemiesAlive == 0 && enemiesLeftToSpawn == 0)
            {
                EndWave();
            }
        }

    }
    public void StartButtonAction()
    {
        if (firstWave)
        {
            firstWave = false;
        }
        if (isSpawning)
        {
            return;
        }
        StartCoroutine(StartWave());
    }
    private void EnemyDestroed()
    {
        --enemiesAlive;
    }
    private void EndWave()
    {
        isSpawning = false;
        timeSinceLastSpawn = 0f;
        ++currentWave;
        currentTimebtwWaves = timeBtwWave;
        StartCoroutine(StartWave());
    }

    private void SpawnEnemy()
    {
        GameObject prefabToSpawn = enemyTypeWaves[currentWave-1];
        GameObject enemy = Instantiate(prefabToSpawn, LevelManager.instance.startPoint.position, Quaternion.identity);
        StartCoroutine(ScaleEnemyOnSpawn(enemy.transform));
    }

    private IEnumerator ScaleEnemyOnSpawn(Transform enemy_transform)
    {
        float elapsedTime = 0f;
        Vector3 originalScale = enemy_transform.localScale;
        Vector3 startScale = Vector3.zero;

        enemy_transform.localScale = startScale;

        while (elapsedTime < spawnScaleDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / spawnScaleDuration;
            enemy_transform.localScale = Vector3.Lerp(startScale, originalScale, AnimationCurve.EaseInOut(0, 0, 1, 1).Evaluate(progress));
            yield return null;
        }

        enemy_transform.localScale = originalScale;
    }

    private IEnumerator StartWave()
    {
        yield return new WaitForSeconds(timeBtwWave);
        isSpawning = true;
        if (currentWave-1 == enemyCount.Length)
        {
            deathMenu.instance.winOrLoseWindowShow(true);
            yield break;
        }
        enemiesLeftToSpawn = enemyCount[currentWave - 1];
        
    }

}
