using System;
using UnityEngine;

namespace DeBox.PlayerPrefsExtensions
{
    /// <summary>
    /// Abstract class describing a PlayerPrefs valus
    /// </summary>
    /// <typeparam name="T">Type of the stored object</typeparam>
    public abstract class PlayerPrefsValue<T>
    {
        /// <summary>
        /// The value of the PlayerPref
        /// </summary>
        public abstract T Value { get; set; }

        /// <summary>
        /// Delete the object from player prefs
        /// </summary>
        public abstract void Delete();

        /// <summary>
        /// Indicates that the PlayerPref value is set
        /// </summary>
        public abstract bool IsSet { get; }
        
        /// <summary>
        /// Writes the value to PlayerPrefs
        /// </summary>
        /// <param name="value">The high-level value</param>
        protected abstract void WriteValue(T value);
        
        /// <summary>
        /// Reads the value from PlayerPrefs
        /// </summary>
        /// <returns>The high-level value</returns>
        public abstract T ReadValue();
    }

    /// <summary>
    /// An abstract, simple implementation of PlayerPrefsValue for single-key premitive values
    /// </summary>
    /// <typeparam name="T">Type of the stored object</typeparam>
    public abstract class SimplePlayerPrefsValue<T> : PlayerPrefsValue<T>
    {
        private T _cachedValue = default(T);
        private T _defaultValue = default(T);
        private bool _isCached = false;
        
        public string KeyName { get; private set; }
        public virtual T DefaultValue => _defaultValue;
        
        public override T Value
        {
            get => GetValue();
            set => SetValue(value);
        }

        /// <summary>
        /// Create an uninitialized prefs
        /// </summary>
        public SimplePlayerPrefsValue() : base()
        {
        }

        /// <summary>
        /// Initialize the PlayerPref object with the Pref KeyName and the default value
        /// </summary>
        /// <param name="keyName">Name of the Unity PlayerPref key</param>
        /// <param name="defaultValue">The default value to be returned, if the key is not set</param>
        public SimplePlayerPrefsValue(string keyName, T defaultValue) : base()
        {
            Initialize(keyName, defaultValue);
        }
        
        public override void Delete()
        {
            UnityEngine.PlayerPrefs.DeleteKey(KeyName);
            _cachedValue = _defaultValue;
        }
        
        /// <summary>
        /// Initialize the PlayerPref object with the Pref KeyName and the default value
        /// </summary>
        /// <param name="keyName">Name of the Unity PlayerPref key</param>
        /// <param name="defaultValue">The default value to be returned, if the key is not set</param>
        public void Initialize(string keyName, T defaultValue)
        {
            KeyName = keyName;
            _defaultValue = defaultValue;
        }
        
        public override bool IsSet
        {
            get
            {
                if (string.IsNullOrEmpty(KeyName))
                {
                    throw new Exception("Not initialized");
                }

                return UnityEngine.PlayerPrefs.HasKey(KeyName);
            }
        }

        private T GetValue()
        {
            if (string.IsNullOrEmpty(KeyName))
            {
                throw new Exception("Not initialized");
            }
            if (!_isCached)
            {
                if (IsSet)
                {
                    _cachedValue = ReadValue();
                }
                else
                {
                    _cachedValue = DefaultValue;
                }
            }
            _isCached = true;
            return _cachedValue;
        }

        private void SetValue(T value)
        {
            if (string.IsNullOrEmpty(KeyName))
            {
                throw new Exception("Not initialized");
            }
            WriteValue(value);
            _cachedValue = value;
            _isCached = true;
        }
    }

    /// <summary>
    /// PlayerPrefs string object store
    /// </summary>
    public class PlayerPrefsString : SimplePlayerPrefsValue<string>
    {
        public PlayerPrefsString() : base() {}

        public PlayerPrefsString(string keyName, string defaultValue) : base(keyName, defaultValue) {}
            
        protected override void WriteValue(string value)
        {
            UnityEngine.PlayerPrefs.SetString(KeyName, value);
        }

        public override string ReadValue()
        {
            return UnityEngine.PlayerPrefs.GetString(KeyName, string.Empty);
        }
    }


    /// <summary>
    /// PlayerPrefs bool object store
    /// </summary>
    public class PlayerPrefsBool : SimplePlayerPrefsValue<bool>
    {
        public PlayerPrefsBool() : base() {}
        public PlayerPrefsBool(string keyName, bool defaultValue) : base(keyName, defaultValue) {}
        
        protected override void WriteValue(bool value)
        {
            UnityEngine.PlayerPrefs.SetInt(KeyName, value ? 1 : 0);
        }

        public override bool ReadValue()
        {
            return UnityEngine.PlayerPrefs.GetInt(KeyName, 0) > 0;
        }

 
    }

    /// <summary>
    /// PlayerPrefs int object store
    /// </summary>
    public class PlayerPrefsInt : SimplePlayerPrefsValue<int>
    {
        public PlayerPrefsInt() : base() {}
        public PlayerPrefsInt(string keyName, int defaultValue) : base(keyName, defaultValue) {}
        protected override void WriteValue(int value)
        {
            UnityEngine.PlayerPrefs.SetInt(KeyName, value);
        }

        public override int ReadValue()
        {
            return UnityEngine.PlayerPrefs.GetInt(KeyName, 0);
        }
    }

    /// <summary>
    /// PlayerPrefs float object store
    /// </summary>
    public class PlayerPrefsFloat : SimplePlayerPrefsValue<float>
    {
        public PlayerPrefsFloat() : base() {} 
        public PlayerPrefsFloat(string keyName, float defaultValue) : base(keyName, defaultValue) {}
        
        protected override void WriteValue(float value)
        {
            UnityEngine.PlayerPrefs.SetFloat(KeyName, value);
        }

        public override float ReadValue()
        {
            return UnityEngine.PlayerPrefs.GetFloat(KeyName, 0);
        }
    }

    /// <summary>
    /// PlayerPrefs double object store
    ///
    /// The double is split to two integers as Unity.PlayerPrefs has no "double" setter/getter
    ///
    /// <example>
    /// <code>
    /// public class MyComponent : MonoBehaviour // You don't have to be in a MonoBehaviour to use this
    /// {
    ///    private PlayerPrefsDouble _myDoublePref = new PlayerPrefsDouble("doubleKey1", 0);
    ///
    ///    private void Start()
    ///    {
    ///        _myDoublePref.Value = _myDoublePref.Value + 0.111f; // Stop start the game, this will persist
    ///    }
    /// }
    /// 
    /// </code>
    /// </example>
    /// </summary>
    public class PlayerPrefsDouble : SimplePlayerPrefsValue<double>
    {
        /// <summary>
        /// Create a new uninitialized PlayerPrefsDouble
        /// </summary>
        public PlayerPrefsDouble() : base() {}
        
        /// <summary>
        /// Create a new PlayerPrefsDouble
        /// </summary>
        /// <param name="keyName">The PlayerPrefs prefix for keys of this instance</param>
        /// <param name="defaultValue">The default value to return if not set in PlayerPrefs</param>
        public PlayerPrefsDouble(string keyName, double defaultValue) : base(keyName, defaultValue) {}

        private string SubKeyName0 => KeyName + "-0";
        private string SubKeyName1 => KeyName + "-1";

        /// <summary>
        /// Overrides IsSet of SimplePlayerPrefsValue, checks that both 32bit keys that describe the 64 double
        /// are set in player prefs
        /// </summary>
        public override bool IsSet => PlayerPrefs.HasKey(SubKeyName0) && PlayerPrefs.HasKey(SubKeyName1);

        /// <summary>
        /// Implementation of WriteValue of SimplePlayerPrefsValue
        ///
        /// Writes two integer playerprefs that together describe the double
        /// Each integer is a 32bit part of the 64bit double
        /// </summary>
        /// <param name="value">The double value to store</param>
        protected override void WriteValue(double value)
        {
            var n = BitConverter.DoubleToInt64Bits(value);
            var components = Long2Int(n);
            PlayerPrefs.SetInt(SubKeyName0, components[0]);
            PlayerPrefs.SetInt(SubKeyName1, components[1]);
        }

        /// <summary>
        /// Implementation of ReadValue of SimplePlayerPrefsValue
        ///
        /// Read two integer playerprefs that together describe the double
        /// Each integer is a 32bit part of the 64bit double
        /// </summary>
        /// <returns>The double value</returns>
        public override double ReadValue()
        {
            var component0 = PlayerPrefs.GetInt(SubKeyName0, 0);
            var component1 = PlayerPrefs.GetInt(SubKeyName1, 0);
            var n = Int2Long(component0, component1);
            var value = BitConverter.Int64BitsToDouble(n);
            return value;
        }
        
        private int[] Long2Int(long a) {
            int a1 = (int)(a & uint.MaxValue);
            int a2 = (int)(a >> 32);
            return new int[] { a1, a2 };
        }

        private long Int2Long(int a1, int a2)
        {
            long b = a2;
            b = b << 32;
            b = b | (uint)a1;
            return b;
        }
    }
}
