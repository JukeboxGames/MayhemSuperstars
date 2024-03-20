using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections;
using Unity.Collections;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode.Components;

// Controls the player vessel
public abstract class PlayerController : NetworkBehaviour, IReactToGameState, IPushable
{

    #region Variables - This GameObject References
    [HideInInspector] public Rigidbody2D rig;
    [HideInInspector] public PlayerInput playerInput;
    public NetworkVariable<int> playerNumber = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] private SO_PlayerEvents[] playerEvents_SOs;
    private SO_PlayerEvents myPlayerEventSO;
    #endregion

    #region Variables - Object References
    [SerializeField] private GameObject playerBullet;
    [SerializeField] private GameObject fakePlayerBullet;
    [SerializeField] private GameObject pushHitbox;
    [HideInInspector] public PlayerSoul playerSoul;
    [SerializeField] private SO_GameState so_GameState;
    [SerializeField] protected SO_Bullets so_Bullets;
    [SerializeField] private SO_FakeBullets so_FakeBullets;
    [HideInInspector] public int bulletIndex = 0;
    [HideInInspector] public List<GameObject> interactables = new List<GameObject>();
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
    public abstract float characterAbilityCooldown { get; }
    #endregion

    #region Variables - Current Player Stats
    // Used to change the players stats without changing the character base
    // (for sponsors, for example)
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

    // Desubscribe from every input action on destroy
    public override void OnDestroy()
    {
        if (playerInput != null)
        {
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

    // Subscribe to game state changes
    void Start()
    {
        rig = gameObject.GetComponent<Rigidbody2D>();
        so_GameState.gameState.AddListener(ReactToGameState);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Respawn();
    }

    // Check for inputs
    public virtual void FixedUpdate()
    {
        if (!enableControl || !IsOwner)
        {
            return;
        }

        Move();

        if (input_Shoot)
        {
            Vector2 direction;
            if (playerInput.currentControlScheme == "Keyboard")
            {
                Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(input_ShootDirection);
                direction = worldMousePos - transform.position;
            }
            else
            {
                direction = input_ShootDirection;
            }

            Shoot(direction);
        }

        if (input_Special)
        {
            CastSpecialAbility();
            //GetHit();
        }

        if (input_Push)
        {
            Push();
        }
    }

    public void ReactToGameState(GameManager.GameState gameState)
    {
        switch (gameState)
        {
            case GameManager.GameState.Countdown:
                Respawn();
                break;
            case GameManager.GameState.WinScreen:
                enableControl = false;
                break;
            default:
                break;
        }
    }

    #endregion

    #region Functions - Player Actions
    protected virtual void Move()
    {
        Vector2 direction;
        direction = input_Movement;
        direction.Normalize();
        direction = direction * currentSpeed;
        rig.velocity = direction;

        // If there are outside forces applied to the player, consider them for the movement
        foreach (Vector2 force in appliedForces)
        {
            rig.AddForce(force, ForceMode2D.Impulse);
        }
    }

    public virtual void AddInstantForce(Vector2 force, float time)
    {
        StartCoroutine(TimeToApplyForce(force, time));
    }

    public virtual IEnumerator TimeToApplyForce(Vector2 force, float time)
    {
        appliedForces.Add(force);
        yield return new WaitForSeconds(time);
        appliedForces.Remove(force);
    }

    protected virtual void Shoot(Vector2 direction)
    {
        if (direction == Vector2.zero) return;
        // if shooting cooldown has ellapsed
        if ((Time.time - timeSinceLastFire) > (1f / currentFireRate))
        {
            timeSinceLastFire = Time.time;
            direction.Normalize();

            // Instantiate bullet locally
            GameObject bulletInstance;
            bulletInstance = Instantiate(so_Bullets.bulletPrefabs[bulletIndex], transform.position, transform.rotation);
            // Call server to instantiate fake bullet on all the other clients
            bulletInstance.GetComponent<PlayerBullet>().StartBullet(currentDamage, direction);
            SpawnFakeBulletServerRpc(bulletIndex, transform.position, direction);
        }
    }
    
    // Instantiate fake bullet on server
    [ServerRpc(RequireOwnership = false)]
    protected void SpawnFakeBulletServerRpc(int bulletIdx, Vector2 startPos, Vector2 direction, ServerRpcParams serverRpcParams = default)
    {
        ulong senderId = serverRpcParams.Receive.SenderClientId;
        if (senderId != NetworkManager.Singleton.LocalClientId)
        {
            GameObject bulletInstance;
            bulletInstance = Instantiate(so_FakeBullets.fakeBulletPrefabs[bulletIdx], startPos, Quaternion.identity);
            bulletInstance.GetComponent<FakePlayerBullet>().StartBullet(direction);
        }
        SpawnFakeBulletClientRpc(bulletIdx, startPos, direction, senderId);
    }

    // Instantiate fake bullet on clients
    [ClientRpc]
    protected void SpawnFakeBulletClientRpc(int bulletIdx, Vector2 startPos, Vector2 direction, ulong senderId)
    {
        if (senderId != NetworkManager.Singleton.LocalClientId && !IsServer)
        {
            GameObject bulletInstance;
            bulletInstance = Instantiate(so_FakeBullets.fakeBulletPrefabs[bulletIdx], startPos, Quaternion.identity);
            bulletInstance.GetComponent<FakePlayerBullet>().StartBullet(direction);
        }
    }

    // Pushes in the direction of the movement
    protected virtual void Push()
    {
        if (input_Movement == Vector2.zero)
        {
            return;
        }

        // If push cooldown has ellapsed
        if ((Time.time - timeSinceLastPush) > currentPushCooldown)
        {
            Vector2 direction = input_Movement.normalized;
            // Call server to spawn push hitbox
            PushServerRpc(direction, transform.position, gameObject);
            timeSinceLastPush = Time.time;
        }
    }

    // spawn push hitbox on server
    [ServerRpc]
    protected void PushServerRpc(Vector2 direction, Vector2 pos, NetworkObjectReference vess)
    {
        if (vess.TryGet(out NetworkObject obj))
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            Quaternion quat = Quaternion.identity;
            quat.eulerAngles = Vector3.forward * angle;
            GameObject instance = Instantiate(pushHitbox, obj.gameObject.transform.position + (Vector3)direction, quat);
            instance.GetComponent<NetworkObject>().Spawn();
            // Enable push hitbox
            instance.GetComponent<PushHitbox>().direction.Value = direction;
            instance.GetComponent<PushHitbox>().playerNumber.Value = obj.gameObject.GetComponent<PlayerController>().playerNumber.Value;
        }
    }

    public abstract void CastSpecialAbility();
    #endregion

    #region Functions - Player Health
    // No parameter GetHit, takes damage as 1
    public virtual void GetHit()
    {

        if (isInvulnerable)
        {
            return;
        }

        currentHealth -= 1;
        myPlayerEventSO?.event_PlayerHealthChanged.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
            myPlayerEventSO.event_PlayerDead.Invoke();
        }
        else
        {
            RecordInvulnerability(currentInvulnerabilityWindow);
        }
    }

    public virtual void GetHit(int damage)
    {
        if (isInvulnerable)
        {
            return;
        }

        currentHealth -= damage;
        myPlayerEventSO?.event_PlayerHealthChanged.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
            myPlayerEventSO?.event_PlayerDead.Invoke();
        }
        else
        {

            RecordInvulnerability(currentInvulnerabilityWindow);
        }
    }

    public virtual void RecordInvulnerability(float time)
    {
        StopCoroutine(InvulnerabilityCoroutine(time));
        StartCoroutine(InvulnerabilityCoroutine(time));
    }

    // Be invulnerable for a period of time
    public virtual IEnumerator InvulnerabilityCoroutine(float time)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(time);
        isInvulnerable = false;
    }

    public virtual void Die()
    {
        rig.velocity = Vector2.zero;
        isDead = true;
        enableControl = false;
    }
    #endregion

    #region Functions - Player Response to Game State
    public virtual void Respawn()
    {
        ResetStats();
        myPlayerEventSO?.event_PlayerHealthChanged.Invoke(currentHealth);
        timeSinceLastAbility = Time.time - currentAbilityCooldown;
        isDead = false;
        enableControl = true;
        transform.position = Vector2.zero;
    }

    public virtual void ResetStats()
    {
        currentSpeed = characterSpeed;
        currentDamage = characterDamage;
        currentAbilityCooldown = characterAbilityCooldown;
        currentFireRate = characterFireRate;
        currentMaxHealth = characterMaxHealth;
        currentHealth = currentMaxHealth;
        currentInvulnerabilityWindow = defaultInvulnerabilityWindow;
        currentPushCooldown = defaultPushCooldown;
    }

    public virtual void Despawn()
    {
        enableControl = false;
        transform.position = new Vector2(0f, 50f);
    }
    #endregion

    #region Functions - Input
    // Subscribe to the soul's input actions
    public void InitializeInput(PlayerInput soulPlayerInput)
    {
        playerSoul = soulPlayerInput.gameObject.GetComponent<PlayerSoul>();
        playerNumber.Value = playerSoul.playerNumber.Value;
        myPlayerEventSO = playerEvents_SOs[playerNumber.Value - 1];
        myPlayerEventSO.event_PlayerHealthChanged.Invoke(characterMaxHealth);
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

    public void OnMove(InputAction.CallbackContext context)
    {
        input_Movement = context.ReadValue<Vector2>();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        input_Shoot = context.action.triggered;
    }

    public void OnSpecial(InputAction.CallbackContext context)
    {
        input_Special = context.action.triggered;
    }

    public void OnShootDirection(InputAction.CallbackContext context)
    {
        input_ShootDirection = context.ReadValue<Vector2>();
    }

    // searches for all interactables on interactables list and calls interact to the closest one
    // interactables should add themselves to the interactables list of the player
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (interactables.Count > 0 && enableControl)
        {
            float minDistance = Mathf.Infinity;
            GameObject selected = null;
            foreach (GameObject interactable in interactables)
            {
                float dist = Vector3.Distance(interactable.transform.position, transform.position);
                if (dist < minDistance)
                {
                    selected = interactable;
                    minDistance = dist;
                }
            }
            selected?.GetComponent<IInteractable>().Interact(this.gameObject);
        }
    }

    public void OnPush(InputAction.CallbackContext context)
    {
        input_Push = context.action.triggered;
    }
    #endregion
}
