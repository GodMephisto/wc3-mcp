---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Extensions/CampaignInfoExtensions.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Extensions
{
    public static class CampaignInfoExtensions
    {
        private static readonly Encoding _defaultEncoding = UTF8EncodingProvider.StrictUTF8;

        public static void WriteArchiveHeaderToStream(this CampaignInfo campaignInfo, Stream stream)
        {
            campaignInfo.WriteArchiveHeaderToStream(stream, null, _defaultEncoding);
        }

        public static void WriteArchiveHeaderToStream(this CampaignInfo campaignInfo, Stream stream, TriggerStrings? campaignTriggerStrings)
        {
            campaignInfo.WriteArchiveHeaderToStream(stream, campaignTriggerStrings, _defaultEncoding);
        }

        public static void WriteArchiveHeaderToStream(this CampaignInfo campaignInfo, Stream stream, Encoding encoding)
        {
            campaignInfo.WriteArchiveHeaderToStream(stream, null, encoding);
        }

        public static void WriteArchiveHeaderToStream(this CampaignInfo campaignInfo, Stream stream, TriggerStrings? campaignTriggerStrings, Encoding encoding)
        {
            using (var writer = new BinaryWriter(stream, encoding, true))
            {
                writer.Write("HM3W".FromRawcode());
                writer.Write(0);
                writer.WriteString(campaignInfo.CampaignName.Localize(campaignTriggerStrings));
                writer.Write((int)campaignInfo.CampaignFlags);
                writer.Write(1);
            }
        }
    }
}