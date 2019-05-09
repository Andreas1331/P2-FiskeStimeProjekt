using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Rainbowtrout : Fish
{
    public Rainbowtrout(int id, float width, GameObject preFab) : base(id, 2000, 0.5f, 10, width, FishType.RainbowTrout, preFab)
    {

    }
}
