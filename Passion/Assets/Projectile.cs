using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float damageCaused;

    public float projectileSpeed;


    private void OnTriggerEnter(Collider other)
    {
        print("Projectile hit : " + other.gameObject);
        Component damageableComponent = other.gameObject.GetComponent(typeof(IDamageable));

        if (damageableComponent)
        {
            (damageableComponent as IDamageable).TakeDamage(damageCaused);
        }
    }

}
