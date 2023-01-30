﻿using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Sendinblue.Models
{
    /// <summary>
    /// Represents SMS list model
    /// </summary>
    public record SmsListModel : BasePagedListModel<SmsModel>
    {
    }
}