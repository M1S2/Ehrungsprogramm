﻿using System;
using System.Windows.Controls;

namespace Ehrungsprogramm.Contracts.Services
{
    public interface IPageService
    {
        Type GetPageType(string key);

        Page GetPage(string key);
    }
}
