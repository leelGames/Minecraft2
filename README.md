# Minecraft2 (AT)

Willkommen auf der offiziellen GitHub-Seite des "Minecraft 2" Projekts.

Das ultimative Cross Plattform 3D Openworld Sandbox Multiplayer Spiel
basierend auf dem Blockraster von MC mit mehr Realismus

## Grundprinzip

Grob eine 3D Kombination aus Minecraft und Terraria (und Teardown), mit
eigenem Grafikstil: Low-Poly mit Texturen aus Dreiecken.

Winkel im Mesh meist 45Grad als Theme

Gameplay soll Entwicklung der Menschheit von Steinzeit bis Neuzeit
entsprechen.

Blöcke verbinden sich (Marching Cubes) und werden abgerundet, Texturen
verbinden sich möglichst.

# Blöcke

Block: Simpler Würfelförmiger Minecraft Block

Terrain: Verbindet sich für minimale Faces (Marching Cubes) -> Für
Boden Erde, Gesteine, Großteil der Weltgeneration,

Custom Model: Eigenes 3D Modell -> Für Möbel, Technik etc.

Pipe: Verbindet sich mit anderen Pipes zu einer achteckigen Röhre ->
Für Baumstämme (breite und schmale Variante)

Rounded: Abgerundet an allen Ecken, verbindet sich -> Für z.b. Felsen

Wall, Slope: Halb und drittel Block, auf 3 Achsen platzierbar

Liquid: Wasser, Lava etc.

## Welt-generation

Entsprechend den diversen für Minecraft existierenden Mods

Landschaft möglichst groß und realistisch

Große, unterschiedliche Biome, auswählbar beim generieren

Möglichst realistische Geologie und Darstellung der Natur

Breite unendlich -> wie MC

Höhe ca. 2000 Blöcke -> wie Terraria 

Komplexe Dörfer und Strukturen mit Gegner und Loot

# Features

1 Block entspricht 2/3 Meter für bessere Proportionen -> wie in
Terraria

Realistische Wasser Physik -> wie in Terraria und MC Mods

Gefällte Bäume fallen um

(Form der (Block und Terrain) Blöcke kann bearbeitet werden -> wie
Hammer in Terraria, Ecken, oder Kanten werden abgetrennt)

Manche Blöcke können angemalt werden (pro Fläche)

Gegner nur unterirdisch und in anderen Dimensionen

Realistisches Crafting durch unterschiedliche Werkstätten, können als
Container verwendet werden, Autocrafting später möglich

Modere Einrichtung, Technik, Fahrzeuge, für Lategame und Kreativmodus

Mit vier Reifen, Lenkrad, Motor, Tank/Batterie lässt sich ein fahrbares
Auto bauen (Wird zu Objekt in Form des Aufbaus), auch Züge und Boote

## Gameplay

Beim Generieren der Welt kann man Ort (Klima, Biom) und Zeit (Steinzeit >
Schwer, Mittelalter > Mittel, Neuzeit > einfach) auswählen. -> Strukturen
und Gegner passen sich an.

Start wie -> RLcraft

Inventar:

Inventar hat 20 Slots (Upgradebar bis 100, Scrollbar), Stack bis zu 100
Blöcke, Items werden gruppiert, Hotbar hat 10 Slots für rechte Hand mit
Rechtsklick (Material), 5 Slots für linke Hand mit Linksklick, wird
automatisch ausgewählt (Werkzeuge), Items in Hotbar sind Referenz auf
alle im Inventar

Abgebaute Sachen landen direkt im Inventar

Kleidungslots (Schuhe, Hose, Jacke, Helm) Für Kleidung und Rüstung geben
entsprechende Effekte

Utilityslots (Karte, Fernglas, Rucksack)

Stufenlose Healthbar (10 - 100HP Upgradebar) keine Hungerbar weil essen
ist nervig -> Kein essen, keine Nutztiere

Energiebar: Sinkt bei Bewegung, lädt sich automatisch auf, wird durch
Consumables, Bewegung, schlafen beeinflusst, beeinträchtigt
Schnelligkeit, Regeneration

XP Level- und Multiplikator

## Entities

Items haben Rigidbody-Physik, despawnen nicht

Items generieren natürlich: Steine, Stöcke (für Anfang)

Animationen: Tür, Piston

VoxelEntitys (Bewegliche zusammengefasste Blöcke): Fahrzeuge, fallender
Baum

Spieler und Mobs

## Grafik

Nicht zu minecraftig, nicht zu realistisch

Shader mit Schattierung

Simples Raytracing?

PBR-Materials

LowPoly Texturen aus Dreiecken (generiert?)

Level of Detail für hohe Renderdistanzen:

Farchunks rendern nur das wichtigste und haben nur eine Farbe als Textur

(werden größer und bilden weniger Blöcke ab)

## Performance

Threaded Chunkloading, Subchunks

Dynamic Chunks für dynamische Blöcke (Wasser, Technik)

Entitychunks für zusammengebakte Entities 8\*8 normale Chunks

## Biome
Kombination von Höhe (1000m), Art(100m) und Klima(10000m)
(Übergang fließend)

Höhe:

-   -2: Tiefes Meer

-   -1: Meer

-   0: Strand

-   1: Flachland/ Dünen

-   2: Hügellandschaft/ Hochland

-   3: Mittelgebirge/ Schlucht

-   4: Hochgebirge/ Extremes Gebirge



Art:

-   Wiese

-   Wald

-   Tiefer Wald

-   Buschwald

-   Felslandschaft

-   Wüste

-   Strand

Klima:

-   Arktisch

-   Sub-arktisch

-   Mediterran

-   Subtropisch-trocken

-   Subtropisch-Feucht

-   Tropisch-trocken

-   Tropisch-feucht

Strukturen:

-   Baum

-   Busch

-   Felsen

-   See

-   Fluss

-   1block: Strauch, Gras, Stein, Stock, Baumstumpf

# Blöcke
(Ausschnitt)

Terrain:

-   Felsstein -> generiert oben

-   Tiefenstein -> Mitte

-   Lavastein -> unten

-   Gras

-   Moos

-   Waldboden

-   Grobe Erde

-   Gute Erde

-   Sand

-   Schnee

-   Eis

-   Kies

-   Geröll

-   Alle möglichen Gesteine die so auftauchen

-   Steinkohle

-   Braunkohle

-   Kupfer -> Technik

-   Lithium -> Technik

-   Silizium -> Technik

-   Zinn -> Metall lvl1

-   Eisen -> Metall lvl2

-   Aluminium -> Metall lvl3

-   Silber -> Metall lvl4

-   Gold -> Metall lvl5

-   Diamant -> Metall lvl6

Custom natürlich:

-   Blätter

-   Büsche

-   Sträucher

-   Gras

-   Spezielle Pflanzen

-   Steine

-   Stöcke

Block:

-   Holz -> auch wall, slope

-   Bruchstein -> auch wall, slope

-   Beton -> auch wall, slope, pipe

-   Stahl -> auch pipe

-   Blech -> auch wall, slope, pipe

-   Kunststoff -> auch wall, slope, pipe

-   Ziegel -> auch wall, slope

-   Pflasterstein -> auch wall, slope

-   Asphalt -> auch slope

Custom:

-   Alle Technik

-   Tür 1\*3

-   Fenster -> verbindet sich

-   Möbel

-   Dekoration

Liquid:

-   Offenes Wasser -> generiert natürlich, unendliche Quelle

-   Künstliches Wasser -> fließt unendlich, versickert

-   Schlamm

-   Lava

-   Erdöl

# Items

-   Alle Items Levelbar -> Metalllvl

-   Schwert -> Kampf

-   Speer -> Kampf

-   Bogen -> Kampf max: Gewehr

-   Axt -> Kampf, Holzabbau max: Kettensäge

-   Spitzhacke -> Steinabbau max: Bohrmaschine

-   Schaufel -> Erdabbau max: Bagger?

-   Schere -> Pflanzenabbau

-   Säge -> Bearbeiten, Holz

-   Hammer -> Bearbeiten, Stein

## Technik

Möglichkeit komplexe technische Systeme und Farmen zu erstellen -\> wie
Create Mod in weniger OP

Kabel: Wie Redstone auf Block, Spannung von 0 bis 255, unterschiedliche
Farben verbinden sich nicht, verliert keine Spannung pro Strecke,
Spannung addiert und teilt sich, bei Spannungsüberlauf brennen Bauteile
durch

-   Diode: lässt Strom nur in eine Richtung -\> wie Repeater ohne
    Verstärkung

-   Transistor: Wenn Spannung an Transmitter, wird Basisspannung an
    Emitter ausgegeben -\> AND-Gate, NAND-Modus bei Klick

-   Kondensator: gibt Spannung weiter, reduziert Spannung nach aus
    langsam -\> Delay

-   (Delayer: Verzögert Signal

-   Komparator: gibt Eingang 1 nur weiter, wenn größer Eingang 2)

-   Widerstand: Reduziert Spannung um die Hälfte

Komponenten können in alle Richtungen und an Wänden platziert werden

Outputs benötigen Mindestspannung und sind effektiver bei höherer

-   Motor min 16: Dreht Achsen und Blöcke, Geschwindigkeit nach
    angeschlossener Spannung

-   Pumpe min 32: bewegt Flüssigkeiten in Rohren nach oben und Gase nach
    unten

-   Lampe min1: Leuchtet nach Spannung, unterschiedliche Farben

-   7-Segment Display 0-15 nach Spannung (Auch Buchstaben, Unicode
    Anzeige)

-   (Piston min 16: Schiebt Blöcke Geschwindigkeit nach angeschlossener
    Spannung

-   Piston mit Bohrer min 64 zerstört Blöcke

-   Placer min 32: setzt Blöcke (Als VoxelEntity)

Inputs erzeugen Spannung

-   Knopf 1: kurzes Signal

-   Hebel 1: anhaltend

-   Input Panel 0-15 nach Input

-   Solarpanel 0-63 nach Tageszeit

-   Dynamo 0-255 nach Drehgeschwindigkeit

-   Kreativ only Hebel, Knopf: 255

Mechanisch:

Drehimpuls wird durch Achsen und Zahnräder weitergegeben auch 0-255

-   Mechanischer Piston: Schiebt vor und zurück nach Drehgeschwindigkeit

-   Fließband: Bewegt Items

-   Windrad: 8-63 nach Wetter

-   Wasserrad 32

Hydraulisch: Druck von Flüssigkeiten und Gasen in Rohren

-   Hydraulischer Piston: Schiebt Blöcke nach Druck, Linear bei Flüssig,
    Federnd bei Gas, höheres Pushlimit

-   Motor 32 - 255: erzeugt Drehung aus Öl

-   Dampfmaschine 1-64: erzeugt Drehung aus Gas

-   Ofen: Erzeugt Dampf nach Brennstoff 16-255

-   Behälter: leert sich, hält Druck konstant

## Marketplace

Das Spiel ist kostenlos, Monetarisierung durch Marketplace

Ingame Währung: Wird für echtes Geld gekauft und durch Achievements
verdient

Ausgeben für Server, Maps, Mods, Texturen, Shader, integrierter
Modloader

Content-Creator können vergütet, Server finanziert werden

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
04 2023             Fertigstellen des Textureditors, erste Texturen erstellen

05 2023             
pre-alpha 0.32:     Verwendung der neuen Texturen, PBR, Cleanes Projekt, zurück zu BuiltinRP
09 2023             Git Repository verwenden
                    neue Connect-Blöcke

# Bugs

VoxelEntitys drehen falsch

VoxelEntity Blöcke rotieren falsch

ChunkData funktioniert nicht richtig

Cutoff Performance

Wasser optimieren

Mesh Verbesserungen: Wasser, Terrain auf block

Slabs sin wieder komisch

# ToDo:

Pre-alpha:

-   Performance

-   Optimierte Meshcollider?

-   LODChunks optimieren (flat shading, UV)

-   Subchunks neu machen

-   Subchunks beim generieren

-   Fast Update (Update ohne collider, nur dynamische blöcke)

-   VoxelEntitys fixen

-   [Inventarsystem]{.underline}

-   [Worldgeneration]{.underline}

-   [Block-Combinatios]{.underline}

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
