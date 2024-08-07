﻿using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinecraftArchive.Class.AppData;
using MinecraftArchive.Class.Enum;
using MinecraftArchive.Class.Utils;

namespace MinecraftArchive.Views.Converters {
    public class ClassToInt32Converter : IValueConverter {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
			try {
                bool isInt = RegexUtils.Int32Check.IsMatch(value!.ToString()!);
                if (isInt) {
                    return value.ToString();
                }
			}
			catch (Exception ex) {
                $"转换失败，原因：{ex}".ShowLog(LogLevel.Error);
			}

            "转换无效，不是数字！".ShowMessage();
            return 0;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            try {
                bool isInt = RegexUtils.Int32Check.IsMatch(value!.ToString()!);
                if (isInt) {
                    return value.ToInt32();
                }
            }
            catch (Exception ex) {
                $"转换失败，原因：{ex}".ShowLog(LogLevel.Error);
            }
            
            "转换无效，不是数字！".ShowMessage();
            return 0;
        }
    }
}
