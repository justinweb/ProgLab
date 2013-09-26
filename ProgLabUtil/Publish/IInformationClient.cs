using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgLab.Util.Publish
{
    public interface IInformationClient<TInfo>
    {
        void Receive(TInfo info);
    }
}
