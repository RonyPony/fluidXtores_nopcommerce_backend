using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Misc.FluidApi.Domain
{
    public class WebHooks : BaseEntity
    {
        public string? User { get; set; }

        public new string? Id { get; set; }

        public string? ProtectedData { get; set; }

        public Byte[]? RowVer { get; set; }
    }
}
