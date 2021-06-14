//
//
//
//
//
//BAAAAAAAAAAAASUUUUUUUUUUURAAAAAAAAAAAAAAAAAAAAAAAAAAAA
//
//
//
//
//
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class VisionCone : MonoBehaviour
{
    //private MeshCollider coneMesh;
    private List<Collider> collidersInSight = new List<Collider>();

    public List<Collider> OverlappingColliders()
    {
        return collidersInSight;
    }

    private void OnTriggerEnter(Collider other)
    {
        collidersInSight.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        collidersInSight.Remove(other);
    }
}

