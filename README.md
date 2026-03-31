# 🧠Unity Systems Architecture Showcase
This repository contains a collection of gameplay systems and architecture patterns I use to build scalable and maintainable games in Unity.

These systems are extracted and simplified from real production scenarios, including my shipped game Sophie: Starlight Whispers.

The goal is to demonstrate how I design systems, not just that I can implement features.
# Key Highlights
- Built with modularity and scalability in mind
- Focus on decoupled architecture and clean communication between systems
- Uses SOLID principles and practical design patterns
---
# 🧩Systems Examples

## Event Bus - Decoupled Communication
A type-safe event system that allows independent modules to communicate without direct references.

👉 **Start here:** `Game Systems Design/Assets/Scripts/Utils/GameEventBus.cs`

- Strongly typed events using generics
- Eliminates tight coupling between systems
- Centralized lifecycle management (Clear on scene reload)
- Based on Observer Pattern
## Resident Evil Inventory Inspired
A simple grid inventory inspired on Resident Evil inventory mechanic, with drag and drop feature, options on click and space management.

👉 **Start here:** `Game Systems Design/Assets/REInventory/Scripts/InventoryCore.cs`

- Fully based on interfaces so it can be easily extended and modified
- Communicates to UI using the **Event Bus**
- Pure C# classes so it can be easily tested
---
# Architecture Philosophy
I design systems with a few core principles:

- **Decoupling first** → Systems should not depend on each other directly
- **Extensibility** → New features should not require rewriting old code
- **Clarity over cleverness** → Code should be easy to understand and maintain
- **Production-oriented** → Built for real games, not just examples
# Background
These patterns were applied in the development of _Sophie: Starlight Whispers_, a metroidvania-style game where I was responsible for:

- Full gameplay architecture
- Core systems (combat, AI, progression)
- Custom tools and debugging workflows
---
# Contact
If you'd like to discuss these systems or opportunities:

- [Portfolio](https://miguel-minotti-portfolio.vercel.app)
- Email: miguelminotti.mm@gmail.com
- [LinkedIn](https://www.linkedin.com/in/miguel-francisco-minotti-dos-santos-8a8722208/)
