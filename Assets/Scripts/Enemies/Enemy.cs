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
    private Coroutine stunCoroutine;

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

    // APPLY STUN #########################################################
    public void ApplyStun(float stunDuration, float slowAmountStun)
    {
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }

        stunCoroutine = StartCoroutine(StunOverTime(stunDuration, slowAmountStun));
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

    // STUN OVER TIME #########################################################
    IEnumerator StunOverTime(float stunDuration, float slowAmountStun)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < stunDuration && !isDead)
        {
            // Continuously apply slow effect to prevent it from being overridden
            Slow(slowAmountStun);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Restore speed when stun ends
        if (!isDead)
        {
            speed = startSpeed;
        }

        stunCoroutine = null;
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
