using System;

namespace DeBox.PlayerPrefs
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
        protected abstract T ReadValue();
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

        protected override string ReadValue()
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

        protected override bool ReadValue()
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

        protected override int ReadValue()
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

        protected override float ReadValue()
        {
            return UnityEngine.PlayerPrefs.GetFloat(KeyName, 0);
        }
    }


}
