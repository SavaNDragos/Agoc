# Agoc
Automatic generator of configuration files

Objective

Avoid 'the ogre work of' manually mantaining a large number of configuration files. (even if it consists only in search and replace in a number of files).
Create a virtual environment that provides the needed variables to specialize configuration files.
Have the capacity to specialize the environments of variables.

Common concepts:

Configuration Fragment xml:

To avoid mantaining a large number of configuration files we will use 'Common Fragments'. They will be able to be shared between the desired configuration files we want to obtain.
For reusability the 'Common Fragments' can be imported from other files.

Example:

<ConfigurationFrag>
  <Fragment>
    job {
  </Fragment>
  <Fragment ImportConfigurationFrag="../${Location}scmDefinition.xml" applyRule="MuscelLocation"/>
  <Fragment ImportConfigurationFrag="../scmDefinition.xml" />
  <Fragment ImportConfigurationFrag="../scmDefinition.xml"/>
  <Fragment>
    }  
  </Fragment>
</ConfigurationFrag>

We will have from this configuration fragment job { + what lies in: ${Location}scmDefinition.xml, scmDefinition.xml, scmDefinition.xml + the last fragment }.


Environment Definition xml:

Because usually the difference between some configuration files will lay in only a couple of variables we provide the capacity to define a collection of variables (the environemnt).

In our previous case to get the fragment that lies in ${Location}scmDefinition.xml we need to know variable ${Location}. This variable will sit in the environment definition xml.

Example:

<EnvironmentDef> 
  
  <Property Name="Location" Value="London" />
  <Property Name="Cluster"  Value="HighEnd" />
  <Property Name="LocationAdmin" Value="admin_${Location}"/>
  <Import FileLocation="../CommonEnvironment.xml" />
  <Property Name="ClusterLanguage"  Value="HighEnd_${allLanguages}" />
  <Import FileLocation="../General.xml" />
  
</EnvironmentDef>

We can see that what we want is LondonscmDefinition.xml.

Note: Environemnts are created by inheriteding the properties from upper environments like  CommonEnvironment.xml. in case the upper environment has the same variable we will override it with the one from the bottom environemnt. Those we provide the capacity of specialization.

Some properties can be influenced by other properties so after we extract all the environments variable, we will then go and resolved the ones that can be resolved. Example: <Property Name="ClusterLanguage"  Value="HighEnd_${allLanguages}" /> This can only be resolved if there is an evironemnt variable ${allLanguages} in CommonEnvironment.xml or in General.xml. If it can be resolved we will let it like it is (there can be engines that process them ex: groovy).




