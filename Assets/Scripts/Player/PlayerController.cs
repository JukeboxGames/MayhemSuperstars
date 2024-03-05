using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections;
using Unity.Collections;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System.Collections.Generic;

public abstract class PlayerController : NetworkBehaviour, IReactToGameState
{

    #region Variables - This GameObject References
    [HideInInspector] public Rigidbody2D rig;
    [HideInInspector] public PlayerInput playerInput;
    #endregion

    #region Variables - Object References
    [SerializeField] private GameObject playerBullet;
    [SerializeField] private GameObject pushHitbox;
    [HideInInspector] public PlayerSoul playerSoul;
    [SerializeField] private SO_GameState so_GameState;
    public List<GameObject> interactables = new List<GameObject>();
    #endregion

    #region Variables - Input Variables
    private bool input_Shoot, input_Push;
    protected bool input_Special;
    protected Vector2 input_ShootDirection, input_Movement;
    #endregion

    #region Variables - Character Stats
    public abstract int characterSpeed { get; }
    public abstract int characterMaxHealth { get; }
    public abstract int characterFireRate { get; }
    public abstract int characterDamage { get; }
    public abstract float characterAbilityCooldown{ get; }
    #endregion

    #region Variables - Current Player Stats
    [HideInInspector] public int currentSpeed;
    [HideInInspector] public int currentMaxHealth;
    [HideInInspector] public int currentHealth;
    [HideInInspector] public int currentFireRate;
    [HideInInspector] public int currentDamage;
    [HideInInspector] public float currentAbilityCooldown;
    [HideInInspector] public float currentInvulnerabilityWindow;
    [HideInInspector] public float defaultInvulnerabilityWindow = 1.5f;
    [HideInInspector] public float defaultPushCooldown = 0.5f;
    [HideInInspector] public float currentPushCooldown = 0.5f;
    #endregion

    #region Variables - Player State
    protected bool isInvulnerable;
    [HideInInspector] public bool isDead;
    protected bool enableControl;
    protected List<Vector2> appliedForces = new List<Vector2>();
    #endregion

    #region Variables - Time
    protected float timeSinceLastFire;
    protected float timeSinceLastAbility;
    protected float timeSinceLastPush;
    #endregion

    #region Functions - Unity Events

    public override void OnDestroy() {
        if (playerInput != null) {
            playerInput.actions["Movement"].performed -= OnMove;
            playerInput.actions["Shoot"].performed -= OnShoot;
            playerInput.actions["Shoot"].canceled -= OnShoot;
            playerInput.actions["Shoot Direction"].performed -= OnShootDirection;
            playerInput.actions["Special"].performed -= OnSpecial;
            playerInput.actions["Special"].canceled -= OnSpecial;
            playerInput.actions["Interact"].canceled -= OnInteract;
            playerInput.actions["Push"].performed -= OnPush;
            playerInput.actions["Push"].canceled -= OnPush;
        }
        base.OnDestroy();
    }

    void Start () {
        rig = gameObject.GetComponent<Rigidbody2D>();
        so_GameState.gameState.AddListener(ReactToGameState);
        Respawn();
    }

    public virtual void FixedUpdate () {
        if (!enableControl || !IsOwner) {
            return;
        }

        Move();

        if (input_Shoot)
        {
            Vector2 direction;
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

        if(input_Push)
        {
            Push();
        }
    }

    public void ReactToGameState(GameManager.GameState gameState) {
        if (gameState == GameManager.GameState.Countdown) {
            Respawn();
        }
    }

    #endregion

    #region Functions - Player Actions
    protected virtual void Move () {
        Vector2 direction;
        direction = input_Movement;
        direction.Normalize();
        direction = direction * currentSpeed;
        rig.velocity = direction;
        foreach(Vector2 force in appliedForces) {
            rig.AddForce(force, ForceMode2D.Impulse);
        }
    }

    public virtual void AddInstantForce(Vector2 force, float time){
        StartCoroutine(TimeToApplyForce(force, time));
    }

    public virtual IEnumerator TimeToApplyForce(Vector2 force, float time){
        appliedForces.Add(force);
        yield return new WaitForSeconds(time);
        appliedForces.Remove(force);
    }

    protected virtual void Shoot (Vector2 direction) {
        if ((Time.time - timeSinceLastFire) > (1f/currentFireRate)) {
            timeSinceLastFire = Time.time;
            direction.Normalize();
            if (direction == Vector2.zero) return;
            GameObject bulletInstance;
            bulletInstance = Instantiate(playerBullet, transform.position, transform.rotation);
            bulletInstance.GetComponent<PlayerBullet>().StartBullet(currentDamage, direction);
        }
    }

    protected virtual void Push() {
        if (input_Movement == Vector2.zero) {
            return;
        }

        if ((Time.time - timeSinceLastPush) > currentPushCooldown) {
            GameObject instance = Instantiate(pushHitbox,transform);
            Vector2 direction = input_Movement.normalized;
            instance.transform.localPosition = direction;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg -90;
            instance.transform.eulerAngles = Vector3.forward * angle;
            instance.GetComponent<PushHitbox>().direction = direction;
            timeSinceLastPush = Time.time;
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
        rig.velocity = Vector2.zero;
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

        // Debug position
        transform.position  = Vector2.zero;
    }

    public virtual void ResetStats () {
        currentSpeed = characterSpeed;
        currentDamage = characterDamage;
        currentAbilityCooldown = characterAbilityCooldown;
        currentFireRate = characterFireRate;
        currentMaxHealth = characterMaxHealth;
        currentHealth = currentMaxHealth;
        currentInvulnerabilityWindow = defaultInvulnerabilityWindow;
        currentPushCooldown = defaultPushCooldown;
    }

    public virtual void Despawn () {
        enableControl = false;
    }
    #endregion

    #region Functions - Input
    public void InitializeInput(PlayerInput soulPlayerInput) {
        playerSoul = soulPlayerInput.gameObject.GetComponent<PlayerSoul>();
        playerInput = soulPlayerInput;
        playerInput.actions["Movement"].performed += OnMove;
        playerInput.actions["Shoot"].performed += OnShoot;
        playerInput.actions["Shoot"].canceled += OnShoot;
        playerInput.actions["Shoot Direction"].performed += OnShootDirection;
        playerInput.actions["Special"].performed += OnSpecial;
        playerInput.actions["Special"].canceled += OnSpecial;
        playerInput.actions["Interact"].canceled += OnInteract;
        playerInput.actions["Push"].performed += OnPush;
        playerInput.actions["Push"].canceled += OnPush;
    }

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

    public void OnInteract(InputAction.CallbackContext context){
        if (interactables.Count > 0) {
            float minDistance = Mathf.Infinity;
            GameObject selected = null;
            foreach(GameObject interactable in interactables) {
                float dist = Vector3.Distance(interactable.transform.position, transform.position);
                if (dist < minDistance)
                {
                    selected = interactable;
                    minDistance = dist;
                }
            }
            selected?.GetComponent<Interactable>().Interact(this.gameObject);
        }
    }

    public void OnPush(InputAction.CallbackContext context){
        input_Push = context.action.triggered;
    }
    #endregion
}
