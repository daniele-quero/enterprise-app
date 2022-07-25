using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class MemoryStreamWrapper
{
    private MemoryStream _ms;

    public MemoryStream Ms { get => _ms; set => _ms = value; }
}

