using UnityEngine;
public class GroundTermite : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 6f;
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float stopDistance = 1.5f;
    [Header("Eye")]
    public SpriteRenderer eyeRenderer;  
    public float timeToDetonate = 2f;
    [Header("Explosion")]
    public GameObject explosionEffect;
    public float holeRadius = 2f;
    [Header("Sound")]
    public AudioClip explosionSFX;
    private AudioSource tAudioSource;
    private Transform player;
    private PlayerController playerController;
    private bool hasLockedOn = false;
    private bool isDetonating = false;
    private float detonateTimer = 0f;
    private GroundGenerator groundGen;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
        groundGen = FindObjectOfType<GroundGenerator>();
        eyeRenderer.color = Color.white;
        tAudioSource = GetComponent<AudioSource>();
        if (tAudioSource == null) tAudioSource = gameObject.AddComponent<AudioSource>();
    }


    void Update()
    {
        if (player == null) return;
        float distance = Vector2.Distance(transform.position, player.position);
        if (!hasLockedOn)
        {
            if (distance <= detectionRange && playerController.isMoving)
            {
                hasLockedOn = true;
            }
        }
        if (isDetonating)
        {
            HandleDetonation();
            return;
        }
        if (hasLockedOn && distance > stopDistance)
        {
            //follow
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        }
        else if (hasLockedOn && distance <= stopDistance)
        {
            //Start Timer
            isDetonating = true;
            detonateTimer = 0f;
        }
    }
    void HandleDetonation()
    {
        detonateTimer += Time.deltaTime;
        float t = Mathf.Clamp01(detonateTimer / timeToDetonate);
        eyeRenderer.color = Color.Lerp(Color.white, Color.red, t);
        if (t >= 1f)
        {
            Explode();
        }
    }
    void Explode()
    {
        Debug.Log("Termite blast!");

        if (explosionEffect)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            playSound();
        }
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            playSound();
        }
        if (groundGen != null)
        {
            groundGen.MakeHole(transform.position, holeRadius);
        }
        if (Vector2.Distance(transform.position, player.position) <= stopDistance + 0.5f)
        {
            player.GetComponent<DeathHandler>()?.Die();
        }
        Destroy(gameObject);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
    void playSound()
    {
        if (explosionSFX != null) tAudioSource.PlayOneShot(explosionSFX);
    }
}