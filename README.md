# Forge Game Data Editor

The Forge Game Data Editor, or FGD Editor for short, is a program that can edit Forge Game Data (FGD) configuration files. These files specify the list of entity classes available for a particular game running on the Half-Life 1 engine, also known as GoldSource. More information about the FGD file format can be found [here](https://developer.valvesoftware.com/wiki/FGD).

The editor currently supports only the Half-Life 1 FGD format grammar. This will be expanded to support newer versions of the format.

## Supported functionality

The FGD Editor currently supports the opening and saving of FGD files and the editing of the list of entity classes. Classes can be added, removed, edited and moved in the list and both the editor and keyvalues can be added, removed, edited and moved.

## Planned functionality

See the issues list and/or the projects board for more information on planned features.

## Requirements

Operating System: Windows 7 or newer

## Downloads

See the releases page for available downloads.

## Project information

The FGD Editor is written in C# 8 using Windows Presentation Foundation (WPF) Core and [Prism](https://prismlibrary.com/docs/) for the Model-View-ViewModel design. It runs on NET Core 3.1.

The project follows a common Prism architectural design where different components are separated into assemblies:
|Assembly Name | Purpose |
|--|--|
| FGDEditor | Contains the Application and MainWindow classes as well as startup logic |
| FGDEditor.Core | Defines region names used by modules as well as the main window |
| FGDEditor.Mvvm | Contains common MVVM functionality |
| FGDEditor.Services.Interfaces | Contains interfaces of services |
| FGDEditor.Services | Contains the implementations of service interfaces |
| FGDEditor.Services.Wpf | Contains WPF-specific service implementations |
| FGDEditor.Modules.GameDataEditor | Contains the user interface for the FGD editor |
| FGD | Contains the FGD lexer, parser, writer and AST functionality |

All assemblies have nullable reference types enabled.
