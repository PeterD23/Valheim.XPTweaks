# Valheim.XPTweaks
A mod for Valheim that does two things:

# Fishing Catch XP
Award XP if a fish is caught (reeled in fully)

If a fish is caught, the catch notification will now append the XP gain in brackets e.g "Perch Caught (1 XP)"

Catch XP Calculation:
`1 XP * (unaltered_escape_stamina_cost / 10) * quality_level`

# Multiple Target XP
Adds additional XP for multiple targets that are damaged by a weapon.

# How to set up
Get the BepinEx pack for Valheim, create a /Libs folder, and copy the following .dlls into it

![image](https://user-images.githubusercontent.com/10536628/224829086-61540ecc-cb97-4b15-b079-8b5c37199839.png)

These can be found at
BepInEx\core\0Harmony.dll
BepInEx\core\BepInEx.dll
valheim_Data\Managed\assembly_valheim.dll
unstripped_corlib\UnityEngine.dll
unstripped_corlib\UnityEngine.CoreModule.dll