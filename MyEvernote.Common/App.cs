using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Common
{
    public static class App
    {
        public static ICommon Common = new DefaultCommon();
        //burada DefaultCommon Web,mobil yada desktop için default değer dönüyor eğer istersek bunu değiştirebiliyorz.Global.asax içerisinde WebCommon kullanarak sessiondan bir veri attık. Yada Mobilden ulaşmak isteseydik Icommona erişip methodunu tanımlamak bir değer değiştirmek için yeterli olacak.
    }
}
