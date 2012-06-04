using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroMarkdown
{
    public class PropertyChangeEventArgs
    {
        public string PropertyName;
        public object OldValue;
        public object NewValue;

        public PropertyChangeEventArgs( string propertyName, object oldValue, object newValue )
        {
            this.PropertyName = propertyName;
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
    }

    class UserPreferences
    {
        private static UserPreferences _instance;
        private Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public delegate void PropertyChangeHandler( object sender, PropertyChangeEventArgs data );
        public event PropertyChangeHandler PropertyChange;

        private UserPreferences()
        { 
        }

        public static UserPreferences Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UserPreferences();
                }

                return _instance;
            }
        }

        public bool PrettyPrint
        {
            get
            {
                return (bool)this.GetProperty("prettyprint", true);
            }
            set
            {
                this.UpdateProperty("prettyprint", value);
            }
        }

        private object GetProperty( string propertName, object defaultValue = null )
        {
            var value = this._localSettings.Values[propertName];
            if (value != null)
            {
                return value;
            }

            return defaultValue;
        }

        private void UpdateProperty( string propertyName, object value )
        {
            var oldValue = this.GetProperty(propertyName);
            this._localSettings.Values[propertyName] = value;

            this.OnPropertyChange(this, new PropertyChangeEventArgs(propertyName, oldValue, value));
        }

        private void OnPropertyChange(object sender, PropertyChangeEventArgs data)
        {
            if (this.PropertyChange != null)
            {
                this.PropertyChange(sender, data);
            }
        }
    }
}
