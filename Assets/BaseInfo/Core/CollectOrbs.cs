using UnityEngine;

public class CollectOrbs : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;
    [SerializeField] string orbTag;
    [SerializeField] CoreData coreData;

    void Update()
    {
        GameObject[] orbs = GameObject.FindGameObjectsWithTag(orbTag);
        foreach(GameObject orb in orbs)
        {
            float dist = Vector3.Distance(orb.transform.position, transform.position);
            orb.transform.position = Vector3.MoveTowards(orb.transform.position, transform.position, speed / dist * Time.deltaTime);
            if(orb.transform.position == transform.position)
            {
                Destroy(orb);
                coreData.addEnergy(1.0f);
            }
        }
    }
}
