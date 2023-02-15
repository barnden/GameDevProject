using System; // contains [Serializable]
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSystem : MonoBehaviour
{
    [Serializable]

    // These do nothing currently
    private class ProjectileParams
    {
        public GameObject projectile;
        public float distToSpawnProjectile;
        public float lifetime;
        public float speed;
        public float damage;
        public float fireRate;
        public float AOESize;
        public float fireSpread;
    }

    [SerializeField] ProjectileParams[] projectilesToFire;
    private GameObject target = null;
    public void setTarget(GameObject target)
    {
        this.target = target;

        // Get front of enemy so we can instantiate object between enemy and target

        foreach (ProjectileParams currProjectileParams in projectilesToFire)
        {
            StartCoroutine(FireProjectile(currProjectileParams));
        }
    }

    IEnumerator FireProjectile(ProjectileParams param)
    {
        // keep the subrutine running so the enemy keeps firing
        while(true)
        {
            if (!target) { yield break; }
            Vector2 targetDir = target.transform.position - transform.position;

            // find front of enemy to instantiate bullet
            Vector2 front = new Vector2(transform.position.x + (targetDir.x * param.distToSpawnProjectile), 
                                        transform.position.y + (targetDir.y * param.distToSpawnProjectile));

            // Instantiate the projectile with provided params
            GameObject projectile = Instantiate(param.projectile, front, Quaternion.identity);

            /*projectile.GetComponent<BaseProjectile>().Init(projectile, param.lifetime, 
                                                        param.speed, param.damage, 
                                                        param.AOESize, targetDir);*/
            projectile.GetComponent<BaseProjectile>().Init(projectile); // To be removed once custom UI for inspector is made
            projectile.GetComponent<BaseProjectile>().setDirection(targetDir); // To be removed once custom UI for inspector is made

            yield return new WaitForSeconds(param.fireRate);
        }
    }
}
