﻿using MusicPlayer.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Core.Searching
{
    interface ISearchCommandAddon
    {
        public void ConfigureSearchOptions(GlobalSearch globalSearch);
    }
}
