using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Cascade
{
    public delegate void ParticleEmittedEventHandler(ParticleEmittedEventArgs e);
    public class ParticleEmittedEventArgs : EventArgs
    {
        public ParticleEmitter Emitter;
        public Particle Particle;
    }
    public class ParticleEmitter
    {
        Vector3 prevPos = Vector3.Zero;
        public Vector3 Pos = Vector3.Zero;
        public Vector3 PosRange = Vector3.Zero;
        public float Step = 1;
        float timer = 0;
        protected ParticleManager manager;
        public Vector2 Scale = Vector2.One;
        public Vector2 ScaleRange = Vector2.Zero;
        public Vector3 Speed = Vector3.Zero;
        public Vector3 SpeedRange = Vector3.Zero;
        public Color Color = Color.Gray;
        public Color ColorRange = new Color(0, 0, 0, 0);
        public event ParticleEmittedEventHandler Emitted;
        public bool Emit = true;
        public float SpeedTransferMultiplier = 1;
        
        public ParticleEmitter(ParticleManager man, Vector3 pos)
        {
            Pos = pos;
            manager = man;
            man.Add(this);
        }
        public virtual void Update()
        {
            if (Emit)
            {
                
                timer += Global.Speed;
                while (Step > 0 && timer > Step)
                {
                    timer -= Step;
                    EmitParticle();
                }
            }
            prevPos = Pos;
        }
        public virtual void EmitParticle()
        {
            var p = CreateParticle();
            p.Pos = Pos.RandomVectorRange(PosRange);
            p.Scale = Scale + new Vector2(MyMath.RandomRange(-ScaleRange.X, ScaleRange.X), MyMath.RandomRange(-ScaleRange.Y, ScaleRange.Y));
            p.Speed = Speed.RandomVectorRange(SpeedRange) + ((Pos - prevPos) * SpeedTransferMultiplier);
            p.Color = new Color(Color.ToVector4().RandomVectorRange(ColorRange.ToVector4()));
            if (Emitted != null)
            {
                Emitted(new ParticleEmittedEventArgs() { Particle = p, Emitter = this });
            }
        }
        protected virtual Particle CreateParticle()
        {
            return new Particle(manager, Vector3.Zero);
        }
    }
    public class CircleEmitter : ParticleEmitter
    {
        public CircleEmitter(ParticleManager man, Vector3 p)
            : base(man, p)
        {

        }
        protected override Particle CreateParticle()
        {
            return new Ellipse(manager, Vector3.Zero, 32);
        }
    }
    public class TriangleEmitter : ParticleEmitter
    {
        public TriangleEmitter(ParticleManager m, Vector3 p)
            : base(m, p)
        {

        }
        protected override Particle CreateParticle()
        {
            return new Triangle(manager, Vector3.Zero);
        }
    }
    public class TouchEmitter : CircleEmitter
    {
        public TouchPoint Touch = null;
        public TouchEmitter(ParticleManager man, Vector3 p)
            : base(man, p)
        {
            Emit = false;
        }
        public override void Update()
        {
            
            if (Touch != null)
            {
                bool hold = (Touch.Position - Pos.ToVector2()).Length() < 1;
                //Global.Output += Pos + ", " + Touch.Position;
                Pos = Touch.Position.ToVector3();
                if (Touch.State == TouchState.Moved)
                {
                    if (Touch.Timer < 10)
                    {
                        Speed = SpeedRange = Vector3.Zero;
                        Emit = false;
                    }
                    else if (Touch.Holding)
                    {
                        SpeedRange = new Vector3(new Vector2(50), 0);
                        Emit = true;
                    }
                    else
                    {
                        Emit = false;
                    }
                }
                else if (Touch.State == TouchState.Touched)
                {
                    Speed = SpeedRange = Vector3.Zero;
                    EmitParticle();
                }
                else
                {
                    Speed = SpeedRange = Vector3.Zero;
                    Emit = false;
                }
                
                if (Touch.State == TouchState.Released || Touch.State == TouchState.None)
                {
                    Speed = SpeedRange = Vector3.Zero;
                    Emit = false;
                    Touch = null;
                }
            }

            base.Update();
        }
    }
}
