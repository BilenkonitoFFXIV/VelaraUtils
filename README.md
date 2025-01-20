# Velara Utils  
FFXIV Dalamud plugin for automation, QoL utilities, and reverse engineering research.  

## Goals  
* **Automating test scenarios while reverse engineering game systems**. I spent the entirety of Shadowbringers and Endwalker reverse engineering the game, so I needed a way of not having to manually repeat the same inputs every time I made a small change to an unknown game variable.  
* **Theorycrafting silly meme rotations against the dummy in my backyard**. I have a neurological problem that affects my hand coordination, which makes precisely and consistently inputting actions quite hard. I wanted a way to experiment with new rotation ideas without my health getting in the way. I never played nor intended to play endgame content, and much less while using this plugins' automation features.  
* **Satisfying my morbid curiosity as to what was possible by modifying the game**. I like the rush of uncovering the systems that make the game work, to the point that I spent 3 literal months of playtime with a debugger attached while my character slept in my appartment's couch.  

## Status  
This plugin hasn't been touched for many years as I stopped playing FFXIV in 2022.  

All AOB patterns are very much outdated.  
Even if you update the Dalamud API to current versions, it's very likely that the game will immediately crash to desktop on startup.  

## Disclaimers  
Some of the included automation features are quite nasty and go directly against Dalamud's ethics guidelines. **Those guidelines exist for a reason.**  

Square Enix "doesn't care" about client modifications as long as they don't interfere with other players and the topic is not discussed in-game. This plugin can very much interfere with other players' experience in the wrong hands.  

Don't be an asshole and respect other players' right to enjoy the game without your interference. **Bothering other players with unwanted shenanigans is cringe.**  

>[!CAUTION]  
>**You will most likely get banned** if you use this plugin in PvP or competitive PvE.
>
>**I am not liable for anything you do with the knowledge contained in this repository.**  

#### Game Scripts  
I have intentionally omitted from this repository a lot of code related to the game's native Lua quest scripting system as the things you can do with it are absolutely deplorable.  
I'm talking teleporting to the end of dungeons, noclipping around, placing housing objects outside plot boundaries, moving bosses outside their intended areas, etc.  
I will take that cursed code to the grave, don't ask for it.  

#### Macros  
The extended macro functionality is pretty egregious as it allows for complete rotation automation without any kind of player input.  
**Don't use it during regular gameplay.**  
