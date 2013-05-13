using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Athyl
{
    public class Damage
    {
        #region Properties
        public int bodyId;
        public float damage;
        #endregion
        #region Constructor
        public Damage(int bodyId, float damage)
        {          
            this.bodyId = bodyId;
            this.damage = damage;
        }

        public Damage(int bodyId, float damage, float playerDmg)
        {
            this.bodyId = bodyId;
            this.damage = damage * playerDmg;
        }
        #endregion
    }
}
