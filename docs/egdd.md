# Terminal Hacker

## Elevator Pitch

You’re stuck in a dead-end CS job and enroll in an underground hacking course to make some extra cash, and finally afford that hair transplant. Dropped into a virtual system, you navigate directories and rooms using terminal commands, uncover hidden files, and ride elevators deeper into a tangled network. Your mission: collect valuable data and sell it off. But the system is watching. An anger meter tracks your mistakes and presence, growing more volatile the longer you stay. Solve mini-puzzles, find the data-rich files, and escape.

## Influences (Brief)

- Deadeye Deepfake Simulacrum
  - Medium: Game
  - Explanation: This game features a decently in-depth hacking system that focuses on the aspects of programming we want to focus on: directory traversal on a command line.
- The Matrix
  - Medium: Movie
  - Explanation: Our game is Terminal Hacker, meaning the player is someone who is hacking into a system to escape people and do hacker things. When I first heard the concept of the game, the movie The Matrix came to mind, as it is about a person being transported into a cyber world, where the main character is essentially a hacker of that world.
- Anonymous
  - Medium: Hacker Group 
  - Explanation: Anonymous is known for using hacking to expose corruption and injustice. This inspired Terminal Hacker, where players must outsmart security systems to achieve their goals.

## Core Gameplay Mechanics (Brief)

You write terminal commands to traverse a virtual file system, searching for hidden data to steal and sell. But the deeper you go, the more unstable the system becomes. An anger meter tracks your mistakes and presence, growing more volatile over time. To stay ahead, you'll need to solve puzzles and avoid detection. Each level has complex layouts, limited time, and trickier file structures.

   Traverse directories and digital rooms

   Avoid system detection as the anger meter rises

   Solve puzzles to collect data and/or avoid detection

   Complex Levels with puzzles


# Learning Aspects

## Learning Domains

Introduction to directory and file traversal and management

## Target Audiences

- Early learners of computer science who have little experience with directory and file traversal and management.

## Target Contexts

- This would be played when you are learning basic directory and file management

## Learning Objectives

*Remember, Learning Objectives are NOT simply topics. They are statements of observable behavior that a learner can do after the learning experience. You cannot observe someone "understanding" or "knowing" something.*

- *Executing Files*: *By the end of the game, players will know how to run files located inside directories.*
- *Creating Files*: *By the end of the game, learners demonstrate the ability to create new files in specified directories.*
- *Traversing Directories*: *By the end of the game, players can navigate through different bash directories.*

## Prerequisite Knowledge

- *Before the game, players need to be able to differentiate between files and directories*
- *Before the game, players need to be able to understand how files and directories work*

## Assessment Measures

*We will assess the learner with a pre/post-test, allowing them to demonstrate their understanding of the learning objectives. We will compare their scores in each test to measure their understanding.*

# What sets this project apart?

- The gameplay of having to “dungeon crawl” through a terminal-style world
- The game focuses on teaching players how cmd/terminal works, with fun gameplay mechanics 
- The game features unique system-driven effects, like corruption, visual glitches, and misleading paths, that actively work against the player’s success.
- This game will have fun and unique endings that the players can achieve

# Player Interaction Patterns and Modes

## Player Interaction Pattern

*Terminal Hacker is a single-player game. You traverse through the world using the WASD movement. You interact with objects/prompts that will pop up a keyboard, allowing you to type in bash commands. You will repeatedly go through harder and harder levels the deeper you go.*

## Player Modes

- *Singleplayer*: *The only mode of the game will be available from the main menu with a simple start option. In this mode, the player will be met with a few difficulty levels where they have to try and traverse the directory and find a specific file.*

# Gameplay Objectives

- *Steal data*:
    - Description: *The player must find and collect secure data hidden throughout the level.*
    - Alignment: *This teaches players how to navigate directories and manage files.*
- *Don’t get caught*:
    - Description: *There will be a firewall (progress bar) that will be hunting you down. You have to write in bash commands/ solve puzzles to throw them off.*
    - Alignment: *This aligns with the player to create new files/directories*
- *Advance to the next level*:
    - Description: *Once the player reaches the end of the level, you will progress to the next level* 
    - Alignment: *This will help them advance their knowledge of directory traversal and  

# Procedures/Actions

*You can traverse the overworld with the WASD keys and interact with terminals by moving to them and pressing Tab. From there, you will have access to various bash commands, which can be freely typed into the terminal. You can also interact with files and elevators by pressing E*

# Rules

*What resources are available to the player that they make use of?  How does this affect gameplay? How are these resources finite?*

- Players move their character with WASD
- LS command will open the map and show you the layout of the dungeon
- LS -a will reveal hidden files within a room, this will have limited uses
- LS -l shows the timestamps of files on the map
- CD can be used in elevators to navigate to different floors (directories)
- RM will allow you to delete files to remove debuffs or to slow down the firewall, this will have limited uses
- VIM Will allow you to edit files to change the text to solve puzzles, apply a buff, and hvae a chance at increasing your score
- PWD will show you the current path of the directories you are in
- The goal of the game is to find the file with the secret data to expose the person you are hacking

# Objects/Entities

- There will be a player model that looks like a hacker
- There will be a firewall progress bar
- There will be dungeon-like rooms acting as the directories
- There will be rooms acting as files
- There will be a terminal displayed with a keyboard on screen that you can type into

## Core Gameplay Mechanics (Detailed)

- *Editing Files and Directories*: *The player will be able to perform commands at terminals that allow them to edit and create files, including moving them between directories or eve removing them entirely.*
- *Hazards*: At the left side of the screen, players can monitor the alert meter—once it fills up, its over, you get caught and sent to jail. Randomly players may encounter trap files raise suspicion. Staying calm under pressure is key, as the system actively works against you the longer you linger.
- *Searching for Core Data*: *The player will have to manage their more limited commands to find the core data they are looking for.*

    
## Feedback

- There will be a walking noise for when your character is moving around
- There will be a command line prompt on screen that will allow you to type in commands
- Advancing to a new level will play a noise and visually transition
- When you win there will be a winning audio and a you won screen
- If you get caught by the firewall there will be a game over screen and there will be defeat audio 


# Story and Gameplay

## Presentation of Rules

The player will learn each of their possible actions at terminals one at a time. The pressure of trap files and firewalls will be added slowly and only ramped up as the player learns better ways to deal with them. We will allow them a few calm levels to figure out the basics before the difficulty increases.

## Presentation of Content

*The player will learn new commands as they complete levels, learning bit by bit instead of all at once. For instance, they’ll learn how to use ls and cd first, since those are very bare bones ways to get around in directories. Then they will be taught things like mv, rm, or touch as they have to deal with the firewall more. Each command will be taught one at a time, and assigned a mechanical purpose in the context of the game to make memorizing the command and how it works more engaging.*

## Story (Brief)

*You’ve been trapped in the digital realm and have to find the key pieces of data to code a way out of here before the firewall burns you to a crisp!*

## Storyboarding


# Assets Needed

## Aesthetics

*The aesthetics should be sort of electronic or mechanical, with a sort of digital look to everything. This plays into the game's theme of terminal hacking in a sort of computer world and will hopefully immerse the player in the game and make them feel more invested.*

## Graphical

- Characters List
  - Hacker -  This is the player-controlled character. The player will be able to customize their hair
  - Firewall -  The Anger Meter that fills up slowly 
- Textures:
  -  Circuit board, retro computer theme 

- Environment Art/Textures:
  - Background -  The background will be similar to the insides of the computer(lots of wire, sensors, etc)
  - Doors - Double doors that are colored similarly to the background
  - Elevators - Elevators will look almost like a teleportation pad, and kinda function that way too


## Audio


*Game region/phase/time are ways of designating a particularly important place in the game.*

- Music List (Ambient sound)
  - *Title Screen*: *https://freesound.org/people/RICHERlandTV/sounds/351920/ (OPening the Game/Loading into a Level)*, *https://freesound.org/people/orginaljun/sounds/396960/ (Title Music)*
  - *General Gameplay*: *https://freesound.org/people/Timbre/sounds/406915/ (Background Music)*, *https://freesound.org/people/B_Lamerichs/sounds/262834/ (general)*
  
*Game Interactions are things that trigger SFX, like character movement, hitting a spiky enemy, or collecting a coin.*

- Sound List (SFX)
  - *Opening a terminal*:*https://freesound.org/people/Debsound/sounds/256543/ (Opening the terminal)*, *https://freesound.org/people/unfa/sounds/543968/ (Running a Command)*, *https://freesound.org/people/FoolBoyMedia/sounds/352661/ (extra)*
  - *Entering a Room*: *https://freesound.org/people/tim.kahn/sounds/91926/ (Elevator)*, *https://freesound.org/people/Pixeliota/sounds/678254/ (File Door Opening/Enterning)*
  - *Death*: * https://freesound.org/people/AceOfSpadesProduc100/sounds/360871/ *


# Metadata

* Template created by Austin Cory Bart <acbart@udel.edu>, Mark Sheriff, Alec Markarian, and Benjamin Stanley.
* Version 0.0.3


