using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NPCTankController : AdvancedFSM 
{
    public GameObject Bullet;
    public int health;
    public Transform healthBar;
    public float healthScale;
    public bool inChargingArea = false;
    public float groundCheckDistance = 5f;
    public Text damageText;
    public TankRank myRank;

    [SerializeField] Material material1, material2, material3, material4, material5, material6, material7;
    [SerializeField] Sprite fastClassBadge, regularClassBadge;


    Transform restPoint;
    [SerializeField] Transform offDutyPoint;
    [SerializeField] Transform offDutyTeleportPoint;

    // We overwrite the deprecated built-in `rigidbody` variable.
    new private Rigidbody rigidbody;

    //Initialize the Finite state machine for the NPC tank
    protected override void Initialize()
    {
        health = 100;


        restPoint = GameObject.FindGameObjectWithTag("RechargeStation").transform;
        offDutyPoint = GameObject.FindGameObjectWithTag("OffDutyPoint").transform;
        offDutyTeleportPoint = GameObject.FindGameObjectWithTag("OffDutyTeleportPoint").transform;

        elapsedTime = 0.0f;
        shootRate = 2.0f;

        //Get the target enemy(Player)
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;

        //Get the healthbar's scale
        healthScale = healthBar.transform.localScale.x;

        //Get the rigidbody
        rigidbody = GetComponent<Rigidbody>();

        if (!playerTransform)
            print("Player doesn't exist.. Please add one with Tag named 'Player'");

        //Get the turret of the tank
        turret = gameObject.transform.GetChild(0).transform;
        bulletSpawnPoint = turret.GetChild(0).transform;

        myRank = GetComponent<TankRank>();

        float randNum = Random.Range(0.0f, 1.0f);
        if (randNum <= 0.5f)
        {
            myRank.rankName = TankRank.Rank.Fast;
            gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = fastClassBadge;
            this.GetComponent<Sight>().enabled = true;
        }
        else
        {
            myRank.rankName = TankRank.Rank.Regular;
            gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = regularClassBadge;
            this.GetComponent<Touch>().enabled = true;
        }

        //Start Doing the Finite State Machine
        ConstructFSM();
    }


    
    //Update each frame
    protected override void FSMUpdate()
    {
        //Check for health
        elapsedTime += Time.deltaTime;
        


        switch(CurrentState.ID)
        {
            case FSMStateID.Attacking:
                GetComponent<Renderer>().material = material1;
                break;
            case FSMStateID.Chasing:
                GetComponent<Renderer>().material = material2;
                break;
            case FSMStateID.Patrolling:
                GetComponent<Renderer>().material = material3;
                break;
            case FSMStateID.OffDuty:
                GetComponent<Renderer>().material = material4;
                break;
            case FSMStateID.Bored:
                GetComponent<Renderer>().material = material5;
                break;
            case FSMStateID.Camp:
                GetComponent<Renderer>().material = material6;
                break;
            case FSMStateID.Resting:
                GetComponent<Renderer>().material = material7;
                break;
            case FSMStateID.Camo:
                break;
        }
        

    }

    protected override void FSMFixedUpdate()
    {

        //set how full the healthbar shows
        healthBar.localScale = new Vector3(health / 100.0f * healthScale, 14f, 1f);
        
        CurrentState.Reason(playerTransform, transform);
        CurrentState.Act(playerTransform, transform);

        
    }

    public void SetTransition(Transition t) 
    { 
        PerformTransition(t); 
    }

    private void ConstructFSM()
    {
        //Get the list of points
        pointList = GameObject.FindGameObjectsWithTag("WandarPoint");

        Transform[] waypoints = new Transform[pointList.Length];
        int i = 0;
        foreach(GameObject obj in pointList)
        {
            waypoints[i] = obj.transform;
            i++;
        }

        PatrolState patrol = new PatrolState(waypoints);
        patrol.AddTransition(Transition.SawPlayer, FSMStateID.Chasing);
        patrol.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        patrol.AddTransition(Transition.LowHealth, FSMStateID.Resting);
        patrol.AddTransition(Transition.ReachBored, FSMStateID.Bored);
        patrol.AddTransition(Transition.ReachNinjaCamp, FSMStateID.Camp);
        patrol.AddTransition(Transition.GoOffDuty, FSMStateID.OffDuty);

        ChaseState chase = new ChaseState(waypoints);
        chase.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);
        chase.AddTransition(Transition.ReachPlayer, FSMStateID.Attacking);
        chase.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        chase.AddTransition(Transition.LowHealth, FSMStateID.Resting);

        AttackState attack = new AttackState(waypoints);
        attack.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);
        attack.AddTransition(Transition.SawPlayer, FSMStateID.Chasing);
        attack.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        attack.AddTransition(Transition.LowHealth, FSMStateID.Resting);
        attack.AddTransition(Transition.CamoAttack, FSMStateID.Camo);

        DeadState dead = new DeadState();
        dead.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        RestingState resting = new RestingState(this.transform, restPoint);
        resting.AddTransition(Transition.FullHealth, FSMStateID.Patrolling);
        resting.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        resting.AddTransition(Transition.LowHealth, FSMStateID.Resting);

        BoredState bored = new BoredState(this.transform);
        bored.AddTransition(Transition.SawPlayer, FSMStateID.Chasing);
        bored.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        bored.AddTransition(Transition.LowHealth, FSMStateID.Resting);
        bored.AddTransition(Transition.ReachPlayer, FSMStateID.Attacking);
        bored.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);

        NinjaCampState camp = new NinjaCampState(this.transform);
        camp.AddTransition(Transition.SawPlayer, FSMStateID.Chasing);
        camp.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        camp.AddTransition(Transition.LowHealth, FSMStateID.Resting);
        camp.AddTransition(Transition.ReachPlayer, FSMStateID.Attacking);
        camp.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);

        OffDutyState offduty = new OffDutyState(offDutyPoint, offDutyTeleportPoint, this.transform);
        offduty.AddTransition(Transition.ReturnToDuty, FSMStateID.Patrolling);
        offduty.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        offduty.AddTransition(Transition.LowHealth, FSMStateID.Resting);

        CamoState camo = new CamoState(this.transform);
        camo.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        camo.AddTransition(Transition.LowHealth, FSMStateID.Resting);
        camo.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);

        AddFSMState(patrol);
        AddFSMState(chase);
        AddFSMState(attack);
        AddFSMState(dead);
        AddFSMState(resting);
        AddFSMState(bored);
        AddFSMState(camp);
        AddFSMState(offduty);
        AddFSMState(camo);
    }

    /// <summary>
    /// Check the collision with the bullet
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        //Check what its on
        if(collision.gameObject.tag == "MainGround")
        {
            Debug.Log("Grounded");
            inChargingArea = false;

        }
        else if(collision.gameObject.tag == "RechargeStation")
        {
            Debug.Log("Pitstop");
            inChargingArea = true;
        }
        //Reduce health
        else if (collision.gameObject.tag == "Bullet" && !(Vector3.Distance(transform.position, restPoint.position) < 100f && this.CurrentStateID == FSMStateID.Resting))
        {
            if (this.CurrentStateID == FSMStateID.Camo)
            {
                float randNum = Random.Range(0.0f, 1.0f);
                if (randNum <= 0.3f)
                {
                    damageText.text = "dodged";
                    StartCoroutine(DamageTextTimer());
                }
                else
                {
                    damageText.text = "50";
                    StartCoroutine(DamageTextTimer());
                    health -= 50;

                    if (health <= 0)
                    {
                        Debug.Log("Switch to Dead State");
                        this.GetComponent<MeshRenderer>().enabled = true;
                        turret.gameObject.GetComponent<MeshRenderer>().enabled = true;
                        healthBar.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                        SetTransition(Transition.NoHealth);
                        Explode();
                    }
                    else if(health < 100 && RestingState.remainingRests > 0){
                        RestingState.remainingRests--;
                        this.GetComponent<MeshRenderer>().enabled = true;
                        turret.gameObject.GetComponent<MeshRenderer>().enabled = true;
                        healthBar.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                        Debug.Log("Switch to Resting State");
                        SetTransition(Transition.LowHealth);
                    }
                }
            }
            else
            {
                damageText.text = "50";
                StartCoroutine(DamageTextTimer());
                health -= 50;

                if (health <= 0)
                {
                    Debug.Log("Switch to Dead State");
                    SetTransition(Transition.NoHealth);
                    Explode();
                }
                else if(health < 100 && RestingState.remainingRests > 0){
                    RestingState.remainingRests--;
                    Debug.Log("Switch to Resting State");
                    SetTransition(Transition.LowHealth);
                }
            }
        }
    }

    protected void Explode()
    {
        float rndX = Random.Range(10.0f, 30.0f);
        float rndZ = Random.Range(10.0f, 30.0f);
        for (int i = 0; i < 3; i++)
        {
            rigidbody.AddExplosionForce(10000.0f, transform.position - new Vector3(rndX, 10.0f, rndZ), 40.0f, 10.0f);
            rigidbody.velocity = transform.TransformDirection(new Vector3(rndX, 20.0f, rndZ));
        }

        Destroy(gameObject, 1.5f);
    }

    /// <summary>
    /// Shoot the bullet from the turret
    /// </summary>
    public void ShootBullet()
    {
        if (elapsedTime >= shootRate)
        {
            float randNum = Random.Range(0.0f, 1.0f);
            if (myRank.rankName == TankRank.Rank.Fast && randNum <= 0.6f)
            {
                StartCoroutine(RapidFire(3));
                
            }
            else if (myRank.rankName == TankRank.Rank.Regular && randNum <= 0.2f)
            {
                StartCoroutine(RapidFire(3));
            }
            Instantiate(Bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            elapsedTime = 0.0f;
        }
    }

    private IEnumerator DamageTextTimer()
    {
        yield return new WaitForSeconds(1.0f);
        damageText.text = "";
    }

    private IEnumerator RapidFire(int shots)
    {
        for (int i = 0; i < shots; i++)
        {
            yield return new WaitForSeconds(0.2f);
            Instantiate(Bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        }
    }
}
