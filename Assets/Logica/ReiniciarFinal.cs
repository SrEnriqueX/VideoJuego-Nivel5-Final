using UnityEngine;
using UnityEngine.SceneManagement;

public class ReiniciarFinal : MonoBehaviour
{
    public void ReiniciarJuego()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
