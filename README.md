# Ongoing
A generative dungeon-delving game that I created as a capstone project for the CS50 Introduction to Games Development course from EDX & HarvardX: https://online-learning.harvard.edu/course/cs50s-introduction-game-development?delta=0

### Requirements
Unity 2020

### Overview

As a 2D RPG, *Ongoing* has a lot stylistic similarities to Assignments 6 (Zelda) and 8 (Pokémon). The lessons learnt during these activities were especially helpfully when putting together this project - specifically the work we did with *finite-state machines*.

The main challenge for this project was implementing a the procedurely generated mazes that form the floors of the dungeon. The algorithm that I used was inspired by this blog-post by Bob Nystrom from 2014: http://journal.stuffwithstuff.com/2014/12/21/rooms-and-mazes/ , the implementation in C# is entirely my own work. The dungeon generation code was the first part of the code-base that I developed, and I was still getting to grips with Unity while writing it, so it works but it isn't exactly pretty. If I continue developing this project, the next step will be to give this section of the code a good refactoring. 

WRT project requirements. There are three discrete game states (each is a discrete Unity scene); a start, a play and a game over state. Obviously a lot more could be done with this once additional game mechanics are designed.

While exploring options for storing data between scene transitions, I can across the ScriptableObjects features in Unity. This talk from the Unite 2017 in Austin was a big source of inspiration for me when creating my data structures, events system, and audio player: https://www.youtube.com/watch?v=raQ3iHhE_Kk .
