using UnityEngine;
using System.Collections.Generic;

public class FreezeTurret : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float freezeRange = 5f;
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private float freezeFactor = 0f; // 0 = полная остановка, 0.5 = замедление на 50%
    [SerializeField] private float checkInterval = 0.2f;

    [Header("References")]
    [SerializeField] private Transform rotatingPart;
    [SerializeField] private ParticleSystem freezeEffect;
    [SerializeField] private GameObject rangeView;
    [SerializeField] private LayerMask enemyMask;

    // Для совместимости с UI апгрейда (как в basic_turret)
    [SerializeField] private int damage = 0;
    [SerializeField] private float speedOfSpawn = 0f;

    private List<Enemy> enemiesInRange = new List<Enemy>();
    private float timer;

    // Свойства для доступа извнеолдр
    public float Range { get => freezeRange; set => freezeRange = value; }
    public float FreezeFactor
    {
        get => freezeFactor;
        set => freezeFactor = Mathf.Clamp(value, 0.1f, 1f);
    }
    public int Damage { get => damage; set => damage = value; }
    public float SpeedOfSpawn { get => speedOfSpawn; set => speedOfSpawn = value; }

    private void Start()
    {
        if (rangeView != null)
        {
            UpdateRangeView();
            HideRange();
            if (freezeEffect != null) freezeEffect.Stop();
        }
    }

    private void Update()
    {
        // Вращение части турели
        if (rotatingPart != null)
            rotatingPart.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Периодическая проверка врагов в радиусе
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0f;
            UpdateEnemiesInRange();
        }
    }

    private void UpdateEnemiesInRange()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, freezeRange, enemyMask);
        HashSet<Enemy> currentEnemies = new HashSet<Enemy>();

        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                currentEnemies.Add(enemy);
                if (!enemiesInRange.Contains(enemy))
                    ApplyFreeze(enemy);
            }
        }

        // Удаляем эффект с врагов, покинувших радиус
        List<Enemy> toRemove = new List<Enemy>();
        foreach (var enemy in enemiesInRange)
        {
            if (!currentEnemies.Contains(enemy))
            {
                RemoveFreeze(enemy);
                toRemove.Add(enemy);
            }
        }
        foreach (var enemy in toRemove)
            enemiesInRange.Remove(enemy);

        // Добавляем новых
        foreach (var enemy in currentEnemies)
        {
            if (!enemiesInRange.Contains(enemy))
                enemiesInRange.Add(enemy);
        }

        // Управление частицами
        if (freezeEffect != null)
        {
            freezeEffect.Play();
            //    if (enemiesInRange.Count > 0 )
            //        freezeEffect.Play();
            //    else if (enemiesInRange.Count == 0)
            //        freezeEffect.Stop();
        }
    }

    private void ApplyFreeze(Enemy enemy)
    {
        enemy.ApplyEffectSlowness(freezeFactor); // freezeFactor = 0 для полной остановки
    }

    public void RemoveFreeze(Enemy enemy)
    {
        enemy.RemoveSlowness();
    }

    public void ChangeRange(float range)
    {
        freezeRange = range;
        UpdateRangeView();
    }

    private void UpdateRangeView()
    {
        if (rangeView != null)
        {
            Vector3 scale = rangeView.transform.localScale;
            scale.x = freezeRange / 5f;
            scale.y = freezeRange / 5f;
            rangeView.transform.localScale = scale;
        }
    }

    public void ShowRange()
    {
        if (rangeView != null)
            rangeView.SetActive(true);
    }

    public void HideRange()
    {
        if (rangeView != null)
            rangeView.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, freezeRange);
    }

    public void OnDestroyThis()
    {
        foreach (var enemy in enemiesInRange)
        {
            RemoveFreeze(enemy);
        }

        Destroy(gameObject);

    }
}