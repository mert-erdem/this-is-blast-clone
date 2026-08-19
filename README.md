# This is Blast! Clone

This project is a small Unity clone of the hybrid-casual puzzle game **This is Blast!**. It focuses on the main gameplay loop: loading level data, placing target blocks, managing cannon queues, moving cannons into slots and shooting matching colored blocks.

## Project Info

- **Unity version:** 6000.3.20f1
- **Render pipeline:** Universal Render Pipeline (URP)
- **Levels:** 4 demo levels

## Tech Used

- Unity 6
- C#
- URP
- DOTween
- JSON

The project also uses a few simple patterns to keep the gameplay code organized: singletons for core managers, object pools for optimization, event-driven communication between systems and data-driven level setup.
## Opening the Project

1. Install **Unity Hub**.
2. Install **Unity 6000.3.20f1**.
3. Clone or download this repository.
4. Add the project folder from Unity Hub.
5. Open the project with Unity 6000.3.20f1.
6. Open `Assets/Scenes/Loading.unity`.
7. Press **Play**.

Note: In the Editor, if you want to replay a level without continuing from the last saved state, delete `Assets/Game/Resources/save.json`.

## Folder Overview

- `Assets/Scenes`: Loading, Menu and Game scenes.
- `Assets/Game/Scripts`: Gameplay logic, managers, data classes, entities, object pools, and UI.
- `Assets/Game/Resources`: Level JSON files and the runtime save file.
- `Assets/Game/Prefabs`: Board, cannon, projectile, target block and UI prefabs.
- `Assets/Game/Materials`: Board, projectile and block materials.
- `Assets/Game/ScriptableObjects`: Shared configuration assets, such as block color setup.

## Data and Gameplay Flow

Levels are written as JSON files under `Assets/Game/Resources`. On start, `LevelManager` loads the current level file and converts it into plain data classes.

From there, the data is passed into the gameplay systems. `Board` turns target block data into pooled target block objects and places them on the grid. `CannonManager` turns cannon data into pooled cannon objects and arranges them into queues. When the player selects a cannon, `CannonSlotManager` moves it into an available slot.

Once a cannon is slotted, `ShootingController` checks the board for a matching front block. If a matching target is available, the cannon fires until it runs out of ammo or there are no valid targets left.

## Saving

The save system is intentionally simple. `SaveManager` writes the current runtime state into `Assets/Game/Resources/save.json`.

The save file keeps the current level, remaining target blocks, cannon queues and occupied cannon slots. When the game starts, `LevelManager` checks for this save file first. If it exists, the level is restored from the saved state. If not, the original level JSON is used.

Progress is saved when the application is paused or closed. The save is cleared when the player loses or finishes the current level.
