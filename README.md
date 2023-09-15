# Sable's Avatar Copier - Beta v0.1
Allows quickly copying all aspects of one avatar to another. This includes all components, 'attachables' like in-unity-added Game Objects such as weapons/cookies/prefabs/etc, and component's with hierarchy references such as PhysBoneCollider references. All with the ability to completely customize what will be copied/merged.

I get it, it's always been an absolute pain to re-add *everything* from one old avatar to a freshly imported avatar everytime you make a small change from Blender. Especially when there are absolutely zero changes needed to be made Unity-side. So, in-order to delay the onset of my inevitable collapse into insanity, I developed an Avatar Copier tool to make all those little changes with just the simple click of a button! Including the ability to see and modify everything that will be copied/merged over before you hit that button for those specific circumstances you may require extra precision.

## How to Use
Open the **Avatar Copier** window via `Window -> Sable Tool's Avatar -> Copier`.

Take the Avatar that you wish to copy to in the **Destination** field.
Take the Avatar that you wish to copy from in the **Source** field.

Press the **Create Copy** toggle to optionally create a completely new merged instance and leave the Destination input untouched.

Make adjustments if needed in the *Merge Details* tab.

Any potential issues will appear as warnings in the *Warning* tab.

Press the *Merge* or *Create Copy* button below the Avatar inputs to Merge

![AvatarCopier2](https://github.com/SableVII/SablesAvatarCopier/assets/143745362/9d96cf6e-78c9-4326-9bb6-09202b499263)
![AvatarCopier1](https://github.com/SableVII/SablesAvatarCopier/assets/143745362/a1918d09-1fb2-4e53-b551-ac87801f1e16)
![AvatarCopier3](https://github.com/SableVII/SablesAvatarCopier/assets/143745362/e0cbab21-e5f1-41f5-a465-c7214c8c3d16)


## What are Attachables
Attachables are any Game Object and its hierarchy that are attached to an Avatar's hierarchy. These are determined by comparing the selected Avatar's in-scene hierarchy with that of the Avatar's imported skeleton bones.

### An Example:
If you add a Cookie Game Object with a Mesh and hierarchy to the right hand of the Avatar while in Unity, the Cookie will be considered an Attachable with an attachment point of the avatar's right hand.

However, if you added the Cookie to the hierarchy inside of Blender and imported the .fbx into Unity with those changes, the cookie will not be considered as an attachable. The Ccokie in that case, would already exist as part of the avatar's imported skeletal hierarchy and therefore cannot be considered an Attachable. 

## Known Issues
- Various minor UX and UI Issues.
- A host of testing still required to squash very specific issues.
- Lack of Joint and Rigid Body support. (coming soon!)
- Lack of any information in the 'Help' tab category.
- Warning Symbols on each Warning in the 'Warning' tab category.
- Probally lots of speling mistakes.
- Better output message once avatar is successfully copied/merged.
- Bug that makes you unable use the rename-shortcut/function on any Game Object in the scene's hierarchy.
- A more concrete method of determining 'Attachables' that cannot, in extreme-edge cases, be potentially broken.
