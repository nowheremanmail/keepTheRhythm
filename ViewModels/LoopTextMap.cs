﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace UniversalKeepTheRhythm.ViewModels
{
    public class LoopTextMap: TextMap
    {
        public TappedEventHandler Tapped { get; set; }
    }
}
