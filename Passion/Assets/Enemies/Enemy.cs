using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Enemy : MonoBehaviour, IDamageable {

    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] float attackRadius = 4f;
    [SerializeField] float chaseRadius = 6f;
    [SerializeField] float damagePerShot = 5f;
    [SerializeField] float secondsBetweenShots = 2f;

    [SerializeField] Vector3 aimOffset = new Vector3(0, 1f, 0);

    [SerializeField] GameObject projectileToUse;
    [SerializeField] GameObject projectileSocket;

    bool isAttacking = false;


    float currentHealthPoints;
    AICharacterControl aiCharacterControl = null;
    GameObject player = null;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        aiCharacterControl = GetComponent<AICharacterControl>();
        currentHealthPoints = maxHealthPoints;

    }

    private void LateUpdate()
    {
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        if (distanceToPlayer <= attackRadius && !isAttacking)
        {
            isAttacking = true;
            InvokeRepeating("SpawnProjectile", 0f, secondsBetweenShots);  //TODO switch to coroutines
             
        }
      
        if(distanceToPlayer > attackRadius)
        {
            isAttacking = false;
            CancelInvoke();
        }
        
        if (distanceToPlayer <= chaseRadius)
        {
            aiCharacterControl.SetTarget(player.transform);
        }
        else
        {
            aiCharacterControl.SetTarget(transform);
        }
        
    }

    void SpawnProjectile()
    {
     
        GameObject newProjectile = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.identity);
        newProjectile.transform.LookAt(player.transform);

        Projectile projectileComponent = newProjectile.GetComponent<Projectile>();


        projectileComponent.damageCaused = damagePerShot;


        Vector3 unitVectorToPlayer = (player.transform.position + aimOffset - projectileSocket.transform.position).normalized;
      

        newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileComponent.projectileSpeed;
    


    }

    public float healthAsPercentage {get { return currentHealthPoints / maxHealthPoints; } }

    public void TakeDamage(float damage)
    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
        if (currentHealthPoints <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    void OnDrawGizmos()
    {
        //draw attack spheres
        Gizmos.color = new Color(255f, 0, 0f, .5f);
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        //draw chase spheres
        Gizmos.color = new Color(0f, 0f, 255, .5f);
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }

}
