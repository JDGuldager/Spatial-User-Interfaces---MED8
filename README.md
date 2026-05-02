🎯 VR Throwing Interaction Project
Overview

This project explores improved throwing interactions in VR using Unity and OpenXR.
The goal is to compare Unity’s default throwing system with a custom-built solution designed to improve accuracy and usability.

The project was developed by Laurids, Jeppe, and Matilda.

YouTube Playthrough: https://www.youtube.com/watch?v=zg1tq0ce_X4

🎮 Features
Custom Throwing System
Crosshair-based aiming for improved precision
Velocity-based throwing with controlled scaling
Adjusted trajectory with slight upward arc
Horizontal correction to reduce misalignment
Default Throw Comparison
Uses Unity’s XR Interaction Toolkit grab interactable
Serves as baseline for evaluation
Force Pull Mechanic
Objects can be pulled toward the player by a quick hand motion
Inspired by “force pull” interactions
Haptics
Custom haptic feedback for different object types
Helps distinguish objects through tactile cues
🧪 Evaluation

The project includes a simple test setup:

Two modes:
Default XR throwing
Custom throwing system
Players hit targets within a time limit
Score is recorded and displayed

Results from testing showed that the custom throwing system made it easier to aim and hit targets compared to the default system.

🎯 Spawning & Scoring
Targets spawn randomly within a defined radius
A minimum distance prevents targets from spawning too close
When a target is hit, a new one is spawned immediately
A timer limits each round
Final score is used to compare throwing systems
🕹 Controls
Action	Input
Grab object	Trigger
Throw object	Release trigger
Recall object	Trigger press
Force pull	Quick arm movement while hovering
Reset game	B button
Return to menu	Y button
🛠 Requirements
Unity (Unity 6 / 2022+ recommended)
OpenXR enabled
VR headset (tested with Meta Quest 2 & 3)
📦 Scenes
Scene Selector – Choose between throwing systems
Default Scene – Unity XR throwing
Custom Scene – Improved throwing system
