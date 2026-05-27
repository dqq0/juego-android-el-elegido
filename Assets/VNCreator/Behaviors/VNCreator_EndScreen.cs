using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace VNCreator
{
    public class VNCreator_EndScreen : MonoBehaviour
    {
        public Button restartButton;
        public Button mainMenuButton;
        [Scene]
        public string mainMenu;
        public Text finalMessageText; // Añade este campo para el texto

        void Start()
        {
            restartButton.onClick.AddListener(Restart);
            mainMenuButton.onClick.AddListener(MainMenu);
        }

        void OnEnable()
        {
            // Se ejecuta al mostrarse la pantalla final
            if (finalMessageText != null && PlayerStats.instance != null)
            {
                int aptitud = PlayerStats.instance.aptitud;
                int miedo = PlayerStats.instance.miedo;

                if (aptitud >= miedo + 5)
                {
                    // Final 1
                    finalMessageText.text = "El protagonista superó todas sus pruebas y se alzó con la victoria en su misión, logrando su objetivo con gran éxito.";
                }
                else if (miedo >= aptitud + 5)
                {
                    // Final 2
                    finalMessageText.text = "El astronauta, al no cumplir con las expectativas, sucumbe frente al éxito de su contrincante.";
                }
                else
                {
                    // Final 3
                    finalMessageText.text = "Una mezcla entre miedo y aptitud logró que el protagonista descubriera el secreto de su viaje.";
                }
            }
        }

        void Restart()
        {
            GameSaveManager.NewLoad("MainGame");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }

        void MainMenu()
        {
            SceneManager.LoadScene(mainMenu, LoadSceneMode.Single);
        }
    }
}
