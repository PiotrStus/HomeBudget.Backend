﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Interfaces
{
    public interface ITemplate
    {
        string Body { get; }
        string Subject { get; }
    }
}