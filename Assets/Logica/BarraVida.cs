using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BarraVida : MonoBehaviour
{
    public Image rellenoBarraVida;
    private Personaje personaje;
    private float vidaMaxima;

    void Start()
    {
        personaje = GameObject.Find("Atahualpa").GetComponent<Personaje>();
        vidaMaxima = personaje.vida;
    }


    void Update()
    {
        rellenoBarraVida.fillAmount = personaje.vida / vidaMaxima;
    }
}
