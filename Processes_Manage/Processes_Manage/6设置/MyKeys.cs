using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Processes_Manage
{
    public class MyKeys : List<System.Windows.Forms.Keys>
    {
        public MyKeys()
        {
            for (int i = 65; i < 91; i++)                                   //A--Z
                this.Add((System.Windows.Forms.Keys)i);
            for (int k = 112; k < 124; k++)                            //F1--F12
                this.Add((System.Windows.Forms.Keys)k);
            for (int j = 37; j < 41; j++)                                  //left-right
                this.Add((System.Windows.Forms.Keys)j);
            for (int t = 48; t < 58; t++)                                 //0--9
                this.Add((System.Windows.Forms.Keys)t);
        }
    }
}
