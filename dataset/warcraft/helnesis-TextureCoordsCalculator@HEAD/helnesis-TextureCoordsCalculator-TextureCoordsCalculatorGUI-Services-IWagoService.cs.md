---
type: vendor-doc
source_repo: "https://github.com/helnesis/TextureCoordsCalculator"
source_path: "TextureCoordsCalculatorGUI/Services/IWagoService.cs"
ref: HEAD
---

﻿using System.IO;

namespace TextureCoordsCalculatorGUI.Services
{
    public interface IWagoService
    {
        /// <summary>
        /// Get file through Wago and store it into a stream.
        /// </summary>
        /// <param name="fdid">File unique identifier</param>
        /// <returns>Stream</returns>
        Task<Stream> GetCascFile(uint fdid);
    }
}
