using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections;
using Unity.Collections;
using UnityEngine.InputSystem;

public abstract class PlayerController : NetworkBehaviour
{

    #region Variables - This GameObject References
    [HideInInspector] public Rigidbody2D rig;
    [HideInInspector] public PlayerInput playerInput;
    #endregion

    #region Variables - Object References
    [SerializeField] private GameObject playerBullet;
    #endregion

    #region Variables - Input Variables
    private bool input_Shoot, input_Special;
    [HideInInspector] public Vector2 input_ShootDirection, input_Movement;
    #endregion

    #region Variables - Character Stats
    public abstract int characterSpeed { get; }
    public abstract int characterMaxHealth { get; }
    public abstract int characterFireRate { get; }
    public abstract int characterDamage { get; }
    public abstract float characterAbilityCooldown{ get; }
    #endregion

    #region Variables - Current Player Stats
    public int currentSpeed;
    public int currentMaxHealth;
    public int currentHealth;
    public int currentFireRate;
    public int currentDamage;
    public float currentAbilityCooldown;
    public float currentInvulnerabilityWindow;
    public float defaultInvulnerabilityWindow = 1.5f;
    #endregion

    #region Variables - Player State
    public bool isInvulnerable;
    public bool isDead;
    public bool enableControl;
    #endregion

    #region Variables - Time
    public float timeSinceLastFire;
    public float timeSinceLastAbility;
    #endregion

    #region Functions - Unity Events

    public void InitializeInput(PlayerInput soulPlayerInput) {
        playerInput = soulPlayerInput;
        playerInput.actions["Movement"].performed += OnMove;
        playerInput.actions["Shoot"].performed += OnShoot;
        playerInput.actions["Shoot Direction"].performed += OnShootDirection;
        playerInput.actions["Special"].performed += OnSpecial;
        playerInput.actions["Shoot"].canceled += OnShoot;
        playerInput.actions["Special"].canceled += OnSpecial;
    }

    public override void OnDestroy() {
        if (playerInput != null) {
            playerInput.actions["Movement"].performed -= OnMove;
            playerInput.actions["Shoot"].performed -= OnShoot;
            playerInput.actions["Shoot Direction"].performed -= OnShootDirection;
            playerInput.actions["Special"].performed -= OnSpecial;
            playerInput.actions["Shoot"].canceled -= OnShoot;
            playerInput.actions["Special"].canceled -= OnSpecial;
        }
        base.OnDestroy();
    }

    void Start () {
        rig = gameObject.GetComponent<Rigidbody2D>();
        Respawn();
    }

    public virtual void Update () {
        if (!enableControl || !IsOwner) {
            return;
        }

        if (input_Shoot)
        {
            Vector2 direction;
            // TODO: Check when null
            if (playerInput.currentControlScheme == "Keyboard") {
                Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(input_ShootDirection);
                direction = worldMousePos - transform.position;
            } else {
                direction = input_ShootDirection;
            }
            
            Shoot(direction);
        }
        
        if (input_Special)
        {
            CastSpecialAbility();
        }
    }

    public virtual void FixedUpdate () {
        if (enableControl && IsOwner) {
            Move();
        }   
    }
    #endregion

    #region Functions - Player Actions
    public virtual void Move () {
        Vector2 direction;
        direction = input_Movement;
        direction.Normalize();
        direction = direction * currentSpeed;
        rig.velocity = direction;
    }

    public virtual void Shoot (Vector2 direction) {
        if ((Time.time - timeSinceLastFire) > (1f/currentFireRate)) {
            timeSinceLastFire = Time.time;
            direction.Normalize();
            GameObject bulletInstance;
            bulletInstance = Instantiate(playerBullet, transform.position, transform.rotation);
            bulletInstance.GetComponent<PlayerBullet>().StartBullet(currentDamage, direction);
        }
    }
    
    public abstract void CastSpecialAbility();
    #endregion

    #region Functions - Player Health

    public virtual void GetHit () {
        
        if (isInvulnerable) {
            return;
        }

        currentHealth -= 1;

        if (currentHealth <= 0) {
            Die();
        } else {
            RecordInvulnerability(currentInvulnerabilityWindow);
        }
    }

    public virtual void GetHit (int damage) {
        if (isInvulnerable) {
            return;
        }

        currentHealth -= damage;

        if (currentHealth <= 0) {
            Die();
        } else {
            RecordInvulnerability(currentInvulnerabilityWindow);
        }
    }

    public virtual void RecordInvulnerability (float time) {
        StopCoroutine(InvulnerabilityCoroutine(time));
        StartCoroutine(InvulnerabilityCoroutine(time));
    }

    public virtual IEnumerator InvulnerabilityCoroutine (float time) {
        isInvulnerable = true;
        yield return new WaitForSeconds(time);
        isInvulnerable = false;
    }

    public virtual void Die () {
        isDead = true;
        enableControl = false;
    }
    #endregion

    #region Functions - Player Response to Game State
    public virtual void Respawn () {
        ResetStats();
        timeSinceLastAbility = Time.time - currentAbilityCooldown;
        isDead = false;
        enableControl = true;
    }

    public virtual void ResetStats () {
        currentSpeed = characterSpeed;
        currentDamage = characterDamage;
        currentAbilityCooldown = characterAbilityCooldown;
        currentFireRate = characterFireRate;
        currentMaxHealth = characterMaxHealth;
        currentHealth = currentMaxHealth;
        currentInvulnerabilityWindow = defaultInvulnerabilityWindow;
    }

    public virtual void Despawn () {
        enableControl = false;
    }
    #endregion

    #region Functions - Input
    public void OnMove(InputAction.CallbackContext context){
        input_Movement = context.ReadValue<Vector2>();
    }

    public void OnShoot(InputAction.CallbackContext context){
        input_Shoot = context.action.triggered;
    }

    public void OnSpecial(InputAction.CallbackContext context){
        input_Special = context.action.triggered;
    }

    public void OnShootDirection(InputAction.CallbackContext context){
        input_ShootDirection = context.ReadValue<Vector2>();
    }
    #endregion
}
