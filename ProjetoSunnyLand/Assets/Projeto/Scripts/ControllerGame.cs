using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerGame : MonoBehaviour
{
    // Start is called before the first frame update
    public int score;
    public Text txtScore;
    public GameObject hitPrefab;

    public Sprite[] imagensVidas;
    public Image barraVida;

    public AudioSource fxGame; // quem controla o som
    public AudioClip fxCenouraColetada; // som especifico
    public AudioClip fxExplosao;
    public AudioClip fxDie;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pontuar(int qtdPontos)
    {
        score += qtdPontos;
        txtScore.text = score.ToString();
        fxGame.PlayOneShot(fxCenouraColetada);
    }


    public void AtualizaBarraVidas(int healthVida)
    {
        barraVida.sprite = imagensVidas[healthVida];
    }
}
