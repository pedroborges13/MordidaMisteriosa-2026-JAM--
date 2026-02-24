# 2 Reais ou uma Mordida Misteriosa? (“2 Reais or a Mysterious Bite?”) (2026-Game Jam) 
Developed for the CRTL ALT Jam. Theme: "Everything must, but not everything can" (Tudo deve, mas nem tudo pode)
![JumpscareVideo](https://github.com/user-attachments/assets/e6fe93d4-5656-4bcd-abea-a0a60a3d0c1f)

This game was developed in just 4-5 days of active work during a 9-day Game Jam, due to team schedule conflicts. It is a suspense simulation where you receive a gift box. However, what is inside might be a friendly pet… or a dangerous predator.
The core gameplay revolves around resource management and careful observation. You must use your limited Action Points to interact with the box and gather clues before making a final guess. But be careful: every interaction increases the animal's Stress Level. If it gets too high, the animal might flee or attack!

## 👥 The Team: 
- Pedro Borges: Programming and Game Design.
- Kauê Uriel (@kau_joaquim): Environment Modeling, Textures, and UI Design.

---

## 🎮 Game Overview:
- Tech Stack: Unity, C#, GitHub
- Platform: PC

---

## Technical Features
As the Programmer and Game Designer, I focused on creating a scalable system using modern Unity practices:
- ScriptableObjects: Used to create a flexible data architecture:
  - **Animal System:** I implemented an abstract [AnimalData](Assets/Scripts/Data/AnimalData.cs) class, with specific [DogData](Assets/Scripts/Data/DogData.cs) and [SnakeData](Assets/Scripts/Data/SnakeData.cs) subclasses. This allowed us to easily tweak animal traits like Size and Temperament via Enums.
  - **Action System:** Created an [ActionData](Assets/Scripts/Data/ActionData.cs) ScriptableObject to define player interactions (Water, Hand, Shake). Each action carries its own data, such as Action Point cost and Action Type (enum), allowing for new gameplay mechanics to be added by simply creating a new asset in the editor without touching the code.
- Event-Driven Architecture: To keep UI and Game Logic separated, I used Events. The [RoundManager](Assets/Scripts/Manager/RoundManager.cs) triggers events for UI updates, stress changes, and feedback messages.
- Manager Pattern: The game is controlled by specialized managers:
  - [RoundManager](Assets/Scripts/Manager/RoundManager.cs): Handles the "logic loop", including animal spawning, action costs, and the method Reaction() system based on switch-statements.
  - [GuessManager](Assets/Scripts/Manager/GuessManager.cs): Checks if the player's guess matches the animal's characteristics and manages the "Clipboard" UI logic.
  - [GameManager](Assets/Scripts/Manager/GameManager.cs), [UIManager](Assets/Scripts/Manager/UIManager.cs), [AudioManager](Assets/Scripts/Manager/AudioManager.cs): Standard singleton-based managers for global states and feedback.

---

## What Could Be Improved
Due to limited development time, some areas could be improved:
- [RoundManager](Assets/Scripts/Manager/RoundManager.cs) accumulated too many responsibilities. The reactions should be handled in a separate system.
- I used some direct Singleton calls (especially for calling AudioManager). While functional for a Jam, with more time, this could be refactored into a cleaner communication.
- Polishing: Due to time constraints, features like background music, volume controls, and more varied box interactions were left out.

---

## 🔗 Links
 Itch.io page: pedrooborges.itch.io/2-reais-ou-uma-mordida-misteriosa
 -![AnimalVideo](https://github.com/user-attachments/assets/22c95884-8285-4862-a075-85331741d14e)
