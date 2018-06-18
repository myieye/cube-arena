# CubeArena

CubeArena is a multiplayer, cooperative, augmented reality (AR) game for testing and comparing handheld devices (HHDs) and head-mounted devices (HMDs).

The game was developed for my Master's thesis (MSc in Software Engineering).

![Alt text](screenshot.png "Title")

## Concept

Each player has a set of cubes. Working together they stack their cubes and then tip them over to squash the bad guys! They can also change into "Spray Mode" to spray the bad guys and slow them down.

## User Interfaces

Seven different user interfaces (UIs) are included. Three are for HHDs, three for HMDs and one for PCs.

These UIs are largely based on a 3D interaction technique known as *pointing*. This simply means that users control a cursor that is moved through the AR environment using a raycast algorithm. This allows HHDs and HMDs to be compared using a similar interaction technique on both devices. On the other hand, two HHD UIs make use of touchscreen gestures to explore an input feature that is unique to HHDs, while two HMD UIs are largely based on hand gestures.

## Supported Devices

CubeArena is desgined for Android devices (specifically smartphones) and Microsoft's HoloLens.

## Technologies

CubeArena is a Unity project. AR was added using Vuforia.
