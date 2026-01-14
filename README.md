# Fade to Grey - Unity Project

## Title
Fade to Grey - Unity Project

## Description
Fade to Grey is a serious game prototype where **Energy = Color**. The player moves through a top-down world that desaturates as their mental energy drains. Stressors in the environment act as obstacles that accelerate exhaustion, while energy pickups provide brief relief and restore visual vibrancy. The visual and audio feedback loops reinforce the lived experience of mental fatigue.

## Features
- Project structure builder that creates the required folders and starter scenes.
- Rigidbody2D top-down player controller with energy-scaled speed and input delay at low energy.
- EnergySystem core logic with events and configurable drain/restore rates.
- URP ColorAdjustments desaturation tied directly to energy (Energy = Color).
- Audio low-pass filtering that muffles sound as energy drops.
- Three obstacle types: moving, pulsing, and homing stress orbs.
- Energy pickup collectibles with audio feedback.
- Smooth 2D camera follow.
- Energy UI display with color-coded feedback.
- SceneLoader utility for menu and flow control.

## Folder Structure
```
Assets/
  Scenes/                Scene assets (menu, levels, UI)
  Scripts/               Gameplay and system scripts
    Player/              Player control and camera follow
    Obstacles/           Obstacle behavior scripts
    Managers/            Game systems (energy, UI, scene loading)
    Audio/               Audio filtering and mix control
    Effects/             Post-processing and visual feedback
  Prefabs/               Reusable prefab assets
    Player/              Player prefab(s)
    Obstacles/           Obstacle prefabs
    Collectibles/        Energy pickup prefabs
  Art/                   Art assets
    Characters/          Characters and sprites
    Environment/         Tiles, props, and backgrounds
    UI/                  UI graphics
  Materials/             Materials and shader assets
  Animations/            Animation clips and controllers
  Audio/                 Audio clips and mixer assets
```

## Scripts Overview

### ProjectStructureBuilder.cs
**Purpose:** Builds the full folder layout and placeholder scenes from the editor menu.
**How it works:** Adds missing folders under `Assets/` and creates empty scene assets in `Assets/Scenes`.
**How to extend:** Add more folders or scene names to the `FolderPaths` or `SceneNames` arrays.

### PlayerController.cs
**Purpose:** Provides top-down movement with energy-driven speed and input delay.
**How it works:** Reads raw input, applies SmoothDamp when energy is low, and uses MoveTowards to accelerate/decelerate via Rigidbody2D.
**How to extend:** Add sprint modifiers, camera-relative movement, or a stamina cooldown system.

### EnergySystem.cs
**Purpose:** Central energy model for drain, restore, and event dispatch.
**How it works:** Tracks energy in 0-100, drains while moving, applies obstacle damage, restores from pickups, and fires `OnEnergyChanged`.
**How to extend:** Add regeneration over time or integrate narrative events that modify energy.

### ColorManager.cs
**Purpose:** Desaturates the world as energy drops using URP ColorAdjustments.
**How it works:** Subscribes to `OnEnergyChanged` and lerps saturation from -50 to 0, smoothing updates per frame.
**How to extend:** Add additional post-processing cues (contrast shift, vignette, film grain).

### AudioEnergyFilter.cs
**Purpose:** Applies a low-pass filter that muffles audio as energy decreases.
**How it works:** Maps energy to an exposed mixer cutoff value with clear sound above 50 energy.
**How to extend:** Add reverb or spatial effects at critical thresholds.

### ObstacleBase.cs
**Purpose:** Base obstacle behavior with energy damage on trigger.
**How it works:** OnTriggerEnter2D locates the player EnergySystem and applies `energyDamage`.
**How to extend:** Add optional knockback or status effects in derived classes.

### MovingObstacle.cs
**Purpose:** Horizontal moving obstacle representing recurring stress.
**How it works:** Oscillates along the X axis using a sine wave.
**How to extend:** Add vertical or path-based movement.

### PulseObstacle.cs
**Purpose:** Visual stress pulse that expands and contracts.
**How it works:** Scales its transform over time using a sine wave.
**How to extend:** Animate color intensity or sync with audio cues.

### StressOrb.cs
**Purpose:** Homing obstacle that slowly pursues the player.
**How it works:** Uses Rigidbody2D velocity steering toward a target transform.
**How to extend:** Add predictive movement or flee behaviors based on distance.

### EnergyPickup.cs
**Purpose:** Restores energy and provides audio feedback on pickup.
**How it works:** On trigger, restores energy, plays a one-shot clip, and destroys itself.
**How to extend:** Add visual effects or timed buffs.

### CameraFollow2D.cs
**Purpose:** Smooth 2D camera follow for top-down scenes.
**How it works:** Uses Vector3.SmoothDamp in LateUpdate to track the player.
**How to extend:** Add dead zones, camera shake, or dynamic zoom.

### EnergyUI.cs
**Purpose:** UI readout for energy with color-coded feedback.
**How it works:** Updates TMP text and color whenever energy changes.
**How to extend:** Add a bar, pulse effects, or warnings at low energy.

### SceneLoader.cs
**Purpose:** Loads scenes by name, restarts, or loads the next scene.
**How it works:** Wraps SceneManager calls with convenience methods.
**How to extend:** Add fade transitions or async loading.

## Gameplay Overview
- **Movement:** Rigidbody2D-based top-down movement with smooth acceleration and deceleration.
- **Energy Drain:** Moving drains energy continuously; obstacles add extra damage.
- **Color Shift:** As energy drops, saturation decreases, reflecting mental exhaustion.
- **Obstacles:** Moving, pulsing, and homing hazards represent different stressors.
- **Relief:** Energy pickups restore color and temporarily relieve audio muffling.

## Technical Setup

### Unity Version
- **Unity 6000.3.4f1** (as recorded in `ProjectSettings/ProjectVersion.txt`).

### URP ColorAdjustments Setup
1. Ensure the project uses URP.
2. Create a Volume in the scene and add a **Color Adjustments** override.
3. Assign the Volume to `ColorManager`.
4. Enable the Saturation override on the Color Adjustments component.

### Script Assignment
- **Player GameObject:** `PlayerController`, `Rigidbody2D`, `EnergySystem` (or keep EnergySystem on a manager object if preferred).
- **Camera:** `CameraFollow2D` with `target` set to the player.
- **Global Manager:** `ColorManager`, `AudioEnergyFilter`, `SceneLoader`.
- **UI Canvas:** `EnergyUI` with a TMP text reference.
- **Obstacles:** `MovingObstacle`, `PulseObstacle`, or `StressOrb` attached to a trigger collider.
- **Pickups:** `EnergyPickup` on a trigger collider.

### Wiring Energy Events
1. Set the **EnergySystem** reference on `ColorManager`, `AudioEnergyFilter`, and `EnergyUI`.
2. Verify the EnergySystem is active at scene start so it can broadcast the initial value.
3. If references are left empty, scripts will attempt auto-find, but explicit wiring is recommended.

## How to Add New Content

### Add New Obstacles
1. Create a new script that inherits `ObstacleBase`.
2. Implement `ApplyBehavior()` for the new movement or effect.
3. Expose tuning values with `[SerializeField]` and document them with XML comments.
4. Create a prefab in `Assets/Prefabs/Obstacles`.

### Add New Levels
1. Create a new scene in `Assets/Scenes`.
2. Add it to **Build Settings** in the desired order.
3. Use `SceneLoader.LoadNext()` to move through the sequence.

### Add New SFX
1. Import clips into `Assets/Audio`.
2. Add or update an AudioMixer with exposed parameters.
3. Assign clips to `EnergyPickup` or route through the mixer for filtering.

### Modify the Color System
1. Update the saturation range in `ColorManager`.
2. Adjust `saturationSmoothTime` for faster or slower transitions.
3. Add additional URP overrides (vignette, bloom) for stronger emotional cues.

## Contributing Guidelines
- Keep all scripts documented with XML comments and organized with regions.
- Use descriptive names for fields and methods; avoid unclear abbreviations.
- Do not hardcode magic numbers without exposing them in serialized fields.
- Maintain the 0-100 energy scale across systems for consistency.
- Comment any non-trivial math or gameplay logic.
- Prefer clear, minimal dependencies between systems to keep iteration fast.
