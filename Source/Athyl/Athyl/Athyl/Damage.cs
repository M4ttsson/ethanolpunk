using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Athyl
{
    public class Damage
    {
        public int bodyId;
        public float damage; 

        public Damage(int bodyId, float damage)
        {
          
            this.bodyId = bodyId;
            this.damage = damage;
            
        }
    }
}
