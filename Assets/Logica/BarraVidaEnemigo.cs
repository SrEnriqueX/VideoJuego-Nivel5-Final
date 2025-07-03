using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BarraVidaEnemigo : MonoBehaviour
{
    public Image rellenoBarraVidaEnemigo;
    private Enemigo enemigo;
    private float vidaMaximaEnemigo;
    void Start()
    {
        enemigo = GameObject.Find("Enemigo").GetComponent<Enemigo>();
        vidaMaximaEnemigo = enemigo.vida;
    }

    
    void Update()
    {
        rellenoBarraVidaEnemigo.fillAmount = enemigo.vida / vidaMaximaEnemigo;
    }
}
