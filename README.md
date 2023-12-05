# ![AppIcon](https://github.com/M1S2/Ehrungsprogramm/raw/master/Icons/AppIcon.png) Ehrungsprogramm

[![GitHub Release Version](https://img.shields.io/github/v/release/M1S2/Ehrungsprogramm)](https://github.com/M1S2/Ehrungsprogramm/releases/latest)
[![GitHub License](https://img.shields.io/github/license/M1S2/Ehrungsprogramm)](https://github.com/M1S2/Ehrungsprogramm/blob/master/LICENSE.md)


Programm zum Verwalten von Ehrungen des TSV Illertissen.
Über das Ehrungsprogramm können Stammdaten aller TSV Mitglieder eingelesen und weiter ausgewertet werden. Näheres zum Dateiformat im Abschnitt [Dateiformat Mitgliederstammdaten](#dateiformat-mitgliederstammdaten).
Zu den Stammdaten gehören beispielsweise der Name, alle bereits erhaltenen Ehrungen oder die Funktionen der Personen im Verein.
Auf dieser Grundlage werden alle fälligen Ehrungen (BLSV und TSV) errechnet und dargestellt.

## Seiten Beschreibung
Nachfolgend werden kurz die Funktionen der jeweiligen Seite der Anwendung beschrieben:
- **Start:** Startseite der Anwendung. Von hier sind alle anderen Seiten erreichbar. Diese Seite ist gleichwertig zum Menü auf der linken Seite.
- **Personen:** Kurze Übersicht aller eingelesenen Personen inklusive Name, Eintrittsdatum und eventuell verfügbaren Ehrungen. Über die Suchleiste kann die Liste nach Namen, Punkten oder dem Eintrittsdatum gefiltert werden. Fährt man mit der Maus auf eine Person, können über den Info-Knopf nähere Details über die Person angezeigt werden. Durch einen Klick auf die Spaltenüberschriften wird die Liste nach der Spalte sortiert.
- **TSV Ehrungen:** Übersicht aller TSV Ehrungen. In den Listen werden die Namen, Punkte und Abteilungen der Personen aufgeführt, für die eine entsprechende Ehrung aussteht. Jede Person wird nur bei der *höchsten verfügbaren Ehrung* angezeigt. Fährt man mit der Maus auf eine Person, können über den Info-Knopf nähere Details über die Person angezeigt werden. Durch einen Klick auf die Spaltenüberschriften wird die Liste nach der Spalte sortiert. Am rechten Rand können bestimmte Ehrungen ausgeblendet werden.
- **BLSV Ehrungen:** Übersicht aller BLSV Ehrungen. In den Listen werden die Namen, Punkte und Abteilungen der Personen aufgeführt, für die eine entsprechende Ehrung aussteht. Jede Person wird nur bei der *höchsten verfügbaren Ehrung* angezeigt. Fährt man mit der Maus auf eine Person, können über den Info-Knopf nähere Details über die Person angezeigt werden. Durch einen Klick auf die Spaltenüberschriften wird die Liste nach der Spalte sortiert. Am rechten Rand können bestimmte Ehrungen ausgeblendet werden. Mit dem Schiebeschalter können nur die BLSV25 und BLSV40 Ehrungen ausgewählt werden.
- **Datenbank verwalten:** Hier können die Grunddaten eingestellt bzw. geladen werden, mit der die Anwendung arbeitet. Über den Knopf "Aus Datei importieren" kann eine Datei mit den Stammdaten der Mitglieder eingelesen werden. Außerdem besteht hier die Möglichkeit, die letzte Datei nochmals zu laden (falls es Änderungen in der Datei gab) oder die geladenen Daten zu löschen. Der Stichtag wird als Bezugstag für alle Berechnungen verwendet. Hier sind auch Statistiken zur geladenen Datei zu finden.
- **Einstellungen:** Hier kann das Design der Anwendung eingestellt werden (Hell, Dunkel oder Standard). Bei Standard wird die Windows Einstellung übernommen. Außerdem ist hier die Version ersichtlich.
- **Person Details:** Diese Seite ist nicht direkt erreichbar. Wird auf der Seite "Personen", "TSV Ehrungen" oder "BLSV Ehrungen" der Info-Knopf einer Person angeklickt, gelangt man auf diese Seite. Es werden alle geladenen Stammdaten (Name, Mitgliedsnummer, Geburtsdatum, Eintrittsdatum, Abteilungen, Funktionen) und berechnete Daten (effektive Jahre in den Funktionen, Ehrungen, Punkte) angezeigt.

<img src="https://github.com/M1S2/Ehrungsprogramm/raw/master/Screenshots/Screenshot_Start.png" width="45%"></img>
<img src="https://github.com/M1S2/Ehrungsprogramm/raw/master/Screenshots/Screenshot_Persons.png" width="45%"></img>
<img src="https://github.com/M1S2/Ehrungsprogramm/raw/master/Screenshots/Screenshot_TSV_Rewards.png" width="45%"></img>
<img src="https://github.com/M1S2/Ehrungsprogramm/raw/master/Screenshots/Screenshot_BLSV_Rewards.png" width="45%"></img>
<img src="https://github.com/M1S2/Ehrungsprogramm/raw/master/Screenshots/Screenshot_ManageDatabase.png" width="45%"></img>
<img src="https://github.com/M1S2/Ehrungsprogramm/raw/master/Screenshots/Screenshot_Settings.png" width="45%"></img>
<img src="https://github.com/M1S2/Ehrungsprogramm/raw/master/Screenshots/Screenshot_PersonDetails.png" width="45%"></img>

Über den Info-Knopf im rechten oberen Eck können detailliertere Informationen über die Anwendung angezeigt werden. Außerdem kann hier auf eine neuere Version der Anwendung aktualisiert werden (wenn verfügbar).

![AppInfoButton](https://github.com/M1S2/Ehrungsprogramm/raw/master/Screenshots/Screenshot_AppInfoButton.png)

## Dateiformat Mitgliederstammdaten
Momentan wird für den Datei Import eine .csv Datei unterstützt, die aus dem Mitgliederverwaltungsprogramm ProWinner exportiert werden kann. Diese Datei muss die Funktionen (inklusive Zeiträumen) sowie alle Ehrungen (inklusive Datum und Bezeichnung) enthalten.

Die Kopfzeile der .csv Datei hat dabei idealerweise folgendes Format:
```
"Name, Vorname";"Mitgliedsnummer";"Geb.Datum";"Eintritt am";"Abteilungen";"Funktionsname";"von -";"bis";"Ehr.Nr.";"Ehr.dat.";"Ehrungsbezeichnung";"Eintritt am";"Funktionsname";"Funktion von";"Funktion bis";"Ehrungs-Nr";"Ehrung am";"Ehrungsname"; ...
```

Folgende Spalten müssen dabei unbedingt vorhanden sein:
- "Name, Vorname"
- "Eintritt am"
- "Funktionsname", "von -", "bis" mindestens jeweils 1x
- "Ehr.Nr.";"Ehr.dat.";"Ehrungsbezeichnung" mindestens jeweils 1x

Alle anderen Spalten sind nicht zwingend notwendig. Kritische Fehler in der Datei werden direkt beim Einlesen angezeigt. Die bisher existierenden Daten bleiben in diesem Fall bestehen.
Alle anderen Fehler, die in der Datei bestehen, werden mit Ausrufezeichen hinter den jeweiligen Personen dargestellt.
Alle Fehler sollten in ProWinner korrigiert werden und nochmals exportiert werden. Ist die neue, korrigierte Datei vorhanden, kann lediglich über "Letzte Datei neu laden" auf der Seite "Datenbank verwalten" alles neu geladen werden.

## Druckfunktion
Auf den Seiten "Personen", "TSV Ehrungen", "BLSV Ehrungen" und "Personen Details" gibt es im rechten oberen Eck einen Knopf, über den eine Übersichtsseite erstellt werden kann. Der Druck erfolgt in eine PDF Datei, für die beim Drucken der Pfad ausgewählt werden kann.
Um die Übersichtsseite auf Papier auszudrucken, muss die PDF Datei in einem geeigneten Programm geöffnet und gedruckt werden.

![PrintButton](https://github.com/M1S2/Ehrungsprogramm/raw/master/Screenshots/Screenshot_PrintButton.png)

## Berechnung der Funktionszeiträume
Die effektiven Jahre als Vorstand, Abteilungsleitung oder in anderen Funktionen werden folgendermaßen berechnet:
- Von allen Funktionen werden die Zeiträume als Vorstand abgezogen.
- Von allen Funktionen werden die Zeiträume als Abteilungsleitung abgezogen.
- Bei allen anderen Funktionen werden überlappende Zeiträume bereiningt (nur eine Funktion pro Zeitpunkt wird bewertet).

![Konzepte Berechnung Zeiträume](https://github.com/M1S2/Ehrungsprogramm/raw/master/Screenshots/Konzept_Berechnung_Zeitraeume.jpg)

## Gespeicherte Einstellungen
Die Anwendung speichert die eingelesen Stammdaten in einer Datenbank im Anwendungsverzeichnis ("Ehrungsprogramm_Persons.db").
Hier werden außerdem der Pfad der letzten eingeladenen Datei sowie der eingestellte Stichtag gespeichert und beim nächsten Start der Anwendung wieder geladen.

## AppIcon
AppIcon from https://www.clipartmax.com/middle/m2i8N4G6i8Z5Z5H7_medal-scalable-vector-graphics-icon-cartoon-medal-vector/