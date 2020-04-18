using UnityEngine;

namespace DeBox.PlayerPrefs.Tests
{
    public class TestDouble : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            PlayerPrefsDouble d = new PlayerPrefsDouble("test", 0);
            var value = 235534354352355.325235d;
            d.Value = value;
            if (d.ReadValue() != value)
            {
                Debug.LogError("Unexpected PlayerPrefsDouble value. Expected " + value + ", got: " + d.ReadValue());
            }
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }

}