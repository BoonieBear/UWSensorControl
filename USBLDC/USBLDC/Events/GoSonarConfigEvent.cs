﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USBLDC.Events
{
    public class GoSonarConfigEvent
    {
        public bool Show { get; set; }

        public GoSonarConfigEvent(bool show = true)
        {
            Show = show;
        }
    }
}
