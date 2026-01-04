using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{

    private Transform target;

    public float speed = 70f;
    public float explosionRadius = 0f;
    public int damage = 50;

    [Header("Burn Effect")]
    public bool hasBurnEffect = false;
    public float burnDamage = 10f; // Damage per second (like laser)
    public float burnDuration = 3f;

    public GameObject impactEffect;
    public GameObject burnEffect;
    
    public void Seek(Transform _target)
    {
        target = _target;
    }

    // UPDATE #########################################################

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        // Rotate bullet to face the target
        transform.rotation = Quaternion.LookRotation(dir);
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);

    }

    // HIT TARGET #########################################################

    void HitTarget()
    {
        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 5f);

        if (explosionRadius > 0f)
        {
            Explode();
        } else
        {
            Damage(target);
        }

        Destroy(gameObject);
    }

    // EXPLODE #########################################################

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Enemy")
            {
                Damage(collider.transform);
            }
        }
    }

    // DAMAGE #########################################################

    void Damage (Transform enemy)
    {
        Enemy e = enemy.GetComponent<Enemy>();
        if (e != null)
        {
            e.TakeDamage(damage);
            
            // Apply burn effect if enabled
            if (hasBurnEffect)
            {
                e.ApplyBurn(burnDamage, burnDuration);
                GameObject burnEffectIns = (GameObject)Instantiate(burnEffect, enemy.position, enemy.rotation);
                burnEffectIns.transform.SetParent(enemy); // Auto-destroyed when enemy dies
                Destroy(burnEffectIns, burnDuration); // Fallback if enemy survives
            }
        }
    }

    // ON DRAW GIZMOS SELECTED #########################################################

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

}
