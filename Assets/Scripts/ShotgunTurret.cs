using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static basic_turret;

public class ShotgunTurret : MonoBehaviour
{
    public enum TargetStrategy
    {
        Closest,   // ближайший по расстоянию
        First,     // самый старый (наибольший прогресс по пути)
        Last       // самый новый (наименьший прогресс)
    }


    [Header("Shotgun Settings")]
    [SerializeField] private int pelletCount = 5;      // количество пуль за выстрел
    [SerializeField] private float spreadAngle = 30f;  // полный угол разброса (в градусах)
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject rangeView;                              //[SerializeField] private float pelletSpeed = 8f;
    [SerializeField] private float targetRange = 5f;
    [SerializeField] private float angleSpeed = 1.0f;
    [SerializeField] private float bps = 1f; // пулей в секунду
    [SerializeField] private int bulletDamage = 15;
    [SerializeField] private TargetStrategy targetStrategy = TargetStrategy.Closest;
    private Transform target;
    private float timeUntilFire;

    public float Range { get { return targetRange; } set { targetRange = value; } }
    public int CounrOfBullets { get { return pelletCount; } set { pelletCount = value; } }
    public int Damage { get { return bulletDamage; } set { bulletDamage = value; } }

    public float Spread { get { return spreadAngle; } set { spreadAngle = value; } }


    // Переопределяем выстрел
    private void Awake()
    {
        ChangeRange(targetRange);
        bulletPrefab.GetComponent<ShotgunBullet>().Damage = bulletDamage;
        HideRange();
    }

    public void ChangeRange(float range)
    {
        targetRange = range;
        Vector3 tr = rangeView.GetComponent<Transform>().localScale;
        tr = new Vector3(targetRange / 5, targetRange / 5, 1);
        rangeView.GetComponent<Transform>().localScale = tr;
    }

    public void ShowRange()
    {
        rangeView.SetActive(true);
    }

    public void HideRange()
    {
        rangeView.SetActive(false);
    }

    private void Update()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }
        //Debug.Log(CheckTargetForward());
        RotateToTarget();
        if (!CheckTargetInRange())
        {
            target = null;
        }
        else
        {
            timeUntilFire += Time.deltaTime;
            if (timeUntilFire >= 1f / bps)
            {
                Shoot();
                timeUntilFire = 0f;
            }
        }


    }
    private void FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetRange, (Vector2)transform.position, 0f, enemyMask);

        if (hits.Length == 0)
        {
            target = null;
            return;
        }

        List<Transform> enemies = hits.Select(h => h.transform).ToList();

        switch (targetStrategy)
        {
            case TargetStrategy.Closest:
                target = enemies.OrderBy(e => Vector2.Distance(e.position, transform.position)).FirstOrDefault();
                break;

            case TargetStrategy.First:
                target = GetByProgress(enemies, first: true);
                break;

            case TargetStrategy.Last:
                target = GetByProgress(enemies, first: false);
                break;
        }
    }

    private Transform GetByProgress(List<Transform> enemies, bool first)
    {
        // Пытаемся получить компонент Enemy (или любой с полем progress)
        var withProgress = enemies.Select(e => new { transform = e, progress = GetProgress(e) })
                                  .Where(x => x.progress.HasValue)
                                  .ToList();

        if (withProgress.Count == 0)
        {
            // Fallback на ближайшего, если прогресс не найден
            Debug.LogWarning($"{name}: нет врагов с Progress, стратегия First/Last заменена на Closest");
            return enemies.OrderBy(e => Vector2.Distance(e.position, transform.position)).FirstOrDefault();
        }

        if (first)
            return withProgress.OrderByDescending(x => x.progress.Value).First().transform;
        else
            return withProgress.OrderBy(x => x.progress.Value).First().transform;
    }

    private float? GetProgress(Transform enemy)
    {
        // Предполагаем, что у врага есть скрипт с полем progress
        // Замените "Enemy" на имя вашего класса врага
        var enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
            return enemyScript.Progress;

        // Альтернатива: поиск поля через рефлексию (медленно, но универсально)
        var component = enemy.GetComponent<MonoBehaviour>();
        if (component != null)
        {
            var field = component.GetType().GetField("progress");
            if (field != null)
                return (float)field.GetValue(component);
        }
        return null;
    }


    private bool CheckTargetInRange()
    {
        return Vector2.Distance(target.position, transform.position) <= targetRange;
    }

    private void RotateToTarget()
    {
        float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg - 90.0f;
        Quaternion targetzRot = Quaternion.Euler(new Vector3(0f, 0f, angle));
        turretRotationPoint.rotation = Quaternion.RotateTowards(turretRotationPoint.rotation, targetzRot, angleSpeed * Time.deltaTime);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position;
        int segments = 36;

        for (int i = 0; i < segments; i++)
        {
            float startAngle = (i / (float)segments) * 360f * Mathf.Deg2Rad;
            float endAngle = ((i + 1) / (float)segments) * 360f * Mathf.Deg2Rad;

            Vector3 start = center + new Vector3(Mathf.Cos(startAngle), Mathf.Sin(startAngle), 0) * targetRange;
            Vector3 end = center + new Vector3(Mathf.Cos(endAngle), Mathf.Sin(endAngle), 0) * targetRange;

            Gizmos.DrawLine(start, end);
        }
    }

    private void Shoot()
    {
        // Определяем базовое направление на цель (если цель есть)
        Vector2 baseDirection = Vector2.right; // по умолчанию вправо
        if (target != null)
        {
            baseDirection = (target.position - firingPoint.position).normalized;
        }
        else
        {
            // Если цели нет, стреляем в сторону, куда смотрит турель
            baseDirection = turretRotationPoint.up;
        }

        // Создаём несколько пуль с разбросом
        for (int i = 0; i < pelletCount; i++)
        {
            // Генерируем случайное отклонение в пределах spreadAngle
            float angleOffset = Random.Range(-spreadAngle / 2f, spreadAngle / 2f);
            Vector2 shootDir = Quaternion.Euler(0, 0, angleOffset) * baseDirection;

            GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
            ShotgunBullet bulletScript = bulletObj.GetComponent<ShotgunBullet>();
            if (bulletScript != null)
            {
                bulletScript.Initialize(shootDir);
            }
            else
            {
                // fallback: если префаб не содержит ShotgunBullet, используем стандартную пулю (не рекомендуется)
                Debug.LogWarning("ShotgunTurret: bullet prefab missing ShotgunBullet component!");
            }
        }
    }

    // Остальная логика (Update, FindTarget, RotateToTarget) наследуется от basic_turret.
    // Если нужно отключить точное наведение пуль – это уже сделано.
}
