using UnityEngine;
using TMPro;

namespace Assets.SimpleLocalization.Scripts
{
    /// <summary>
    /// Localize TextMeshPro component.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedTextMeshPro : MonoBehaviour
    {
        [SerializeField]
        private string _localizationKey;
        public string LocalizationKey
        {
            get { return _localizationKey; }
            set { 
                _localizationKey = value;
                Localize();
            }
        }

        [SerializeField]
        private object[] _localizationVariables;
        public object[] LocalizationVariables
        {
            get { return _localizationVariables; }
            set
            {
                _localizationVariables = value;
                Localize();
            }
        }

        private TMP_Text tmp;

        private void Awake()
        {
            tmp = GetComponent<TMP_Text>();
        }

        public void Start()
        {
            if (_localizationKey != null) Localize();
            LocalizationManager.OnLocalizationChanged += Localize;
        }

        public void OnDestroy()
        {
            LocalizationManager.OnLocalizationChanged -= Localize;
        }

        private void Localize()
        {
            if (_localizationKey == null || tmp == null) return;

            if (_localizationKey == "") 
            {
                tmp.text = "";
                return;
            }
            
            if( _localizationVariables != null )
            {
                tmp.text = LocalizationManager.Localize(LocalizationKey, LocalizationVariables);
            }
            else
            {
                tmp.text = LocalizationManager.Localize(LocalizationKey);
            }
        }
    }
}