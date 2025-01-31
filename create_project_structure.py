import os

def create_directory_structure(root_dir):
    """Crea la estructura de directorios y archivos para el proyecto Magic Words."""

    directories = [
        "Assets/Art/Animations",
        "Assets/Art/Materials",
        "Assets/Art/Prefabs/UI",
        "Assets/Art/Prefabs/World",
        "Assets/Art/Sprites",
        "Assets/Art/Textures",
        "Assets/Resources/Dictionaries/en-EN",
        "Assets/Resources/Dictionaries/es-ES",
        "Assets/Resources/ScriptableObjects",
        "Assets/Scenes",
        "Assets/Scripts/Controllers",
        "Assets/Scripts/Data",
        "Assets/Scripts/Enums",
        "Assets/Scripts/Events",
        "Assets/Scripts/GameModes",
        "Assets/Scripts/Initialization",
        "Assets/Scripts/Interfaces",
        "Assets/Scripts/Managers",
        "Assets/Scripts/Models",
        "Assets/Scripts/ScriptableObjects",
        "Assets/Scripts/Views",
        "Assets/Tests/EditMode"
    ]

    files = [
        "Assets/Scenes/MainMenu.unity",
        "Assets/Scenes/Game.unity",
        "Assets/Scripts/Controllers/GameController.cs",
        "Assets/Scripts/Controllers/BoardController.cs",
        "Assets/Scripts/Controllers/DictionaryController.cs",
        "Assets/Scripts/Controllers/FirebaseController.cs",
        "Assets/Scripts/Controllers/MatchmakingController.cs",
        "Assets/Scripts/Controllers/PowerUpController.cs",
        "Assets/Scripts/Data/Cell.cs",
        "Assets/Scripts/Data/Board.cs",
        "Assets/Scripts/Data/Word.cs",
        "Assets/Scripts/Data/Player.cs",
        "Assets/Scripts/Enums/GameMode.cs",
        "Assets/Scripts/Enums/GameState.cs",
        "Assets/Scripts/Enums/PowerUpType.cs",
        "Assets/Scripts/Events/CellSelectedEvent.cs",
        "Assets/Scripts/Events/WordValidatedEvent.cs",
        "Assets/Scripts/GameModes/IGameMode.cs",
        "Assets/Scripts/GameModes/PvPGameMode.cs",
        "Assets/Scripts/GameModes/PvAlgorithmGameMode.cs",
        "Assets/Scripts/Initialization/GameInitializer.cs",
        "Assets/Scripts/Initialization/FirebaseInitializer.cs",
        "Assets/Scripts/Initialization/DictionaryInitializer.cs",
        "Assets/Scripts/Interfaces/IBackendService.cs",
        "Assets/Scripts/Interfaces/IDictionaryService.cs",
        "Assets/Scripts/Managers/GameManager.cs",
        "Assets/Scripts/Managers/InputManager.cs",
        "Assets/Scripts/Models/GameModel.cs",
        "Assets/Scripts/ScriptableObjects/GameConfig.cs",
        "Assets/Scripts/ScriptableObjects/BoardConfig.cs",
        "Assets/Scripts/ScriptableObjects/DictionaryConfig.cs",
        "Assets/Scripts/Views/GameView.cs",
        "Assets/Scripts/Views/MainMenuView.cs",
        "Assets/Scripts/Views/CellView.cs",
        "Assets/Scripts/Views/PlayerView.cs"
    ]

    # Crear directorios
    for directory in directories:
        os.makedirs(os.path.join(root_dir, directory), exist_ok=True)

    # Crear archivos vacíos
    for file in files:
        with open(os.path.join(root_dir, file), 'w') as f:
            pass  # Crea un archivo vacío

if __name__ == "__main__":
    project_root = "MagicWords"  # Define el directorio raíz del proyecto
    create_directory_structure(project_root)
    print(f"Estructura de directorios y archivos creada en '{project_root}'")