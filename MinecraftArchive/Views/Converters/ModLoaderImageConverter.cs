﻿using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using MinecraftLaunch.Base.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinecraftArchive.Class.Utils;

namespace MinecraftArchive.Views.Converters
{
    public class ModLoaderImageConverter : IValueConverter {   
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            var type = (ModLoaderType)value!;

            try {
                return BitmapUtils.GetIconBitmap($"{type}.png");
            }
            catch (Exception) {           
            }

            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {       
            throw new NotImplementedException();
        }
    }
}
