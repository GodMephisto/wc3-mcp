---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src/i18n/locales/pt-BR.json"
ref: HEAD
---

{
  "app": {
    "name": "BLP Converter",
    "tagline": "Conversão rápida BLP e PNG para texturas do World of Warcraft",
    "description": "Ferramenta desktop leve para converter texturas BLP da Blizzard para PNG e vice-versa. Arraste arquivos para o lado esquerdo (BLP) ou direito (PNG), as conversões rodam em paralelo. Os arquivos são salvos ao lado dos originais, a menos que você escolha outra pasta nas Configurações.",
    "madeBy": "Feito por {{author}}",
    "dropToConvert": "Arraste arquivos para converter",
    "converting": "Convertendo…"
  },
  "panels": {
    "toPng": {
      "hint": "Arraste arquivos ou pastas .blp",
      "subhint": "Exporta PNG com transparência"
    },
    "toBlp": {
      "hint": "Arraste arquivos ou pastas .png",
      "subhint": "Codifica com suas configurações de compressão"
    }
  },
  "activity": {
    "title": "Atividade",
    "clear": "Limpar",
    "empty": "Arraste arquivos BLP ou PNG, por padrão a saída fica ao lado dos originais."
  },
  "settings": {
    "title": "Configurações",
    "close": "Fechar",
    "language": "Idioma",
    "outputFolder": "Pasta de saída",
    "outputFolderHint": "Opcional. Deixe vazio para salvar ao lado dos originais.",
    "outputDefault": "Mesma pasta do original (padrão)",
    "resetOutput": "Restaurar pasta de origem",
    "compression": "Compressão PNG -> BLP",
    "compressionRaw": "Sem compressão / RAW",
    "compressionDxt1": "DXT1 (opaco ou alpha de 1 bit)",
    "compressionDxt3": "DXT3 (bordas alpha nítidas)",
    "compressionDxt5": "DXT5 (alpha suave)",
    "generateMipmaps": "Gerar mipmaps na saída BLP",
    "pickOutputTitle": "Selecionar pasta de saída"
  },
  "blpview": {
    "title": "Miniaturas do Explorer (BLPView)",
    "install": "Instalar",
    "uninstall": "Desinstalar",
    "restartExplorer": "Reiniciar Explorer",
    "statusError": "Não foi possível ler o status do BLPView.",
    "installFailed": "Falha na instalação",
    "uninstallFailed": "Falha na desinstalação",
    "restartFailed": "Não foi possível reiniciar o Explorer",
    "loadingStatus": "Verificando o status do BLPView…",
    "unsupportedReason": "BLPView é uma extensão shell do Explorador de Arquivos do Windows. Miniaturas de pastas para arquivos .blp estão disponíveis apenas no Windows 10/11."
  },
  "conversion": {
    "start": "Convertendo {{count}} arquivo(s) ({{label}})…",
    "outputSameFolder": "Saída: mesma pasta do original",
    "noFiles": "Nenhum arquivo {{format}} encontrado para este lado.",
    "fileResult": "{{name}} -> {{path}}",
    "failed": "Falha na conversão",
    "done": "Concluído. {{succeeded}} com sucesso, {{failed}} com falha.",
    "scanError": "Varredura: {{error}}",
    "dropFailed": "Falha ao processar arquivos arrastados",
    "busy": "Conversão em andamento, aguarde a conclusão.",
    "outputCustomFolder": "Saída: {{path}}",
    "labelBlpToPng": "BLP -> PNG",
    "labelPngToBlp": "PNG -> BLP"
  },
  "about": {
    "title": "Sobre"
  },
  "errorBoundary": {
    "title": "Algo deu errado",
    "fallback": "Ocorreu um erro inesperado.",
    "retry": "Tentar novamente"
  }
}
