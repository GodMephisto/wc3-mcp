---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/cfg/buildprofiles.xml"
ref: HEAD
---

<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE buildprofiles SYSTEM "https://resources.jetbrains.com/writerside/1.0/build-profiles.dtd">
<buildprofiles xsi:noNamespaceSchemaLocation="https://resources.jetbrains.com/writerside/1.0/build-profiles.xsd"
               xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

    <build-profile instance="JassHelper">
        <variables>
            <primary-color>ruby</primary-color>
            <color-preset>contrast</color-preset>
            <noindex-content>false</noindex-content>

            <download-title>JetBrains plugin</download-title>
            <download-page>https://plugins.jetbrains.com/plugin/23120-jass</download-page>
            <showDownloadButton>true</showDownloadButton>

            <webmaster>nazarpunk@gmail.com</webmaster>

            <og-image>https://repository-images.githubusercontent.com/867122770/e54bbda9-9694-425e-b891-d37cf285aeac</og-image>

            <!-- https://www.jetbrains.com/help/writerside/buildprofiles-xml.html#resizable-sidebar -->
            <resizable-sidebar>true</resizable-sidebar>

            <!-- https://www.jetbrains.com/help/writerside/offline-documentation.html-->
            <offline-docs>true</offline-docs>

            <!-- https://www.jetbrains.com/help/writerside/buildprofiles-xml.html#content-max-width -->
            <content-max-width>99999</content-max-width>
        </variables>

        <footer>
            <link href="https://discord.com/invite/mpvASEcujC">Discord</link>
            <link href="https://unryzec.github.io/UjAPI">UjAPI</link>

            <link href="https://warraft.github.io/JASS-doc">JASS</link>
            <link href="https://warraft.github.io/AngelScript-doc">AngelScript</link>
            <link href="https://plugins.jetbrains.com/plugin/23120-jass">JetBrains plugin</link>

        </footer>
    </build-profile>

</buildprofiles>
