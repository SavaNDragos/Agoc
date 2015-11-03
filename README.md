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


Environment Definition xml:

Because usually the difference between some configuration files will lay in only a couple of variables we provide the capacity to define a collection of variables (the environemnt).


