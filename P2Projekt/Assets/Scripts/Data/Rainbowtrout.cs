using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class Rainbowtrout : Fish
    {
        public Rainbowtrout(int id, GameObject preFab) : base(id, 2000, 3, 10, FishType.RainbowTrout, preFab)
        {

        } 
    }
}
