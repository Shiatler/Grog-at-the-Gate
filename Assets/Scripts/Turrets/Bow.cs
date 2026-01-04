using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    Turret turret;

    void Start()
    {
        turret = GetComponent<Turret>();
    }

    public void Shoot()
    { 
        turret.Shoot();
    }
}
