using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private Transform target;
    private Enemy targetEnemy;

    [Header("General")]
    
    public float range = 15f;

    [Header("Use Bullets (default)")]
    public GameObject bulletPrefab;
    public float fireRate = 1f;
    public float firePointNormalizedTime = 0.3f;
    private float fireCountdown = 0f;
    private bool isFiring = false;

    [Header("Use Laser")]
    public bool useLaser = false;

    public int damageOverTime = 30;
    public float slowAmount = 0.5f;

    public LineRenderer lineRenderer;
    public ParticleSystem impactEffect;
    public Light impactLight;

    [Header("Use Fire")]
    public bool useFire = false;

    public int fireDamage = 30;

    public LineRenderer fireLineRenderer;
    public ParticleSystem fireEffect;
    public Light fireLight;

    [Header("Unity Setup Fields")]
    public string enemyTag = "Enemy";

    public Transform partToRotate;
    public float turnSpeed = 10f;

    public Transform firePoint;

    // START #########################################################

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
        
    }

    // UPDATE TARGET #########################################################

    void UpdateTarget()
    {
        GameObject [] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
            targetEnemy = nearestEnemy.GetComponent<Enemy>();
        } else
        {
            target = null;
        }
    }

    // UPDATE #########################################################

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            if (useLaser)
            {
                if (lineRenderer.enabled)
                {
                    lineRenderer.enabled = false;
                    impactEffect.Stop();
                    impactLight.enabled = false;
                    AudioManager am = AudioManager.instance != null ? AudioManager.instance : FindObjectOfType<AudioManager>();
                    if (am != null)
                    {
                        am.Stop("Laser");
                    }
                    AnimationController animController = GetComponent<AnimationController>();
                    if (animController != null)
                    {
                        animController.PlayIdleAnimation();
                    }
                }
            }
            if (useFire)
            {
                if (fireLineRenderer.enabled)
                {
                    fireLineRenderer.enabled = false;
                    fireEffect.Stop();
                    fireLight.enabled = false;
                    AudioManager am = AudioManager.instance != null ? AudioManager.instance : FindObjectOfType<AudioManager>();
                    if (am != null)
                    {
                        am.Stop("Fire");
                    }
                }
            }
            return;
        }        
        // Target lock on
        LockOnTarget();
        if (useLaser)
        {
            Laser();
        }
        else
        {
            // Fire and bullets can work together
            if (useFire)
            {
                Fire();
            }
            
            // Bullets can fire alongside fire
            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }
            fireCountdown -= Time.deltaTime;
        }
    }

    Vector3 GetTargetCenter(Transform target)
    {
        // Get the Enemy component to access its GameObject
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            // Use the enemy's GameObject to get collider bounds
            Collider col = enemy.GetComponent<Collider>();
            if (col != null)
            {
                return col.bounds.center;
            }
            
            // Fallback to renderer bounds
            Renderer renderer = enemy.GetComponent<Renderer>();
            if (renderer != null)
            {
                return renderer.bounds.center;
            }
        }
        
        // Final fallback: use transform position with a manual offset
        return target.position + Vector3.up * 1f;
    }

    // LOCK ON TARGET #########################################################

    void LockOnTarget()
    {
        Vector3 targetCenter = GetTargetCenter(target);
        Vector3 dir = targetCenter - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    // LASER #########################################################

    void Laser()
    {
        targetEnemy.TakeDamage(damageOverTime * Time.deltaTime);
        targetEnemy.Slow(slowAmount);

        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
            impactEffect.Play();
            impactLight.enabled = true;
            AudioManager am = AudioManager.instance != null ? AudioManager.instance : FindObjectOfType<AudioManager>();
            if (am != null)
            {
                am.Play("Laser");
            }
            AnimationController animController = GetComponent<AnimationController>();
            if (animController != null)
            {
                animController.PlayLaserAnimation();
            }
        }

        Vector3 targetCenter = GetTargetCenter(target);

        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, targetCenter);

        Vector3 dir = firePoint.position - targetCenter;

        impactEffect.transform.position = targetCenter + dir.normalized * 0.5f;
        impactEffect.transform.rotation = Quaternion.LookRotation(dir);
    }

    // FIRE #########################################################
    void Fire()
    {
        targetEnemy.TakeDamage(fireDamage * Time.deltaTime);

        if (!fireLineRenderer.enabled)
        {
            fireLineRenderer.enabled = true;
            fireEffect.Play();
            fireLight.enabled = true;
            AudioManager am = AudioManager.instance != null ? AudioManager.instance : FindObjectOfType<AudioManager>();
            if (am != null)
            {
                am.Play("Fire");
            }
        }

        Vector3 targetCenter = GetTargetCenter(target);

        fireLineRenderer.SetPosition(0, firePoint.position);
        fireLineRenderer.SetPosition(1, targetCenter);

        Vector3 dir = firePoint.position - targetCenter;

        fireEffect.transform.position = targetCenter + dir.normalized * 0.5f;
        fireEffect.transform.rotation = Quaternion.LookRotation(dir);
    }

    // SHOOT #########################################################

    public void Shoot()
    {
        if (isFiring) return;
        
        AnimationController animController = GetComponent<AnimationController>();
        Animator animator = GetComponent<Animator>();
        
        if (animController != null)
        {
            animController.PlayShootAnimation();
        }
        
        // Start coroutine to fire when animation reaches fire point
        StartCoroutine(FireBulletAtAnimationPoint(animator, firePointNormalizedTime));
    }

    IEnumerator FireBulletAtAnimationPoint(Animator animator, float normalizedTime)
    {
        isFiring = true;
        
        if (animator != null)
        {
            // Wait until animation is playing and reaches the desired point
            yield return null; // Wait one frame for animation to start
            
            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < normalizedTime)
            {
                yield return null;
            }
        }
        
        GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
            bullet.Seek(target);
        
        isFiring = false;
    }
}
