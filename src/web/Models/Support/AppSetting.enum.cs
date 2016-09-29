using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wwwplatform.Models
{
	public partial class AppSetting
    {
        public const int KindString = 0;
        public const int KindBool = 1;
        public const int KindDirectory = 2;
        public const int KindFile = 3;
        public const int KindUpload = 4;
        public const int KindRole = 5;
        public const int KindNumber = 6;
    }
}