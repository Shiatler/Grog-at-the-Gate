using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public float startSpeed = 10f;
    public float startHealth = 100;
    public int worth = 50;

    [HideInInspector]
    public float speed;
    private float health;
    private bool isDead = false;

    public GameObject deathEffect;

    [Header("Unity Stuff")]
    public Image healthBar;

    private Coroutine burnCoroutine;

    // START #########################################################

    void Start()
    {
        speed = startSpeed;
        health = startHealth;
    }

    // TAKE DAMAGE #########################################################
    public void TakeDamage(float amount)
    {
        if (isDead) return;
        
        health -= amount;
        healthBar.fillAmount = health / startHealth;
        
        if (health <= 0)
        {
            Die();
        }
    }

    // SLOW #########################################################
    public void Slow(float pct)
    {
        speed = startSpeed * (1f - pct);
    }

    // APPLY BURN #########################################################
    public void ApplyBurn(float burnDamagePerSecond, float burnDuration)
    {
        // Stop existing burn if one is active
        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);
        }
        
        // Start new burn effect
        burnCoroutine = StartCoroutine(BurnOverTime(burnDamagePerSecond, burnDuration));
    }

    // BURN OVER TIME #########################################################
    IEnumerator BurnOverTime(float burnDamagePerSecond, float burnDuration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < burnDuration && !isDead)
        {
            // Apply burn damage continuously using deltaTime (like laser)
            TakeDamage(burnDamagePerSecond * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        burnCoroutine = null;
    }

    // DIE #########################################################

    void Die()
    {
        if (isDead) return;
        isDead = true;

        PlayerStats.Money += worth;

        GameObject effect = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 5f);

        WaveSpawner.enemiesAlive--;
        
        Destroy(gameObject);
    }
}
