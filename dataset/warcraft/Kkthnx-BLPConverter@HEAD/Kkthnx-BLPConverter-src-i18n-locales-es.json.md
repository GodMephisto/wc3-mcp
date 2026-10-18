---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src/i18n/locales/es.json"
ref: HEAD
---

{
  "app": {
    "name": "BLP Converter",
    "tagline": "Conversión rápida BLP y PNG para texturas de World of Warcraft",
    "description": "Herramienta de escritorio ligera para convertir texturas BLP de Blizzard a PNG y viceversa. Arrastra archivos al lado izquierdo (BLP) o derecho (PNG), las conversiones se ejecutan en paralelo. Los archivos se guardan junto a los originales, salvo que elijas otra carpeta en Ajustes.",
    "madeBy": "Hecho por {{author}}",
    "dropToConvert": "Arrastra archivos para convertir",
    "converting": "Convirtiendo…"
  },
  "panels": {
    "toPng": {
      "hint": "Arrastra archivos o carpetas .blp",
      "subhint": "Exporta PNG con transparencia"
    },
    "toBlp": {
      "hint": "Arrastra archivos o carpetas .png",
      "subhint": "Codifica con tu configuración de compresión"
    }
  },
  "activity": {
    "title": "Actividad",
    "clear": "Borrar",
    "empty": "Arrastra archivos BLP o PNG, por defecto se guardan junto a los originales."
  },
  "settings": {
    "title": "Ajustes",
    "close": "Cerrar",
    "language": "Idioma",
    "outputFolder": "Carpeta de salida",
    "outputFolderHint": "Opcional. Déjalo vacío para guardar junto a los originales.",
    "outputDefault": "Misma carpeta que el origen (predeterminado)",
    "resetOutput": "Restablecer carpeta de origen",
    "compression": "Compresión PNG -> BLP",
    "compressionRaw": "Sin comprimir / RAW",
    "compressionDxt1": "DXT1 (opaco o alpha de 1 bit)",
    "compressionDxt3": "DXT3 (bordes alpha nítidos)",
    "compressionDxt5": "DXT5 (alpha suave)",
    "generateMipmaps": "Generar mipmaps para salida BLP",
    "pickOutputTitle": "Seleccionar carpeta de salida"
  },
  "blpview": {
    "title": "Miniaturas del Explorador (BLPView)",
    "install": "Instalar",
    "uninstall": "Desinstalar",
    "restartExplorer": "Reiniciar Explorador",
    "statusError": "No se pudo leer el estado de BLPView.",
    "installFailed": "Error al instalar",
    "uninstallFailed": "Error al desinstalar",
    "restartFailed": "No se pudo reiniciar el Explorador",
    "loadingStatus": "Comprobando el estado de BLPView…",
    "unsupportedReason": "BLPView es una extensión shell del Explorador de Windows. Las miniaturas de carpetas para archivos .blp solo están disponibles en Windows 10/11."
  },
  "conversion": {
    "start": "Convirtiendo {{count}} archivo(s) ({{label}})…",
    "outputSameFolder": "Salida: misma carpeta que el origen",
    "noFiles": "No se encontraron archivos {{format}} para este lado.",
    "fileResult": "{{name}} -> {{path}}",
    "failed": "Error de conversión",
    "done": "Listo. {{succeeded}} correctos, {{failed}} fallidos.",
    "scanError": "Escaneo: {{error}}",
    "dropFailed": "No se pudieron procesar los archivos arrastrados",
    "busy": "Conversión en curso, espere a que termine.",
    "outputCustomFolder": "Salida: {{path}}",
    "labelBlpToPng": "BLP -> PNG",
    "labelPngToBlp": "PNG -> BLP"
  },
  "about": {
    "title": "Acerca de"
  },
  "errorBoundary": {
    "title": "Algo salió mal",
    "fallback": "Ocurrió un error inesperado.",
    "retry": "Reintentar"
  }
}
