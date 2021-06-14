using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visor : MonoBehaviour
{
    public string tagWall = "Wall";
    public string tagTarget = "Player";
    public GameObject agent;
    // Start is called before the first frame update
    void Start()
    {
        if(agent == null)
            agent = gameObject;
    }

    public void OnTriggerStay(Collider coll) {
        Debug.Log("Mirando cosas");
        string tag = coll.gameObject.tag;
        if(!tag.Equals(tagTarget))
            return;

        GameObject target = coll.gameObject;
        Vector3 agentPos = agent.transform.position; 
        Vector3 targetPos = target.transform.position; 
        Vector3 direction = targetPos - agentPos;

        float length = direction.magnitude; 
        direction.Normalize(); 
        Ray ray = new Ray(agentPos, direction);

        RaycastHit[] hits; 
        hits = Physics.RaycastAll(ray, length);

        int i; 
        for (i = 0; i < hits.Length; i++) 
        { 
            GameObject hitObj; 
            hitObj = hits[i].collider.gameObject; 
            tag = hitObj.tag; 
            if (tag.Equals(tagWall)){
                Debug.Log("Muro encontrado");
                return;
            }
        } 
        // TODO 
        // target is visible 
        // code your behaviour below
        Debug.Log("LO ESTOY VIENDO");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
