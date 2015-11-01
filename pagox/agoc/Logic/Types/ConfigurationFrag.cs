using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agoc.Logic.Types
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class ConfigurationFrag
    {

        private ConfigurationFragFragment[] fragmentField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Fragment")]
        public ConfigurationFragFragment[] Fragment
        {
            get
            {
                return this.fragmentField;
            }
            set
            {
                this.fragmentField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConfigurationFragFragment
    {

        private string importConfigurationFragField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ImportConfigurationFrag
        {
            get
            {
                return this.importConfigurationFragField;
            }
            set
            {
                this.importConfigurationFragField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }


}
