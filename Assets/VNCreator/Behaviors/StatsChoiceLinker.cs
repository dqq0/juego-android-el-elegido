using UnityEngine;
using UnityEngine.UI;

namespace VNCreator
{
    /// <summary>
    /// Agrega este script al mismo objeto que VNCreator_DisplayUI.
    /// Por cada nodo con elecciones, define cuántos puntos da cada botón.
    /// 
    /// CÓMO USARLO:
    ///   - Llena el array "choices" con tantas entradas como nodos de elección tengas.
    ///   - Para cada entrada define qué hace el Botón 1, 2 y 3.
    ///   - El sistema detecta automáticamente en qué nodo estás y aplica los puntos correctos.
    /// </summary>
    public class StatsChoiceLinker : MonoBehaviour
    {
        [System.Serializable]
        public class ChoiceEffect
        {
            [Header("Nombre del nodo (para identificarlo fácilmente)")]
            public string nombreNodo = "Nodo X";

            [Header("Efectos del Botón 1")]
            public int btn1_aptitud = 0;
            public int btn1_miedo   = 0;

            [Header("Efectos del Botón 2")]
            public int btn2_aptitud = 0;
            public int btn2_miedo   = 0;

            [Header("Efectos del Botón 3")]
            public int btn3_aptitud = 0;
            public int btn3_miedo   = 0;
        }

        [Header("Configura el efecto de cada nodo de elección")]
        public ChoiceEffect[] choices;

        // Índice del nodo de elección actual (cuántos nodos de elección hemos pasado)
        private int choiceIndex = 0;

        // Referencia al DisplayUI para escuchar cuándo se avanza un nodo
        private VNCreator_DisplayUI displayUI;

        void Start()
        {
            displayUI = GetComponent<VNCreator_DisplayUI>();
            if (displayUI == null)
            {
                Debug.LogError("[StatsChoiceLinker] No se encontró VNCreator_DisplayUI en este objeto.");
                return;
            }

            // Conectar los botones de elección
            if (displayUI.choiceBtn1 != null)
                displayUI.choiceBtn1.onClick.AddListener(() => AplicarEfecto(0));
            if (displayUI.choiceBtn2 != null)
                displayUI.choiceBtn2.onClick.AddListener(() => AplicarEfecto(1));
            if (displayUI.choiceBtn3 != null)
                displayUI.choiceBtn3.onClick.AddListener(() => AplicarEfecto(2));
        }

        void AplicarEfecto(int botonIndex)
        {
            if (PlayerStats.instance == null)
            {
                Debug.LogWarning("[StatsChoiceLinker] No se encontró PlayerStats en la escena.");
                return;
            }

            if (choiceIndex >= choices.Length)
            {
                Debug.LogWarning($"[StatsChoiceLinker] No hay efecto configurado para el nodo de elección #{choiceIndex}.");
                choiceIndex++;
                return;
            }

            ChoiceEffect efecto = choices[choiceIndex];

            int aptitudDelta = 0;
            int miedoDelta   = 0;

            switch (botonIndex)
            {
                case 0:
                    aptitudDelta = efecto.btn1_aptitud;
                    miedoDelta   = efecto.btn1_miedo;
                    break;
                case 1:
                    aptitudDelta = efecto.btn2_aptitud;
                    miedoDelta   = efecto.btn2_miedo;
                    break;
                case 2:
                    aptitudDelta = efecto.btn3_aptitud;
                    miedoDelta   = efecto.btn3_miedo;
                    break;
            }

            // Aplicar estadísticas (soporte para valores negativos = reducir)
            if (aptitudDelta > 0) PlayerStats.instance.AumentarAptitud(aptitudDelta);
            else if (aptitudDelta < 0) PlayerStats.instance.DisminuirAptitud(-aptitudDelta);

            if (miedoDelta > 0) PlayerStats.instance.AumentarMiedo(miedoDelta);
            else if (miedoDelta < 0) PlayerStats.instance.DisminuirMiedo(-miedoDelta);

            Debug.Log($"[StatsChoiceLinker] Nodo '{efecto.nombreNodo}' | Btn{botonIndex+1} → Aptitud:{aptitudDelta:+#;-#;0}, Miedo:{miedoDelta:+#;-#;0}");

            choiceIndex++; // Avanzar al siguiente nodo de elección
        }
    }
}
