using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int maxHP= 2;
    [SerializeField] private int currancyWorth = 10;
    [SerializeField] private Image _healthbar;
     private int currentHP;

    private bool isDestroyed = false;
    private void Start()
    {
        currentHP = maxHP;
    }
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0 && !isDestroyed)
        {
            WaveManager.onEnemyDestroy.Invoke();
            LevelManager.instance.IncreaseCurrency(currancyWorth);
            isDestroyed = true;
            _healthbar.fillAmount = 0;
            StartCoroutine(DeathAnimation());
        }
        else
        { 
            _healthbar.fillAmount = currentHP*1.0f / maxHP;
        }
    }
    private IEnumerator DeathAnimation()
    {
        // Можно добавить эффекты смерти
        float deathDuration = 0.5f;
        float elapsedTime = 0f;
        Vector3 originalScale = transform.localScale;

        while (elapsedTime < deathDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / deathDuration;

            // Вращение и масштабирование при смерти
            transform.localScale = originalScale * (1f - progress);
            transform.Rotate(0, 0, 360 * Time.deltaTime);

            yield return null;
        }
        Destroy(gameObject);
    }
}
