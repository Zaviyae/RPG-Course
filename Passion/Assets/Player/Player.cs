using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{

    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] int enemyLayer = 13;
    [SerializeField] float damagePerHit = 15f;
    [SerializeField] float minTimeBetweenHits = 1f;
    [SerializeField] float maxAttackRange = 3f;

    GameObject currentTarget;

    float currentHealthPoints;
    CameraRaycaster cameraRayCaster;
    float lastHitTime = 0f;

    private void Start()
    {
        cameraRayCaster = FindObjectOfType<CameraRaycaster>();
        cameraRayCaster.notifyMouseClickObservers += OnMouseClick;
        currentHealthPoints = maxHealthPoints;
    }


    public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

    public void TakeDamage(float damage)
    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
    }

    void OnMouseClick(RaycastHit raycastHit, int layerHit)
    {
        if (layerHit == enemyLayer)
        {
            GameObject enemy = raycastHit.collider.gameObject;

            //check enemy is in range
            if ((enemy.transform.position - transform.position).magnitude > maxAttackRange)
            {
                return;
                //returns from the method.
            }

            //else do this
            currentTarget = enemy;
            var enemyComponent = enemy.GetComponent<Enemy>();
            if (Time.time - lastHitTime > minTimeBetweenHits)
            {
                enemyComponent.TakeDamage(damagePerHit);
                lastHitTime = Time.time;
            }


        }
    }
}
