using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class MemoryStreamListWrapper
{
    private List<MemoryStreamWrapper> _mswList;
    public List<MemoryStreamWrapper> MswList { get => _mswList; set => _mswList = value; }

    public MemoryStreamListWrapper()
    {
        _mswList = new List<MemoryStreamWrapper>();
    }
}

