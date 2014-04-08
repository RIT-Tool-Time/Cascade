using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Cascade.Behaviors
{
    public class Pulsate : ParticleBehavior
    {
        float phase = 0, phaseSpeed = 0.1f;
        Vector2 startScale, endScale;
        float mult = 1;
        public Pulsate(Vector2 startScale, Vector2 endScale, float startPhase, float phaseSpeed)
        {
            phase = startPhase;
            this.phaseSpeed = phaseSpeed;
            this.startScale = startScale;
            this.endScale = endScale;
        }
        public override void Update(Particle part)
        {
            //phaseTarget = phaseTarget % 1f;
            phase += phaseSpeed * Global.Speed * mult;
            phase = MathHelper.Clamp(phase, 0, 1);
            double pow = 1.5;

            float value = phase;

            value = (float)(Math.Pow(value, pow));

            value = 1 - (float)Math.Pow(1 - value, pow);
            if (phase >= 1)
            {
                mult = -1;
            }
            else if (phase <= 0)
            {
                mult = 1;
            }
            
            Vector2 val = MyMath.Between(startScale, endScale, value);
            part.Scale = val;
            base.Update(part);
        }
    }
}
