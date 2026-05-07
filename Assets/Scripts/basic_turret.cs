using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using static basic_turret;
using static ShotgunTurret;


public class basic_turret : MonoBehaviour
{

    [Header("Attributes")]
    [SerializeField] protected float targetRange = 5f;
    [SerializeField] protected float angleSpeed = 1.0f;
    [SerializeField] protected float bps = 1f; // пулей в секунду

    [Header("References")]
    [SerializeField] protected Transform turretRotationPoint;
    [SerializeField] protected LayerMask enemyMask;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform firingPoint;
    [SerializeField] protected GameObject rangeView;

    public float Range { get { return targetRange; } set { targetRange = value; } }
    public float SpeedOfSpawn { get { return bps; } set { bps = value; } }
    public int Damage { get { return bulletDamage; } set { bulletDamage = value; } }

    protected int bulletDamage;
    protected Transform target;

    private float timeUntilFire;
    private TargetStrategy targetStrategy;
    public bool isChanged;

    protected void Awake()
    {
        ChangeRange(targetRange);
        bulletDamage = bulletPrefab.GetComponent<bullet>().Damage;
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

    protected void Shoot()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        bullet bulletScript = bulletObj.GetComponent<bullet>();
        bulletScript.SetTarget(target);
        bulletScript.Damage = bulletDamage;
        AudioManager.Instance?.PlayShootBase();
    }

    private void FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
        transform.position, targetRange, (Vector2)transform.position, 0f, enemyMask);

        if (hits.Length == 0)
        {
            target = null;
            return;
        }

        List<Transform> enemies = hits.Select(h => h.transform).ToList();
        targetStrategy = GetComponent<Tower>().Strategy;



        switch (targetStrategy)
        {
            case TargetStrategy.Closest:
                target = enemies.OrderBy(e => Vector2.Distance(e.position, transform.position)).FirstOrDefault();
                break;

            case TargetStrategy.First:
                target = target = enemies
                    .OrderByDescending(e => e.GetComponent<Enemy>().Progress)
                    .FirstOrDefault();
                break;
            case TargetStrategy.Last:
                target = target = enemies
                .OrderBy(e => e.GetComponent<Enemy>().Progress)
                .FirstOrDefault();
                break;
        }
    }


    private bool CheckTargetInRange()
    {
        return target != null && Vector2.Distance(target.position, transform.position) <= targetRange;
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

}
