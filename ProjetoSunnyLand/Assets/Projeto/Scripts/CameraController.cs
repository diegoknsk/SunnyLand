using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float offsetX = 3f;
    public float smooth = 0.1f; //suavidade no movimento

    public float limitedUp = 2f;
    public float limitedDown = 0;
    public float limitedLeft = 0;
    public float limitedRight = 100f;

    private Transform player;
    private float playerX;
    private float playerY;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>()?.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            playerX = Mathf.Clamp(player.position.x, limitedLeft, limitedRight);
            playerY = Mathf.Clamp(player.position.y, limitedDown, limitedUp);

            //lerp parametro inicial, final e taxa percentual de interpolação entre os pontos
            transform.position = Vector3.Lerp(transform.position, new Vector3(playerX, playerY, transform.position.z), smooth);
        }
    }
}
