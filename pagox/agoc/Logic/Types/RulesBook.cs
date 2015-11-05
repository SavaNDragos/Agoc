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
    public partial class RulesBook
    {

        private RulesBookRules[] rulesField;

        private RulesBookRule[] ruleField;

        private RulesBookRuleColection ruleColectionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Rules")]
        public RulesBookRules[] Rules
        {
            get
            {
                return this.rulesField;
            }
            set
            {
                this.rulesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Rule")]
        public RulesBookRule[] Rule
        {
            get
            {
                return this.ruleField;
            }
            set
            {
                this.ruleField = value;
            }
        }

        /// <remarks/>
        public RulesBookRuleColection RuleColection
        {
            get
            {
                return this.ruleColectionField;
            }
            set
            {
                this.ruleColectionField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RulesBookRules
    {

        private string importRuleBookField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ImportRuleBook
        {
            get
            {
                return this.importRuleBookField;
            }
            set
            {
                this.importRuleBookField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RulesBookRule
    {

        private string[] equalField;

        private string nameField;

        private string descriptionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Member", IsNullable = false)]
        public string[] Equal
        {
            get
            {
                return this.equalField;
            }
            set
            {
                this.equalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RulesBookRuleColection
    {

        private RulesBookRuleColectionApplyRule[] applyRuleField;

        private string nameField;

        private string descriptionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ApplyRule")]
        public RulesBookRuleColectionApplyRule[] ApplyRule
        {
            get
            {
                return this.applyRuleField;
            }
            set
            {
                this.applyRuleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RulesBookRuleColectionApplyRule
    {

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }



}
