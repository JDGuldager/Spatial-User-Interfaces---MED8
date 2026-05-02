# 🎯 VR Throwing Interaction Project

## 📖 Overview
This project explores improved throwing interactions in VR using Unity and OpenXR.  
The goal is to compare Unity’s default throwing system with a custom-built solution designed to improve accuracy and usability.

Developed by **Laurids, Jeppe, and Matilda**.

🎥 **YouTube Playthrough:**  
https://www.youtube.com/watch?v=zg1tq0ce_X4

---

## 🎮 Features

### 🎯 Custom Throwing System
- Crosshair-based aiming for improved precision  
- Velocity-based throwing with controlled scaling  
- Adjusted trajectory with a slight upward arc  
- Horizontal correction to reduce misalignment  

### 🔧 Default Throw Comparison
- Uses Unity’s XR Interaction Toolkit grab interactable  
- Serves as a baseline for evaluation  

### ✋ Force Pull Mechanic
- Pull objects toward the player with a quick arm movement  
- Inspired by “force pull” interactions  

### 🔊 Haptics
- Custom haptic feedback for different object types  
- Helps distinguish objects through tactile cues  

---

## 🧪 Evaluation

The project includes a simple test setup:

- Two modes:
  - Default XR throwing  
  - Custom throwing system  
- Players hit targets within a time limit  
- Score is recorded and displayed  

**Result:**  
The custom throwing system made it easier to aim and hit targets compared to the default system.

---

## 🎯 Spawning & Scoring

- Targets spawn randomly within a defined radius  
- Minimum distance prevents targets from spawning too close  
- Targets respawn immediately after being hit  
- Time-limited rounds  
- Final score is used to compare throwing systems  

---

## 🕹 Controls

| Action           | Input                              |
|------------------|-------------------------------------|
| Grab object      | Trigger                             |
| Throw object     | Release trigger                     |
| Recall object    | Trigger press                       |
| Force pull       | Quick arm movement while hovering   |
| Reset game       | B button                            |
| Return to menu   | Y button                            |

---

## 🛠 Requirements

- Unity (Unity 6 / 2022+ recommended)  
- OpenXR enabled  
- VR headset (tested with Meta Quest 2 & 3)  

---

## 📦 Scenes

- **Scene Selector** – Choose between throwing systems  
- **Default Scene** – Unity XR throwing  
- **Custom Scene** – Improved throwing system  

---

## 🚀 Summary

This project demonstrates how adding visual guidance, controlled velocity, and small trajectory adjustments can significantly improve throwing accuracy and overall user experience in VR.

---

## Use of AI
AI has been used in the process of creating this project. Namely, it has been used to
- Suggest and restructure code
- Strengthen the language and structure of the report
