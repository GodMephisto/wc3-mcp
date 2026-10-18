---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "DialogSystem/src/Screenplays/resources/CustomFrameFDF.fdf"
ref: HEAD
---

Frame "TEXT" "ClickableText" {
    //DecorateFileNames,
    FrameFont "MasterFont", 0.024, "",
    FontJustificationH JUSTIFYLEFT,
    FontJustificationV JUSTIFYTOP,
    FontColor 1.0 1.0 1.0 1.0,
    FontShadowColor 0.0 0.0 0.0 0.9,
    FontShadowOffset 0.001 -0.001,
}
Frame "TEXTAREA" "ClickableTextArea" {
    //DecorateFileNames,
    FrameFont "MasterFont", 0.013, "",
    TextAreaLineHeight 0.008,
    TextAreaLineGap 0.005,
    TextAreaInset 0.00,
    TextAreaMaxLines 128,
}
Frame "BACKDROP" "ClickableEscMenuBackdrop" {
	Width 0.288,
	Height 0.384,

    DecorateFileNames,
    BackdropTileBackground,
    BackdropBackground  "EscMenuBackground",
    BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",
    BackdropCornerSize  0.048,
    BackdropBackgroundSize  0.256,
    BackdropBackgroundInsets 0.01 0.01 0.01 0.01,
    BackdropEdgeFile  "EscMenuBorder",
    BackdropBlendAll,
}
Frame "TEXT" "UnclickableText" {
    //DecorateFileNames,
    FrameFont "MasterFont", 0.024, "",
    FontJustificationH JUSTIFYLEFT,
    FontJustificationV JUSTIFYTOP,
    FontColor 1.0 1.0 1.0 1.0,
    FontShadowColor 0.0 0.0 0.0 0.9,
    FontShadowOffset 0.001 -0.001,
    LayerStyle "IGNORETRACKEVENTS",
}
Frame "TEXTAREA" "UnclickableTextArea" {
    //DecorateFileNames,
    FrameFont "MasterFont", 0.013, "",
    TextAreaLineHeight 0.008,
    TextAreaLineGap 0.005,
    TextAreaInset 0.00,
    TextAreaMaxLines 128,
    LayerStyle "IGNORETRACKEVENTS",
}
Frame "BACKDROP" "UnclickableEscMenuBackdrop" {
	Width 0.288,
	Height 0.384,

    DecorateFileNames,
    BackdropTileBackground,
    BackdropBackground  "EscMenuBackground",
    BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",
    BackdropCornerSize  0.048,
    BackdropBackgroundSize  0.256,
    BackdropBackgroundInsets 0.01 0.01 0.01 0.01,
    BackdropEdgeFile  "EscMenuBorder",
    BackdropBlendAll,
    LayerStyle "IGNORETRACKEVENTS",
}
