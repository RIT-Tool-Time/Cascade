using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cascade
{
    public class FieldAndObjectInfo
    {
        public Object Object
        {
            get
            {
                try
                {
                    return Objects.Last();
                }
                catch
                {
                    return null;
                }
            }
        }
        public MemberInfo FieldInfo
        {
            get
            {
                try
                {
                    return Fields.Last();
                }
                catch
                {
                    return null;
                }
            }
        }
        public List<Object> Objects = new List<object>();
        public List<MemberInfo> Fields = new List<MemberInfo>();
        public FieldAndObjectInfo()
        {

        }
        public void BubbleBackValues()
        {
            object lastObj = null;
            for (int i = Objects.Count - 1; i >= 0; i--)
            {
                int ii = i;
                if (i < Objects.Count && ii < Fields.Count)
                {
                    if (i == Objects.Count - 1)
                    {
                        var field = Fields[ii];
                        var obj = Objects[i];
                        field.SetValue(obj, field.GetValue(Objects[i]));
                        lastObj = obj;
                    }
                    else
                    {
                        var field = Fields[ii];
                        var obj = Objects[i];
                        field.SetValue(obj, lastObj);
                        lastObj = obj;
                    }
                }
            }
        }
    }
    public static class GameExtensions
    {
        //public static ReflectionObject CreateReflectorObject(this object Object)
        //{

        //}
        public static Vector2 GetCenter(this Texture2D tex)
        {
            return new Vector2(tex.Width / 2, tex.Height / 2);
        }
        public static void SetValue(this MemberInfo member, object obj, object value)
        {
            if (member is FieldInfo)
            {
                ((FieldInfo)member).SetValue(obj, value);
            }
            else
            {
                if (member is PropertyInfo)
                {
                    ((PropertyInfo)member).SetValue(obj, value, null);
                }
            }
        }
        public static object GetValue(this MemberInfo member, object obj)
        {
            if (member is FieldInfo)
            {
                return ((FieldInfo)member).GetValue(obj);
            }
            else if (member is PropertyInfo)
            {
                return ((PropertyInfo)member).GetValue(obj, null);
            }
            else
            {
                return null;
            }
        }
        public static Type Type(this MemberInfo member)
        {
            if (member is FieldInfo)
            {
                return ((FieldInfo)member).FieldType;
            }
            else if (member is PropertyInfo)
            {
                return ((PropertyInfo)member).PropertyType;
            }
            else
            {
                return null;
            }
        }
        public static FieldAndObjectInfo GetFieldInfo(string property, object o)
        {
            var props = property.Split('.');
            string finalProp = props[props.Length - 1];
            FieldInfo pi = null;
            object lastObj = o;
            FieldAndObjectInfo fao = new FieldAndObjectInfo() {  };
            foreach (var prop in props)
            {
                try
                {
                    var pr = lastObj.GetType().GetMember(prop);
                    if (prop == finalProp)
                    {
                        fao.Fields.Add(pr[0]);
                        fao.Objects.Add(lastObj);
                        return fao;
                    }
                    fao.Fields.Add(pr[0]);
                    fao.Objects.Add(lastObj);
                    lastObj = pr[0].GetValue(lastObj);
                }
                catch
                {
                    return null;
                }
            }
            return fao;
        }
        public static FieldAndObjectInfo GetStaticFieldInfo(string property)
        {
            var props = property.Split('.');
            if (props.Length > 1)
            {
                string className = props.First();
                var types = Assembly.GetExecutingAssembly().GetTypes();
                Type classType = null;
                foreach (var type in types)
                {
                    if (type.Name == className)
                    {
                        classType = type;
                        break;
                    }
                }
                if (classType != null)
                {
                    var field = classType.GetMember(props[1])[0];
                    if (props.Length > 2)
                    {
                        string prop = "";
                        for (int i = 2; i < props.Length; i++)
                        {
                            prop += props[i] + ((i < props.Length - 1) ? "." : "");
                        }
                        return GetFieldInfo(prop, field.GetValue(null));
                    }
                    else
                    {
                        var info = new FieldAndObjectInfo();
                        //info.Objects.Add(field.GetValue(null));
                        info.Fields.Add(field);
                        return info;
                    }

                }
            }
            return null;
        }
        public static float Average(this float[] floats)
        {
            float flo = 0;
            foreach (var f in floats)
            {
                flo += f;
            }
            return flo / floats.Length;
        }
        public static string PrintAll(this float[] floats)
        {
            string s = "";
            foreach (var f in floats)
            {
                s += f.ToString() + ", ";
            }
            return s;
        }
        public static Vector3 ToVector3(this Vector2 vec)
        {
            return new Vector3(vec.X, vec.Y, 0);
        }
        public static Vector2 ToVector2(this Vector3 vec)
        {
            return new Vector2(vec.X, vec.Y);
        }
        public static Vector2 ToVector2(this Point point)
        {
            return new Vector2(point.X, point.Y);
        }
        public static float MinDistance(this Rectangle rec, float objZ, Vector3 vec)
        {
            float d = (new Vector3(rec.Left, rec.Top, objZ) - vec).Length();
            float d2 = (new Vector3(rec.Right, rec.Top, objZ) - vec).Length();
            float d3 = (new Vector3(rec.Left, rec.Bottom, objZ) - vec).Length();
            float d4 = (new Vector3(rec.Right, rec.Bottom, objZ) - vec).Length();
            float d5 = (new Vector3(rec.Center.X, rec.Center.Y, objZ) - vec).Length();
            return Math.Min(Math.Min(Math.Min(Math.Min(d, d2), d3), d4), d5);
        }
        public static Color GetColorFromHue(float Hue)
        {
            Hue = Hue % 3;
            if (Hue < 1)
                return new Color((1f - Hue), Hue, 0);
            else if (Hue < 2)
                return new Color(0, (2f - Hue), Hue - 1f);
            else
                return new Color(Hue - 2f, 0, (3f - Hue));
        }
    }
}
