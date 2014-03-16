using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Cascade.Behaviors
{
    public class ParticleBehavior
    {
        public ParticleBehavior()
        {

        }
        public virtual void Update(Particle part)
        {

        }
    }
    public class Disappear : ParticleBehavior
    {
        float time = 0, timer, fadeInSpeed, fadeOutSpeed, maxAlpha;
        float alpha = 0;
        bool started = false;
        public Disappear(float timer, float fadeinspeed, float fadeoutspeed, float maxalpha)
        {
            this.timer = timer;
            this.fadeInSpeed = fadeinspeed;
            this.fadeOutSpeed = fadeoutspeed;
            this.maxAlpha = maxalpha;
        }
        public override void Update(Particle part)
        {
            time += Global.Speed;
            if (!started)
            {
                alpha += fadeInSpeed * Global.Speed;
                if (alpha > maxAlpha)
                {
                    alpha = maxAlpha;
                    started = true;
                }
            }
            if (time > timer)
            {
                alpha -= fadeOutSpeed * Global.Speed;
                if (alpha < 0)
                {
                    part.Remove();
                }
            }
            part.Alpha = alpha;
            base.Update(part);
        }
    }
    public class Spin : ParticleBehavior
    {
        Vector3 radius;
        Vector3 phase = Vector3.Zero;
        Vector3 phaseSpeed;
        bool start = false;
        Vector3 pos = Vector3.Zero;
        public Spin(Vector3 radius, Vector3 phaseSpeed)
        {
            this.phaseSpeed = phaseSpeed;
            this.radius = radius;
        }
        public override void Update(Particle part)
        {
            if (!start)
            {
                pos = part.Pos;
                start = true;
            }
            pos += part.Speed * Global.Speed;
            phase += phaseSpeed * Global.Speed;
            Vector3 x = new Vector3(0, MyMath.LengthDirX(radius.X, phase.X), MyMath.LengthDirY(radius.X, phase.X));
            Vector3 y = new Vector3(MyMath.LengthDirX(radius.Y, phase.Y), 0, MyMath.LengthDirY(radius.Y, phase.Y));
            Vector3 z = new Vector3(MyMath.LengthDirX(radius.Z, phase.Z), MyMath.LengthDirY(radius.Z, phase.Z), 0);
            part.Pos = pos + x + y + z;
            base.Update(part);
        }
    }
    public class Bounce : ParticleBehavior
    {
        float minY = 0;
        float speedMult = 1;
        public Bounce(float minY, float speedMult = 1)
        {
            this.minY = minY;
            this.speedMult = speedMult;
        }
        public override void Update(Particle part)
        {
            if (part.Pos.Y <= minY)
            {
                part.Pos.Y = minY;
                part.Speed.Y *= -speedMult;
            }
            base.Update(part);
        }
    }

}
