# Acre's Legacy – Memory Revival 🏛️📱

An Augmented Reality (AR) mobile application that brings the historical heritage of **Acre (Akko)** to life through immersive storytelling, interactive animations, audio narration, and gamification.

## 📖 About the Project

**Acre's Legacy – Memory Revival** is a mobile AR application developed to enhance the way visitors experience the historical sites of Acre.

Acre is one of the most historically significant cities in the region, with a history shaped by civilizations including the Canaanites, Crusaders, Mamluks, and Ottomans. Although the city is internationally recognized for its cultural heritage, many of its historical stories and lesser-known sites remain underexplored.

This project uses **Augmented Reality, GPS-based location services, interactive storytelling, and gamification** to transform a traditional sightseeing experience into an interactive historical journey.

The application allows users to explore historical locations, watch AR and non-AR historical animations, listen to narrated stories, answer quizzes, and collect badges as they progress through the experience.

---

## 🎯 Project Goals

The main goals of the project are to:

* 🏛️ Preserve and communicate Acre's historical heritage.
* 📍 Encourage visitors to explore lesser-known historical locations.
* 🥽 Use AR to connect historical stories with their real-world locations.
* 🎧 Provide audio and textual historical information.
* 🎮 Increase engagement through quizzes and a badge system.
* 🌍 Encourage tourism and cultural exploration in Acre and Northern Israel.
* 📚 Create a more interactive alternative to traditional heritage interpretation methods.

---

## ✨ Main Features

### 🗺️ Interactive Map

The application provides an interactive map of Acre's Old City with marked historical locations.

GPS functionality allows the application to determine the user's location and provide location-based experiences.

### 🥽 Augmented Reality Experiences

Users can experience historical stories through AR animations displayed in the real-world environment.

The AR experiences are designed around the historical context of each location.

### 🎬 Non-AR Experiences

Users who are unable or unwilling to use AR can still access historical animations describing the story associated with a location.

This provides an alternative way to experience the content.

### 🎧 Audio & Text Narration

Historical information is presented through:

* Audio narration
* Textual descriptions
* Historical storytelling
* Visual animations

The application is designed to support narration in multiple languages.

### 🎮 Gamification

The application uses a badge-based progression system to encourage exploration.

Users can earn:

| Badge     | Achievement                                     |
| --------- | ----------------------------------------------- |
| 🥉 Bronze | Complete the site's historical animation        |
| 🥈 Silver | Successfully complete the site's quiz           |
| 🥇 Gold   | Complete the exploration of all supported sites |

After viewing a site's historical experience, the user unlocks its quiz. Successfully answering all questions awards the user with the site's silver badge.

Collecting all site-specific badges allows the user to achieve the final gold badge.

### 🧠 Historical Quizzes

Each supported location includes a quiz based on the historical information presented in its experience.

The quizzes are designed to reinforce the information users encounter during the AR or non-AR experience.

---

## 🏛️ Supported Heritage Sites

The implemented project focuses on outdoor historical locations in Acre, including:

* **Tower of Flies**
* **Khan Al-Hameer**
* **Acre's Eastern Wall Cannons**

Each location has its own historical narrative and interactive experience.

The project is designed so that additional heritage sites can be incorporated in the future.

---

## 🏗️ System Architecture

The application follows a **three-layer architecture**:

```text
┌──────────────────────────────────────┐
│         Presentation Layer           │
│                                      │
│  Map • UI • AR Scenes • Animations   │
│  Quizzes • Badges • Audio • Text     │
└──────────────────┬───────────────────┘
                   │
┌──────────────────▼───────────────────┐
│          Application Layer            │
│                                      │
│ Scene Management • GPS • AR Logic    │
│ Quiz Logic • Badge System • Progress │
└──────────────────┬───────────────────┘
                   │
┌──────────────────▼───────────────────┐
│             Data Layer                │
│                                      │
│ Assets • Audio • Scripts • Models    │
│ Scenes • Cached Content              │
└──────────────────────────────────────┘

              Infrastructure
        ┌──────────────────────┐
        │ Vuforia + ARCore     │
        │ Camera / AR Tracking  │
        └──────────────────────┘
```

### Presentation Layer

Responsible for the application's visual interface and user interaction, including:

* Acre's map
* Historical site menus
* AR and non-AR scenes
* Quizzes
* Badge displays
* Audio and textual content

### Application Layer

Contains the core application logic, including:

* Scene transitions
* GPS/location handling
* AR animation control
* User progression
* Quiz management
* Badge management
* Interaction between the UI and data

### Data Layer

Manages the application's content and assets, including:

* Historical scripts
* Audio files
* Textual information
* Badges
* 3D models
* Scenes
* AR-related assets
* Cached content

---

## 🛠️ Technologies & Tools

| Technology         | Purpose                         |
| ------------------ | ------------------------------- |
| **Unity 6**        | Application and game engine     |
| **Vuforia Engine** | Augmented Reality functionality |
| **ARCore**         | Android AR support              |
| **C#**             | Application scripting           |
| **GPS**            | Location-based functionality    |
| **Unity UI**       | User interface                  |
| **3D Assets**      | Historical visualization        |
| **Audio Assets**   | Historical narration            |

### Target Platform

* **Android smartphones**

---

## 🔄 Application Workflow

The basic user flow is:

```text
Launch Application
        │
        ▼
   Acre Map
        │
        ▼
Select Historical Site
        │
        ▼
   Site Experience
      /      \
     /        \
    ▼          ▼
   AR       Non-AR
Animation   Animation
    \          /
     \        /
      ▼      ▼
   Bronze Badge
        │
        ▼
      Quiz
        │
        ▼
   Correct Answers
        │
        ▼
   Silver Badge
        │
        ▼
All Sites Completed
        │
        ▼
    Gold Badge
```

---

## 🎮 Gamification System

The badge system encourages users to explore multiple historical sites rather than interacting with only one location.

### Progression

**1. Discover a site**

The user selects a historical location from the map.

**2. Experience the story**

The user watches either the AR or non-AR historical animation.

**3. Earn a Bronze Badge**

Completing the historical experience awards the site's bronze badge.

**4. Unlock the Quiz**

The site's quiz becomes available after the experience is completed.

**5. Earn a Silver Badge**

Answering all quiz questions correctly awards the site's silver badge.

**6. Complete the Journey**

After collecting the required site badges, the user receives the final gold badge.

---

## 📍 Location-Based Experience

GPS functionality is used to connect the digital experience with Acre's physical environment.

The application tracks the user's location when exploring the relevant area of Acre and uses location information to support the site's exploration experience.

This allows historical information to be presented in the context of the physical place where the story occurred.

---

## ⚙️ Development Approach

The project was developed using the **Design Thinking methodology**.

The development process included:

1. Problem identification
2. Background and literature research
3. User and stakeholder research
4. Requirements definition
5. System design
6. Prototype development
7. Implementation
8. Testing and evaluation
9. Iteration and refinement

The project also incorporated knowledge from local stakeholders and historical guides to help shape the application's historical narratives.

---

## 📊 Evaluation

The implemented system was evaluated using several measures related to usability, engagement, and historical learning.

Reported evaluation results include:

* **AR experience:** 3.07 / 4
* **Average badges earned:** 3.07 / 4
* **Badge completion:** approximately 77%
* **Expert heuristic evaluation:** 4 / 5

User feedback highlighted the interactive map and the feeling of engagement with Acre's historical locations.

These results were used to assess the application's usability and the effectiveness of its interactive heritage experience.

---

## 📂 Project Structure

A simplified structure of the Unity project is:

```text
Acre's-Legacy/
│
├── Assets/
│   ├── Scenes/
│   ├── Scripts/
│   ├── Models/
│   ├── Animations/
│   ├── Audio/
│   ├── Images/
│   ├── UI/
│   └── Resources/
│
├── Packages/
│
├── ProjectSettings/
│
└── README.md
```

> The exact folder structure may vary depending on the current Unity project version and development environment.

---

## 🚀 Getting Started

### Requirements

To open and develop the project, you will need:

* Unity 6
* Android development support
* Android-compatible device
* Vuforia Engine
* ARCore-compatible Android device for AR functionality

### Installation

1. Clone the repository:

```bash
git clone <repository-url>
```

2. Open **Unity Hub**.

3. Select **Add Project** and choose the cloned project folder.

4. Open the project using the required Unity version.

5. Ensure Android Build Support is installed.

6. Configure the required Vuforia and ARCore settings.

7. Connect a compatible Android device.

8. Build and run the application on the device.

---

## 📱 Using the Application

1. Launch the application.
2. Explore the Acre map.
3. Select a historical site.
4. Read or listen to the historical information.
5. Experience the AR animation when available.
6. Alternatively, watch the non-AR animation.
7. Earn the site's bronze badge.
8. Complete the site's quiz.
9. Earn the silver badge.
10. Continue exploring other locations.
11. Complete the required sites to unlock the gold badge.

---

## 🔮 Future Improvements

Possible future development includes:

* Adding more historical sites in Acre.
* Expanding the number of historical stories.
* Adding additional languages.
* Improving GPS accuracy.
* Adding more advanced AR interactions.
* Expanding the gamification system.
* Adding more quizzes and challenges.
* Improving accessibility features.
* Adding richer historical 3D reconstructions.
* Improving offline functionality.
* Expanding the application to additional heritage cities.

---

## 🎓 Academic Project

**Project:** Acre's Legacy – Memory Revival
**Project Code:** 25-2-D-10
**Type:** Capstone Project
**Platform:** Android
**Technology:** Unity 6 + Vuforia + ARCore

### Authors

**Basel Haddad**
**Abed Amar**

### Advisor

**Dr. Naomi Unkelos-Shpigel**

---

## 📄 Project Documentation

The repository contains supporting project documentation covering:

* Problem background
* Research and literature review
* System architecture
* Development methodology
* Engineering decisions
* Implementation
* Evaluation
* Testing
* Lessons learned
* User guide
* Maintenance information

---

## 📜 License

This project was developed as an academic capstone project.

Unless otherwise specified, the project's source code, assets, historical content, models, animations, and other materials should be considered part of the academic project and should not be redistributed or reused without permission from the project authors.

---

## ❤️ Acknowledgements

We would like to thank our advisor, **Dr. Naomi Unkelos-Shpigel**, and the local stakeholders and guides whose knowledge and feedback contributed to the development of the historical narratives and overall project concept.

---

**Acre's Legacy – Memory Revival**
*Bringing Acre's history to life through augmented reality.*
