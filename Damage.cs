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
        #endregion
    }
}
