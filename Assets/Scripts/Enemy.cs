using System.Collections;
using System.Collections.Specialized;
using System.Resources;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;


public class Enemy : MonoBehaviour
{
    [Header("Характеристики врага")]
    public int damage = 10;
    public float moveSpeed = 2f;

    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private Transform target;
    private Rigidbody2D rb;
    private bool isMoving = true;
    private float baseSpeed;
    private bool isSlowed = false;

    private void Start()
    {
        waypoints = LevelManager.instance.path;
        currentWaypointIndex = 0;
        isMoving = true;
        if (waypoints != null && waypoints.Length > 0)
        {
            target = waypoints[currentWaypointIndex];
        }
        rb = GetComponent<Rigidbody2D>();
        baseSpeed = moveSpeed;
    }

    private void Update()
    {
        if (!isMoving || waypoints == null || waypoints.Length == 0) return;

        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        if (Vector2.Distance(target.position, transform.position) <= 0.1f)
        {
            ++currentWaypointIndex;
        }
        if (currentWaypointIndex >= waypoints.Length)
        {
            ReachedEnd();
            return;
        }
        target = waypoints[currentWaypointIndex];
    }

    private void ReachedEnd()
    {
        isMoving = false;
        LevelManager.instance.takeDamage(damage);
        StartCoroutine(ScaleEnemyOnDespawn(transform));

    }
    private void FixedUpdate()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

    public void ApplyEffectSlowness(float slowFactor)
    {
        if (isSlowed) return;
        moveSpeed *= slowFactor;

        isSlowed = true;
    }
    public void RemoveSlowness()
    {
        if (!isSlowed) return;
        moveSpeed = baseSpeed;
        isSlowed = false;
    }
    private IEnumerator ScaleEnemyOnDespawn(Transform enemy_transform)
    {
        float elapsedTime = 0f;
        Vector3 originalScale = enemy_transform.localScale;
        Vector3 targetScale = Vector3.zero;
        float despawnScaleDuration = WaveManager.instance.despawnScaleDuration;
        while (elapsedTime < despawnScaleDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / despawnScaleDuration;
            enemy_transform.localScale = Vector3.Lerp(originalScale, targetScale, AnimationCurve.EaseInOut(0, 0, 1, 1).Evaluate(progress));
            yield return null;
        }

        enemy_transform.localScale = targetScale;
       WaveManager.onEnemyDestroy?.Invoke();
        Destroy(gameObject);
    }
}

