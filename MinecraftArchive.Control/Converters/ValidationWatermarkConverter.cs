﻿using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinecraftArchive.control.Controls.Dialog;

namespace MinecraftArchive.control.Converters {
    public class ValidationWatermarkConverter : IValueConverter {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            var result = (ValidationDialog.ValidationTypes)value!;

            return result switch {
                ValidationDialog.ValidationTypes.Yggdrasil => "优香地址",
                ValidationDialog.ValidationTypes.Offline => "玩家名",
                _ => "玩家名",
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
