# Minecraft2 (AT)

Willkommen auf der offiziellen GitHub-Seite des "Minecraft 2" Projekts.

Das ultimative Cross Plattform 3D Openworld Sandbox Multiplayer Spiel
basierend auf dem Blockraster von MC mit mehr Realismus


# Versionshistorie

2020 Idee

08 2021             Minecraft in Unity nach Tutorial

09 2021             Experimente mit Custom Mesh (failed)

(Erstes Semester)

03 2022             Experimente mit Marching Cubes nach Tutorial

pre-alpha 0.01:     Marching Cubes Terrain

04 2022 

pre-alpha 0.02:     Shader nach Tutorial

pre-alpha 0.03:     Blocktypen Terrain, Block und Custom

pre-alpha 0.04:     Generierung von Bäumen, allgemeine Optimierungen

pre-alpha 0.05:     Verbesserung des Playercontrollers und Collider

pre-alpha 0.10:     Threaded Chunkloading

(Zweites Semester)

05 2022 

Tests mit URP und UV-Mapping (failed)

pre-alpha 0.11:     Highlight Verbesserungen und Optimierungen

08 2022 

pre-alpha 0.12:     Generierung von Biomen

pre-alpha 0.13:     Half- und Thirdslabs, dynamisches Highlight

09 2022 

pre-alpha 0.14:     Subchunks und weitere Optimierungen

Weitere Tests mit UV-Mapping und PBR (Sieht nicht gut aus)

pre-alpha 0.15     Transparenzshader und Qualitätsverbesserungen

pre-alpha 0.16:     Pipe und Ocean Implementierung

pre-alpha 0.17:     Slope Implementierung

pre-alpha 0.18:     Rotierbare Customs und Refractoring

10 2022 

pre-alpha 0.20:     Upgrade auf URP, Verwendung von Shadergraph, Post-processing

(Drittes Semester)

11 2022 

pre-alpha 0.21:     BlockEvents, Wasserphysik, Chunkloading Optimierung

Experimente mit Umbau der Engine für Farchunks, VoxelEnitys

12 2022 

pre-alpha 0.22:     VoxelEntitys, Rounded als invertiertes Terrain

01 2023 

pre-alpha 0.23:     Neuimplementierung des Highlights, Hitboxen, Slope-slabs

02 2023 

pre-alpha 0.24:     Umbau der Engine, Thinpipe, rekursive Bäume

03 2023 

pre-alpha 0.30:     Distant LODs, LOD-Chunks, subchunks temorär entfernen

pre-alpha 0.31:     VoxelEntity Verbesserungen, unendliche Welt

Erstellung eines Textureditors

(Viertes Semester)

04 2023             

Fertigstellen des Textureditors, erste Texturen erstellen

05 2023 

pre-alpha 0.32:     Verwendung der neuen Texturen, PBR, Cleanes Projekt, zurück zu BuiltinRP

09 2023 

Git Repository verwenden

pre-alpha 0.33     Connect-Customslabs, neue Blöcke und Texturen, Hotbar und Kreativinventar

10 2023

pre-alpha 0.34     Umbau des BlockEvent-Systems, BlockUpdate-Thread, Verbesserung des Playercontrollers, allgemeine Optimierungen

## Bugs

- VoxelEntitys platzieren sich falsch rotiert

- SlopeSlab-Voxelentitys platzieren nicht

- Löcher im Mesh (bei Block auf Terrain, umschlossene Customs)

- Stetig steigender Speicherverbrauch bei vielen Chunkupdates

## ToDo

-   Blöcke und Texture hinzufügen

-   Weltgeneration überarbeiten
    
-   Performance Verbesserungen (Shader, Generierung...)

-   Voxelentity-Connectregeln verbessern

-   Meshcollider optimieren

-   LODChunks optimieren (flat shading, UV)

-   Subchunks neu machen (Cubic chunks?)

-   Raycast Chunkupdates

-   Lösung für fehlerfreie Connect-Blöcke finden

## Roadmap:

Pre-alpha:

-    Blocktypen

-    Block Events

-    Lod

-   Fast Update (Update ohne collider, nur dynamische blöcke)

-   Inventarsystem

-   Worldgeneration

-   Block-Combinatios

Alpha:

-   Menüsystem

-   Welt speichern

-   Items

-   Texturen

-   Grafik

Beta:

-   Survival-Gameplay

-   Strukturen

-   Technik

-   Fahrzeuge

Release:

-   Multiplayer

## In ferner Zukunft

Zweiter Teil: Fotorealistische Grafik ideal an die echte Welt angepasst
Weltgeneration, extremer Realismus (Verwendung von AI, Unreal Engine)
