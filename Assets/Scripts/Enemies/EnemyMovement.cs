using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{
    private Transform target;
    private int wavepointIndex = 0;

    private Enemy enemy;

    // START #########################################################

    void Start()
    {
        enemy = GetComponent<Enemy>();
        target = Waypoints.points[0];
        
        // Face the first waypoint immediately on spawn
        Vector3 dir = target.position - transform.position;
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    // UPDATE #########################################################

    void Update()
    {
        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized * enemy.speed * Time.deltaTime, Space.World);

        // Rotate to face movement direction
        if (dir != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        if (Vector3.Distance(transform.position, target.position) <=0.4f)
        {
            GetNextWaypoint();
        }

        enemy.speed = enemy.startSpeed;
    }

    // GET NEXT WAYPOINT #########################################################

    void GetNextWaypoint()
    {

        if (wavepointIndex >= Waypoints.points.Length - 1)
        {
            EndPath();
            return;
        }
        
        wavepointIndex++;
        target = Waypoints.points[wavepointIndex];
    }

    // END PATH #########################################################

    void EndPath()
    {
        PlayerStats.Lives--;
        WaveSpawner.enemiesAlive--;
        FindObjectOfType<AudioManager>().Play("Break");
        Destroy(gameObject);
    }
}
