---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src/i18n/locales/fr.json"
ref: HEAD
---

{
  "app": {
    "name": "BLP Converter",
    "tagline": "Conversion rapide BLP et PNG pour les textures World of Warcraft",
    "description": "Un outil de bureau léger pour convertir les textures BLP de Blizzard en PNG et inversement. Déposez les fichiers à gauche (BLP) ou à droite (PNG), les conversions s'exécutent en parallèle. Les fichiers sont enregistrés à côté des originaux, sauf si vous choisissez un autre dossier dans les paramètres.",
    "madeBy": "Créé par {{author}}",
    "dropToConvert": "Déposez des fichiers pour convertir",
    "converting": "Conversion…"
  },
  "panels": {
    "toPng": {
      "hint": "Déposez des fichiers ou dossiers .blp",
      "subhint": "Exporte des PNG transparents"
    },
    "toBlp": {
      "hint": "Déposez des fichiers ou dossiers .png",
      "subhint": "Encode avec vos paramètres de compression"
    }
  },
  "activity": {
    "title": "Activité",
    "clear": "Effacer",
    "empty": "Déposez des fichiers BLP ou PNG, les sorties sont enregistrées à côté des originaux par défaut."
  },
  "settings": {
    "title": "Paramètres",
    "close": "Fermer",
    "language": "Langue",
    "outputFolder": "Dossier de sortie",
    "outputFolderHint": "Facultatif. Laissez vide pour enregistrer à côté des originaux.",
    "outputDefault": "Même dossier que la source (par défaut)",
    "resetOutput": "Réinitialiser au dossier source",
    "compression": "Compression PNG -> BLP",
    "compressionRaw": "Non compressé / RAW",
    "compressionDxt1": "DXT1 (opaque ou alpha 1 bit)",
    "compressionDxt3": "DXT3 (bords alpha nets)",
    "compressionDxt5": "DXT5 (alpha lisse)",
    "generateMipmaps": "Générer les mipmaps pour la sortie BLP",
    "pickOutputTitle": "Sélectionner le dossier de sortie"
  },
  "blpview": {
    "title": "Miniatures Explorer (BLPView)",
    "install": "Installer",
    "uninstall": "Désinstaller",
    "restartExplorer": "Redémarrer l'Explorateur",
    "statusError": "Impossible de lire l'état de BLPView.",
    "installFailed": "Échec de l'installation",
    "uninstallFailed": "Échec de la désinstallation",
    "restartFailed": "Impossible de redémarrer l'Explorateur",
    "loadingStatus": "Vérification de l'état de BLPView…",
    "unsupportedReason": "BLPView est une extension shell de l'Explorateur Windows. Les miniatures de dossiers pour les fichiers .blp ne sont disponibles que sur Windows 10/11."
  },
  "conversion": {
    "start": "Conversion de {{count}} fichier(s) ({{label}})…",
    "outputSameFolder": "Sortie : même dossier que la source",
    "noFiles": "Aucun fichier {{format}} trouvé pour ce côté.",
    "fileResult": "{{name}} -> {{path}}",
    "failed": "Échec de la conversion",
    "done": "Terminé. {{succeeded}} réussi(s), {{failed}} échoué(s).",
    "scanError": "Analyse : {{error}}",
    "dropFailed": "Impossible de traiter les fichiers déposés",
    "busy": "Conversion en cours, veuillez patienter.",
    "outputCustomFolder": "Sortie : {{path}}",
    "labelBlpToPng": "BLP -> PNG",
    "labelPngToBlp": "PNG -> BLP"
  },
  "about": {
    "title": "À propos"
  },
  "errorBoundary": {
    "title": "Une erreur s'est produite",
    "fallback": "Une erreur inattendue s'est produite.",
    "retry": "Réessayer"
  }
}
