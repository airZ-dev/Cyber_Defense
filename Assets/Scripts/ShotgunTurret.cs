using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static basic_turret;

public class ShotgunTurret : MonoBehaviour
{


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

    private Transform target;
    private float timeUntilFire;
    private TargetStrategy targetStrategy;
    public bool isChanged;

    public float Range { get { return targetRange; } set { targetRange = value; } }
    public int CounrOfBullets { get { return pelletCount; } set { pelletCount = value; } }
    public int Damage { get { return bulletDamage; } set { bulletDamage = value; } }

    public float Spread { get { return spreadAngle; } set { spreadAngle = value; } }

    public float SpeedOfSpawn { get => bps; set => bps = value; }
    // ѕереопредел€ем выстрел
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
        if (isChanged)
        {
            isChanged = false;
            target = null;
            return;
        }

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

        List<Transform> enemies = hits.Select(h => h.transform).OrderBy(x => Vector3.Distance(x.position, LevelManager.instance.path[^1].position)).ToList();

        targetStrategy = gameObject.GetComponent<Tower>().Strategy;
        switch (targetStrategy)
        {
            case TargetStrategy.Closest:
                target = enemies.OrderBy(e => Vector2.Distance(e.position, transform.position)).FirstOrDefault();
                break;

            case TargetStrategy.First:

                target = enemies[0];
                break;

            case TargetStrategy.Last:
                target = enemies[^1];
                break;
        }

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
        // ќпредел€ем базовое направление на цель (если цель есть)
        Vector2 baseDirection = Vector2.right; // по умолчанию вправо
        if (target != null)
        {
            baseDirection = (target.position - firingPoint.position).normalized;
        }
        else
        {
            // ≈сли цели нет, стрел€ем в сторону, куда смотрит турель
            baseDirection = turretRotationPoint.up;
        }

        // —оздаЄм несколько пуль с разбросом
        for (int i = 0; i < pelletCount; i++)
        {
            // √енерируем случайное отклонение в пределах spreadAngle
            float angleOffset = Random.Range(-spreadAngle / 2f, spreadAngle / 2f);
            Vector2 shootDir = Quaternion.Euler(0, 0, angleOffset) * baseDirection;

            GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
            ShotgunBullet bulletScript = bulletObj.GetComponent<ShotgunBullet>();
            if (bulletScript != null)
            {
                bulletScript.Initialize(shootDir);
            }
        }
        AudioManager.Instance?.PlayShootShootgn();
    }

    // ќстальна€ логика (Update, FindTarget, RotateToTarget) наследуетс€ от basic_turret.
    // ≈сли нужно отключить точное наведение пуль Ц это уже сделано.
}
