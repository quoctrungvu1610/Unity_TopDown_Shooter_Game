using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_HitBox : HitBox
{
    private Player player;
    protected override void Awake()
    {
        base.Awake();
         player = GetComponentInParent<Player>();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        player.health.TakeDamage(damage);
    }
}
