using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.ShaderGraph.Internal;
using System;


public class basic_turret : MonoBehaviour
{


    [Header("Attributes")]
    [SerializeField] private float targetRange = 5f;
    [SerializeField] private float angleSpeed = 1.0f;
    [SerializeField] private float bps = 1f; // пулей в секунду

    [Header("References")]
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject rangeView;

    private Transform target;
    private float timeUntilFire;
    private void Awake()
    {
        ChangeRange(targetRange);
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

    private void Shoot()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        bullet bulletScript = bulletObj.GetComponent<bullet>();
        bulletScript.SetTarget(target);

    }

    private void FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetRange, (Vector2)transform.position, 0f, enemyMask);

        if (hits.Length > 0)
        {
            target = hits[0].transform;
        }

    }




    private bool CheckTargetInRange()
    {
        return Vector2.Distance(target.position, transform.position) <= targetRange;
    }
    private bool CheckTargetForward()
    {
        float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg - 90.0f;
        return angle <= target.rotation.z || angle > target.rotation.z;
    }

    private void RotateToTarget()
    {
        float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg - 90.0f;
        Quaternion targetzRot = Quaternion.Euler(new Vector3(0f, 0f, angle));
        turretRotationPoint.rotation = Quaternion.RotateTowards(turretRotationPoint.rotation, targetzRot, angleSpeed * Time.deltaTime);

    }
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, transform.forward, targetRange);
    }

}
