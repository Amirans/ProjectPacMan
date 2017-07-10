using UnityEngine;
using System.Collections;


public interface IDamagable<T1,T2>
{
    void Damage(float DamageValue, GameObject Instigator);
}