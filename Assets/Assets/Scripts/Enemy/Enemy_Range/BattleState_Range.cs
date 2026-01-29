using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleState_Range : EnemyState
{
    private Enemy_Range enemy;
    private float lastTimeShot = -10;
    private int bulletsShot = 0;

    private int bulletsPerAttack = 1;
    private float weaponCooldown = 1.5f;

    private float coverCheckTimer;
    private bool firstTimeAttack = true;

    public BattleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();
        SetupValuesForFirstAttack();

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        enemy.visuals.EnableIK(true, true);

        stateTimer = enemy.attackDelay;
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsSeeingPlayer()) 
        {
            enemy.FaceTarget(enemy.aim.position);
        }

        if (MustAdvancePlayer()) 
        {
            stateMachine.ChangeState(enemy.advancePlayerState);
        }

        enemy.FaceTarget(enemy.player.position);

        if (stateTimer > 0)
        {
            return;
        }

        ChangeCoverIfShould();

        if (WeaponOutOfBullets())
        {
            if (enemy.IsUnstoppable() && UnstoppableWalkReady()) 
            {
                enemy.advanceDuration = weaponCooldown;
                stateMachine.ChangeState(enemy.advancePlayerState);
            }

            if (WeaponOnCooldown())
            {
                AttemptToResetWeapon();
            }
            return;
        }

        if (CanShoot() && enemy.IsAimOnPlayer())
        {
            Shoot();
        }
    }

    private bool MustAdvancePlayer() 
    {
        if (enemy.IsUnstoppable()) 
        {
            return false;
        }
        return enemy.IsPlayerInAgrresionRange() == false && ReadyToLeaveCover();
    }

    private bool UnstoppableWalkReady() 
    {
        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.position);
        bool outOfStoppingDistance = distanceToPlayer > enemy.advanceStoppingDistance;
        bool unstoppableWalkOnCooldown = Time.time < enemy.weaponData.minWeaponCooldown + enemy.advancePlayerState.lastTimeAdvanced;

        return outOfStoppingDistance && unstoppableWalkOnCooldown == false;
    }

    #region Cover System Region

    private bool ReadyToLeaveCover() 
    {
        return Time.time > enemy.minCoverTime + enemy.runToCoverState.lastTimeTookCover;
    }

    private void ChangeCoverIfShould()
    {
        if (enemy.coverPerk != CoverPerk.CanTakeAndChangeCover) 
        {
            return;
        }

        coverCheckTimer -= Time.deltaTime;

        if (coverCheckTimer < 0) 
        {
            coverCheckTimer = 0.1f; //Check Rate

            if (ReadyToChangeCover())
            {
                if (enemy.CanGetCover())
                {
                    stateMachine.ChangeState(enemy.runToCoverState);
                }
            }
        }
    }

    private bool ReadyToChangeCover() 
    {
        bool inDanger = IsPlayerClose() || IsPlayerInClearSight();
        bool advanceTimeIsOver = Time.time > enemy.advancePlayerState.lastTimeAdvanced + enemy.advanceDuration;

        return inDanger && advanceTimeIsOver;
    }


    private bool IsPlayerClose() 
    {
        return Vector3.Distance(enemy.transform.position, enemy.player.transform.position) < enemy.safeDistance;
    }

    private bool IsPlayerInClearSight() 
    {
        Vector3 directionToPlayer = enemy.player.transform.position - enemy.transform.position;

        if (Physics.Raycast(enemy.transform.position, directionToPlayer, out RaycastHit hit)) 
        {
            return hit.collider.gameObject.GetComponentInParent<Player>();
        }

        return false;
    }

    #endregion

    #region Weapon Region
    private void AttemptToResetWeapon()
    {
        bulletsShot = 0;
        bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack();
        weaponCooldown = enemy.weaponData.GetWeaponCooldown();
    }

    private bool WeaponOnCooldown()
    {
        return Time.time > lastTimeShot + weaponCooldown;
    }

    private bool WeaponOutOfBullets()
    {
        return bulletsShot >= bulletsPerAttack;
    }

    private bool CanShoot()
    {
        return Time.time > lastTimeShot + 1 / enemy.weaponData.fireRate;
    }

    private void Shoot()
    {
        enemy.FireSingleBullet();
        lastTimeShot = Time.time;
        bulletsShot++;
    }

    private void SetupValuesForFirstAttack()
    {
        if (firstTimeAttack)
        {
            firstTimeAttack = false;
            bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack();
            weaponCooldown = enemy.weaponData.GetWeaponCooldown();
        }
    }

    #endregion
}
