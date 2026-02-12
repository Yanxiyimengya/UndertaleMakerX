using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IObjectPoolObject
{
    public virtual void Awake()
    {
    }

    public virtual void Disabled()
    {
    }
}
