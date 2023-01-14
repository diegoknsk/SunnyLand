using UnityEngine;

public class IASlug : MonoBehaviour
{
    public Transform        enemie;
    public SpriteRenderer   enemieSprite;
    public Transform[]      positions;
    public float            speed;
    public bool             isRight;
    private int             idTarget;
    // Start is called before the first frame update
    void Start()
    {
        enemieSprite = enemie.gameObject.GetComponent<SpriteRenderer>();
        enemie.position = positions[0].position;
        idTarget = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemie != null)
        {
            enemie.position = Vector3.MoveTowards(enemie.position, positions[idTarget].position, speed * Time.deltaTime);
            if (enemie.position == positions[idTarget].position)
            {
                idTarget += 1;
                if (idTarget == positions.Length)
                {
                    idTarget = 0;
                }
            }

            if (positions[idTarget].position.x < enemie.position.x && isRight == true)
            {
                Flip();
            }
            else if (positions[idTarget].position.x > enemie.position.x && !isRight)
            {
                Flip();
            }
        }
    }

    void Flip()
    {
        isRight = !isRight;
        enemieSprite.flipX = !enemieSprite.flipX;
    }
}
