import os

# Lista de rutas de carpetas a crear dentro de "Assets"
directories = [
    "Assets/Scripts/Core/Managers",
    "Assets/Scripts/Core/Services",
    "Assets/Scripts/Core/Utilities",
    "Assets/Scripts/Features",
    "Assets/Scripts/Features/Words",     # Ejemplo de un módulo/feature
    "Assets/Scripts/Features/Levels",    # Otro ejemplo de módulo/feature
    "Assets/Scripts/UI",
    "Assets/Scenes",
    "Assets/Plugins",
    "Assets/Resources"
]

for dir_path in directories:
    os.makedirs(dir_path, exist_ok=True)
    print(f"Created folder: {dir_path}")
