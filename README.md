# SnappingToSurfaceTool-Unity 2021
 Tool in Unity, permitting to snap a game object position and rotation along a surface, using a tag. 

Made in Unity 2021.3.17 (but should work for higher version)

#Included
-This README with instruction
-Full unity project (can be opened directly with Unity Hub)
-Unity package, containing an exemple scene,as well as the script. 


#How it work:
-Assign the script to any object that you want to move along a surface (can be a group of object)
-In the script, precise the tag that you want to use as surface 
-Assign this tag to any surfaces you want to use. This surface need to have a collider
-If you want the object to align with the normal of surface, check the box "Align Rotation to Surface"
-Move it along! You have two gizmo, one at the base of the surface and the original of the object. 
-To increase the offset from surface, you can change it in inspector or use the white arrow.
-Ctrl-Z to cancel any movement

#Possible bug:
-If you delete a tag, it may induce a bug if it is still registered in the inspector of the script, creating a warning message in console. Recreate the tag or de-assign it in the inespector
-If two surfaces with the used tag are close together, the script will quickly change between the two. Adapt the Max Distance Detection, or remove the tag from one. 
-Big movement on small surfaces may break the snap, and teleport the object pretty far. I recommend using it on bigger objects only.
-No surface detecte: check if the surface you want to use is in range, with the correct tag and using a collider.


Lou de Tarade