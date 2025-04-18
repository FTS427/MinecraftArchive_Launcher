﻿using MinecraftArchive.Class.Utils;

namespace MinecraftArchive.Class.Models {
    public class ExceptionModel {
        public string? Message { get; set; }

        public string? Exception { get; set; }

        public string? StackTrace { get; set; }

        public string SystemType { get; set; } = SystemUtils.GetPlatformName();
    }
}
