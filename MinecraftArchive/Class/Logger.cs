﻿using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using MinecraftArchive.Class.Utils;
using MinecraftLaunch.Utilities;

namespace MinecraftArchive.Class {
    public class Logger {
        private List<string> Logs = new();

        private readonly string LogsPath = Path.Combine(Directory.GetCurrentDirectory(), ".log");

        public Logger Log(string message) {
            string log = $"[{SystemUtils.GetPlatformName()}] {message}";
            Logs.Add(log);
            Trace.WriteLine(log);
            return this;
        }

        public Logger Info(string message) {
            string log = $"[{SystemUtils.GetPlatformName()}][II] {message}";
            Logs.Add(log);
            Trace.WriteLine(log);
            return this;
        }

        public Logger Error(string message) {
            string log = $"[{SystemUtils.GetPlatformName()}][EE] {message}";
            Logs.Add(log);
            Trace.WriteLine(log);
            return this;
        }

        public Logger Warning(string message) {
            string log = $"[{SystemUtils.GetPlatformName()}][WW] {message}";
            Logs.Add(log);
            Trace.WriteLine(log);
            return this;
        }

        public static Logger LoadLogger(Window window) {
            Logger logger = new Logger();
            window.Closed += async (o, ctx) => {
                await logger.EncapsulateLogsToFileAsync();
                Environment.Exit(0);
            };

            return logger.Info("Logger has been loaded");
        }

        public async ValueTask EncapsulateLogsToFileAsync() {
            JsonUtils.WriteLaunchInfoJson();
            JsonUtils.WriteLauncherInfoJson();

            if (!LogsPath.IsDirectory()) {
                Directory.CreateDirectory(LogsPath);
            }

            var today = DateTime.Now;
            await File.WriteAllLinesAsync(Path.Combine(LogsPath, $"{today:yyyy-MM-dd-HH-mm-ss}.log"), Logs);
        }
    }
}
