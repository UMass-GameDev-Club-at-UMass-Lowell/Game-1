using UnityEngine;

/** \brief
Basic projectile functionality that every other projectile type must inherit from to be compatible with other systems (like the attack manager).
You can also put this script onto any object or prefab to give it normal projectile functionality.

Documentation updated 9/4/2024
\author Stephen Nuttall, Roy Pascual
\todo Replace damagePlayers and damageNonPlayers bools with one layer mask that allows for more choices in what is damaged.
*/
public class BasicProjectile : MonoBehaviour
{
    /// \brief Reference to the game object that holds the sprite renderer of the projectile.
    /// Often this is just the projectile itself, but sometimes there's a child with the sprite. This allows compatability with both systems.
    public GameObject sprite;
    /// The speed the projectile moves. The basic projectile moves at a consistent speed every second.
    public float speed = 10f;
    /// The amount of damage the projectile will apply if it comes in contact with something it can damage.
    public int damage = 30;
    /// True if the projectile is facing (and thus moving) to the left. Likewise, false if the projectile is facing to the right.
    public bool facingLeft = false;
    /// If true, the projectile can damage the player.
    public bool damagePlayers = true;
    /// If true, the projectile can damage objects that aren't the player (assuming they have an ObjectHealth component).
    public bool damageNonPlayers = true;
    /// Sound effect that plays when the projectile is spawned in.
    public string spawnSFX = "bullet_fire";

    /// The dimensions of the sprite when the projectile is facing left.
    Vector3 spriteScaleLeft;
    /// The dimensions of the sprite when the projectile is facing right.
    Vector3 spriteScaleRight;

    /// Runs AwakeMethods(), which can be changed in scripts that inherit from BasicProjectile.
    void Awake()
    {
        AwakeMethods();
    }

    /// \brief Runs when Awake() is called
    /// 
    protected virtual void AwakeMethods()
    {
        // determine ahead of time how to display the sprite depending on the direction the projectile is facing
        spriteScaleRight = sprite.transform.localScale;
        spriteScaleLeft = new Vector3(-sprite.transform.localScale.x, sprite.transform.localScale.y, sprite.transform.localScale.z);
    }

    /// Runs StartMethods(), which can be changed in scripts that inherit from BasicProjectile.
    void Start()
    {
        StartMethods();
    }

    /// Plays the spawn sound effect.
    protected virtual void StartMethods()
    {
        AudioManager.Instance.PlaySFX(spawnSFX);
    }

    /// Runs UpdateMethods(), which can be changed in scripts that inherit from BasicProjectile.
    void Update()
    {
        UpdateMethods();
    }

    /// Moves the projectile in the direction it's set to face, and ensures the sprite is facing that direction.
    protected virtual void UpdateMethods()
    {
        if (facingLeft)
        {
            // move projectile to the left by (speed * Time.deltaTime), accounting for the amount of time that has past since the last frame
            transform.position = new Vector2(transform.position.x - (speed * Time.deltaTime), transform.position.y);
            sprite.transform.localScale = spriteScaleLeft;
        }
        else
        {
            // move projectile to the right by (speed * Time.deltaTime), accounting for the amount of time that has past since the last frame
            transform.position = new Vector2(transform.position.x + (speed * Time.deltaTime), transform.position.y);
            sprite.transform.localScale = spriteScaleRight;
        }
    }

    /// <summary>
    /// Runs OnTriggerEnterMethods(), which can be changed in scripts that inherit from BasicProjectile.
    /// This is required if your projectile's Collider2D component is set to be a trigger.
    /// </summary>
    /// <param name="collision">Represents the object the projectile collided with.</param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerEnterMethods(collision);
    }

    /// \brief Runs if the projectile collides with an object (and it's a trigger zone). Does nothing by default.
    /// \note This function only called if the projectile's Collider2D component is set to be a trigger. This is an option that makes the object have no
    /// collision, but sets off OnTriggerEnter2D() if anything steps into it (usually for something like a finish line in a race, or something similar).
    /// The basic projectile is not set to be a trigger, so this function isn't used in BasicProjectile. A script inheriting from it for a projectile that is
    /// a trigger though (maybe a laser that goes through multiple enemies) could use this.
    /// <param name="collision">Represents the object the projectile collided with.</param>
    protected virtual void OnTriggerEnterMethods(Collider2D collision)
    {

    }

    /// <summary>
    /// Runs OnCollisionEnterMethods(), which can be changed in scripts that inherit from BasicProjectile.
    /// </summary>
    /// <param name="collisionInfo">Represents the object the projectile collided with.</param>
    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        OnCollisionEnterMethods(collisionInfo);
    }

    /// \brief Runs if the projectile collides with an object. Damages the object it collided with if possible, then destroys the projectile.
    /// <param name="collisionInfo">Represents the object the projectile collided with.</param>
    protected virtual void OnCollisionEnterMethods(Collision2D collisionInfo)
    {
        // If the object we collided with has an object health AND
        // (both damageNonPlayers and damagePlayers are true)
        // OR (the object is not the player AND damangeNonPlayers is true)
        // OR (the object is the player AND damangePlayers is true),
        // then apply damage to the object.
        if (collisionInfo.collider.TryGetComponent<ObjectHealth>(out var objHealth) && (damageNonPlayers && damagePlayers ||
        !(collisionInfo.collider.CompareTag("Player") && damageNonPlayers || (collisionInfo.collider.CompareTag("Player") && damagePlayers))))
        {
            objHealth.TakeDamage(transform, damage);
        }

        // Destory the projectile, regardless of if what we hit can be damaged.
        Destroy(sprite);
        Destroy(gameObject);
    }

    /// Flip the direction the projectile is facing (and thus moving).
    /// Switches facingLeft to be the opposite of what it currently is.
    public void FlipDirection()
    {
        facingLeft = !facingLeft;
    }
}
