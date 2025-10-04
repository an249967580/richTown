using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    public class CareerView : MonoBehaviour
    {
        public Toggle tgTexas, tgBull;
        public StatView goTexas, goBull;

        private void Awake()
        {
            tgTexas.onValueChanged.AddListener((isOn) =>
            {
                goTexas.gameObject.SetActive(isOn);
            });

            tgBull.onValueChanged.AddListener((isOn) =>
            {
                goBull.gameObject.SetActive(isOn);
            });
            goTexas.game = GameType.dz;
            goBull.game = GameType.bull; 
        }
    }
}
