using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    public float speed = 5f;
    public float rotateSpeed = 200f;

    [Header("Explosion")]
    public ParticleSystem explosionEffect;  
    public float holeRadius = 2f;

    private GroundGenerator groundGen;
    private Transform target;

    [Header("Sound")]
    public AudioClip blastSound;
    private AudioSource rocketAudioSource;

    void Start()
    {
        groundGen = FindObjectOfType<GroundGenerator>();
        rocketAudioSource = GetComponent<AudioSource>();
        if (rocketAudioSource == null) rocketAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (target == null) return;

        Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
        direction.Normalize();

        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        transform.Rotate(0, 0, -rotateAmount * rotateSpeed * Time.deltaTime);
        transform.position += transform.up * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
 
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Missile hit the Player!!!!");

            collision.GetComponent<DeathHandler>()?.Die();
            Explode();
        }
       
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || !collision.isTrigger)
        {
            Debug.Log("Missile hit!-!-!");
            Explode();
        }
    }

    void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        if (groundGen != null)
        {
            groundGen.MakeHole(transform.position, holeRadius);
        }

        if (blastSound != null) rocketAudioSource.PlayOneShot(blastSound);
        Destroy(gameObject);

    }
}
