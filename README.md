A simple script loader for The Last of Us

This code represents a basic script loader called "Tlou Mod Loader" designed for The Last of Us game. It allows users to load and activate custom scripts (DLL files) to enhance their gaming experience. The Mod Loader provides a convenient way to manage and enable/disable various scripts easily.

Upon execution, the program checks for administrator privileges and ensures it is run with administrative rights. It sets the current directory to the location of the program's assembly and initializes the console interface.

The program looks for a configuration file called "Config.ini" to retrieve the game's path. If the file is not found or if the Scripts directory is missing, the program displays appropriate error messages and terminates.

Next, the Mod Loader loads the list of scripts from the "scripthook.ini" file. It also scans the Scripts directory for any new DLL files and adds them to the list. The updated list of scripts is then saved back to the INI file.

After verifying the game process is running, the Mod Loader injects the enabled scripts into the game using the ProcessHack class. If successful, it displays a success message for each injected script. If no scripts are activated, it counts and displays the number of available scripts.

Additionally, the program integrates with Discord Rich Presence and displays a presence status indicating the number of loaded scripts in The Last of Us.

To exit the program and close the game, the user can press the Enter key. The Mod Loader terminates the game process and exits gracefully.

In summary, Tlou Mod Loader is a lightweight and user-friendly tool for loading and managing custom scripts in The Last of Us, enhancing the game's functionality and providing an enriched gaming experience.
