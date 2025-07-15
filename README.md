# SmartTutor
<img width="1901" height="905" alt="image" src="https://github.com/user-attachments/assets/4c2442e8-9eb6-40b8-8b1c-a6dbd267f9c7" />

## Overview
SmartTutor is an educational platform designed to enhance learning through personalized experiences. This refactored version of an earlier project integrates artificial intelligence to adapt to individual learning needs, making education more engaging and effective for students and educators alike.

## Features
- **Personalized Learning**: Utilizes the DeepSeek API to tailor content and quizzes to user progress.
- **Adaptive Quizzes**: Dynamically adjusts difficulty based on user performance.
- **Progress Tracking**: Provides detailed insights into learning milestones.
- **Modular Architecture**: Built with an N-tier structure for scalability and maintainability.

<p align="center">
    <kbd>
        <img src="https://github.com/user-attachments/assets/a164f90a-7eec-4a47-80b8-ec8c61c52d45" alt="Courses Management" style="border: 10px solid black; padding: 10px; max-width: 20%; height: 350px;">
    </kbd>
    <p align="center"><em>Courses Management - Admin area</em></p>
</p>

<p align="center">
    <kbd>
        <img src="https://github.com/user-attachments/assets/9e3484e4-76f1-4b56-960f-187a56f3d14f" alt="Test Chapters" style="border: 10px solid black; padding: 10px; max-width: 20%; height: auto;">
    </kbd>
    <p align="center"><em>One chapter - One test</em></p>
</p>

<p align="center">
    <kbd>
        <img src="https://github.com/user-attachments/assets/8cbeb763-845e-4cd3-955c-53f24f986ddb" alt="AI answer analysis" style="border: 10px solid black; padding: 10px; max-width: 20%; height: 350px;">
    </kbd>
    <p align="center"><em>Example of AI Answer Analysis</em></p>
</p>



## Technologies
- **Framework**: ASP.NET Core, Blazor
- **AI Integration**: DeepSeek API
- **Database**: SQL Server with Entity Framework Core
- **Development Tools**: Visual Studio 2022
- **Version Control**: Git, GitHub

## Installation
1. Clone the repository: `git clone https://github.com/yourusername/SmartTutor.git`
2. Navigate to the project directory: `cd SmartTutor`
3. Restore dependencies: `dotnet restore`
4. Update the `appsettings.json` file with your SQL Server connection string, and AI API.
5. Run the application: `dotnet run`
