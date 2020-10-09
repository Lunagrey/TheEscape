using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabChicken : MonoBehaviour
{

    [SerializeField] private GameObject chickenGrabbed;

    private GameObject chickenCanGrab;

    private bool canGrab = false;

    [SerializeField] private float chickenVelocity = 10f;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) &&  chickenGrabbed != null)
        {
            DropChicken();
        }
        else if (Input.GetMouseButtonDown(0) && canGrab == true && chickenGrabbed == null)
        {
            GrabTheChicken();
        }
        else if (Input.GetMouseButtonDown(1) && chickenGrabbed != null)
        {
            ThrowChicken();
        }
    }

    private void ThrowChicken()
    {
        chickenGrabbed.GetComponent<Chicken>().took = false;

        GameObject closestGuard = FindClosestEnemy();

        if (closestGuard == null)
            return;

        Vector3 normalizeDirection = (closestGuard.transform.position - transform.position).normalized;


        //chickenGrabbed.tranform.position += normalizeDirection * chickenVelocity * Time.deltaTime;

        chickenGrabbed.GetComponent<Rigidbody>().useGravity = true;

        chickenGrabbed.GetComponent<Rigidbody>().velocity = normalizeDirection * chickenVelocity;


        chickenGrabbed.transform.parent = null;

        chickenGrabbed = null;
    }


    private void DropChicken()
    {
        chickenGrabbed.GetComponent<Chicken>().took = false;
        chickenGrabbed.transform.parent = null;

        chickenGrabbed.GetComponent<Rigidbody>().useGravity = true;

        chickenGrabbed = null;
    }

    // graab the chicken and make it children of player object
    private void GrabTheChicken()
    {
        if (chickenCanGrab == null || canGrab == false)
            return;

        chickenCanGrab.transform.SetParent(transform.transform);

        chickenCanGrab.transform.localPosition = new Vector3(0, 1.2f, 0);

        chickenGrabbed = chickenCanGrab;

        chickenGrabbed.GetComponent<Chicken>().took = true;

        chickenGrabbed.GetComponent<Rigidbody>().useGravity = false;


        chickenCanGrab = null;
        canGrab = true;

    }

    // get a chicken close to you
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);

        if (other.tag != "Chicken")
            return;

        chickenCanGrab = other.transform.gameObject;


        canGrab = true;
    }

    // clear chicken close to you
    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Chicken")
            return;

        chickenCanGrab = null;
        canGrab = false;
    }


    public GameObject FindClosestEnemy()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Guard");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }
}
