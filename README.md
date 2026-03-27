# Cart Management System

This repository is a simple C# project for managing cart features. It follows a structured design with separate layers for business logic and data handling, making the code modular and easier to maintain.

## Key Features & Benefits

*   **Architecture**: The system is split into two parts: Business Logic `(CartManagement.BusinessLogic)` and Data Logic `(CartManagement.DataLogic)`. This keeps the code organized and reusable.
*   **Core Cart Management**: Uses `CartManager.cs` and `CartService.cs` to handle cart tasks like adding items, removing items, and calculating totals.
*   **Business Rule**: `CartRules.cs` applies rules and validations to keep data accurate and consistent with app policies.
*   **Exception Handling**: `CartExceptions.cs` manages custom cart errors, making problems easier to catch and debug.
*   **C# Language**: Written fully in C#, taking advantage of .NET’s performance and ecosystem.
*   **Separation of Concerns**: Program.cs is the entry point, clearly separated from business and data logic. This makes the system easier to read, test, and maintain.

## Prerequisites & Dependencies

To build and run this project, you will need the following tools and software:

*   **[.NET SDK](https://dotnet.microsoft.com/download)**: Version 6.0 or higher. This project is built using C# and requires the .NET SDK to compile and run.
*   **C# Development Environment**:
    *   **[Visual Studio](https://visualstudio.microsoft.com/downloads/)**: Recommended for a full-featured Integrated Development Environment (IDE) experience.

## Installation & Setup Instructions

Follow these steps to get the project up and running on your local machine:

1.  **Clone the Repository**:
    Open your terminal or command prompt and clone the repository using Git:
    ```bash
    git clone https://github.com/AlfredLim16/Cart-Management---Lim.git
    cd Cart-Management---Lim
    ```

2.  **Open the Solution**:
    *   **Using Visual Studio**: Open the `Cart Management/Cart Management.sln` file directly. Visual Studio will automatically load all projects within the solution.

3.  **Restore NuGet Packages**:
    The .NET SDK should automatically restore any necessary NuGet packages upon opening the solution or building. If you encounter issues, you can manually restore them from the solution root:
    ```bash
    dotnet restore
    ```

4.  **Build the Project**:
    Build the solution to compile all projects. This can be done via your IDE's build command (e.g., `Build > Build Solution` in Visual Studio) or via the command line:
    ```bash
    dotnet build
    ```

5.  **Run the Application**:
    Navigate to the main executable project directory and run it. Assuming `Cart Management` is the console application project:
    ```bash
    cd "Cart Management"
    dotnet run
    ```
    Alternatively, you can run directly from the solution root specifying the project:
    ```bash
    dotnet run --project "Cart Management/Cart Management.csproj"
    ```
    If using Visual Studio, you can simply press `F5` or the "Start" button to run the application in debug mode.

## License Information

This project is currently provided **without a specified license**.

This means that by default, all rights are reserved by the copyright holder, AlfredLim16. You should contact the owner directly for explicit permission if you wish to use, distribute, or modify this software.
