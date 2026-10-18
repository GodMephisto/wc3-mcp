---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src/i18n/locales/de.json"
ref: HEAD
---

{
  "app": {
    "name": "BLP Converter",
    "tagline": "Schnelle BLP- und PNG-Konvertierung für World of Warcraft-Texturen",
    "description": "Ein schlankes Desktop-Tool zum Konvertieren von Blizzard-BLP-Texturdateien nach PNG und zurück. Dateien auf die linke (BLP) oder rechte (PNG) Seite ziehen, die Konvertierung läuft parallel. Ausgaben landen neben den Quelldateien, sofern kein anderer Ordner in den Einstellungen gewählt wurde.",
    "madeBy": "Erstellt von {{author}}",
    "dropToConvert": "Dateien zum Konvertieren ablegen",
    "converting": "Konvertiere…"
  },
  "panels": {
    "toPng": {
      "hint": ".blp-Dateien oder Ordner ablegen",
      "subhint": "Exportiert transparente PNGs"
    },
    "toBlp": {
      "hint": ".png-Dateien oder Ordner ablegen",
      "subhint": "Kodiert mit Ihren Kompressionseinstellungen"
    }
  },
  "activity": {
    "title": "Aktivität",
    "clear": "Leeren",
    "empty": "BLP- oder PNG-Dateien ablegen, Ausgaben werden standardmäßig neben den Originalen gespeichert."
  },
  "settings": {
    "title": "Einstellungen",
    "close": "Schließen",
    "language": "Sprache",
    "outputFolder": "Ausgabeordner",
    "outputFolderHint": "Optional. Leer lassen, um neben den Originalen zu speichern.",
    "outputDefault": "Gleicher Ordner wie Quelle (Standard)",
    "resetOutput": "Auf Quellordner zurücksetzen",
    "compression": "PNG -> BLP Kompression",
    "compressionRaw": "Unkomprimiert / RAW",
    "compressionDxt1": "DXT1 (deckend oder 1-Bit-Alpha)",
    "compressionDxt3": "DXT3 (scharfe Alphakanten)",
    "compressionDxt5": "DXT5 (weiches Alpha)",
    "generateMipmaps": "Mipmaps für BLP-Ausgabe erzeugen",
    "pickOutputTitle": "Ausgabeordner wählen"
  },
  "blpview": {
    "title": "Explorer-Vorschau (BLPView)",
    "install": "Installieren",
    "uninstall": "Deinstallieren",
    "restartExplorer": "Explorer neu starten",
    "statusError": "BLPView-Status konnte nicht gelesen werden.",
    "installFailed": "Installation fehlgeschlagen",
    "uninstallFailed": "Deinstallation fehlgeschlagen",
    "restartFailed": "Explorer konnte nicht neu gestartet werden",
    "loadingStatus": "BLPView-Status wird geprüft…",
    "unsupportedReason": "BLPView ist eine Windows-Explorer-Shell-Erweiterung. Ordner-Vorschauen für .blp-Dateien sind nur unter Windows 10/11 verfügbar."
  },
  "conversion": {
    "start": "Konvertiere {{count}} Datei(en) ({{label}})…",
    "outputSameFolder": "Ausgabe: gleicher Ordner wie Quelle",
    "noFiles": "Keine {{format}}-Dateien für diese Seite gefunden.",
    "fileResult": "{{name}} -> {{path}}",
    "failed": "Konvertierung fehlgeschlagen",
    "done": "Fertig. {{succeeded}} erfolgreich, {{failed}} fehlgeschlagen.",
    "scanError": "Scan: {{error}}",
    "dropFailed": "Abgelegte Dateien konnten nicht verarbeitet werden",
    "busy": "Konvertierung läuft bereits, bitte warten.",
    "outputCustomFolder": "Ausgabe: {{path}}",
    "labelBlpToPng": "BLP -> PNG",
    "labelPngToBlp": "PNG -> BLP"
  },
  "about": {
    "title": "Über"
  },
  "errorBoundary": {
    "title": "Etwas ist schiefgelaufen",
    "fallback": "Ein unerwarteter Fehler ist aufgetreten.",
    "retry": "Erneut versuchen"
  }
}
