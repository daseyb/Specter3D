Spriter2Unity
=============

Tool that converts SCML files into unity .prefab and .anim files
Based on the original work by Malhavok (https://github.com/Malhavok/Spriter2Unity)

About
=====

What it does:
- for each entity in SCML file creates a Unity prefab
- each entity got it's sprites assigned as long as you imported these sprites before conversion
- for each animation in SCML file creates a Unity animation under the prefab
- sprites are changed during animations, so only 1 SpriteRenderer is used for each node in the spriter file
- a Spriter2Unity component - the CharacterMap - is attached to the prefab to look up 

Limitations (and why):
- Only quadratic, cubic, linear and instant curves are supported
- Mainline curves are not yet imported

Usage
=====

Copy the contents of the Assets folder into your Unity project's Assets folder or import the package from (https://github.com/bonus2113/Spriter2Unity/raw/master/Package/Spriter2Unity.unitypackage)
Any SCML files copied into the project will be automatically processed. Prefabs are created in the same folder as the SCML file.
