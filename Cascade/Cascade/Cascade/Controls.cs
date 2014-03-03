using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Cascade
{
    public enum ControlState { None, Held, Released, Pressed }
    public class Controls
    {
        public const int Pressed = 3;
        public const int Released = 2;
        public const int Held = 1;
        public const int None = 0;

        public static MouseState mouse;
        public static Vector2 mousePos, mousePosPrev = Vector2.Zero;
        private int mouseLB =0;
        private int mouseLB2 = 0;
        public int mouseLB3 = 0;
        private int mouseRB = 0;
        private int mouseRB2 = 0;
        public int mouseRB3 = 0;
        private int mouseCB = 0;
        private int mouseCB2 = 0;
        public int mouseCB3 = 0;
        public static Vector2 ScreenSize = new Vector2(1280, 720);
        static Vector2 mouseMul = new Vector2(1280, 720);


        public static ControlState MouseLeft
        {
            get
            {
                switch (instance.mouseLB3)
                {
                    case Pressed:
                        return ControlState.Pressed;
                    case Released:
                        return ControlState.Released;
                    case Held:
                        return ControlState.Held;
                    case None:
                        return ControlState.None;
                    default:
                        return ControlState.None;
                }
            }
        }

        public static ControlState MouseRight
        {
            get
            {
                switch (instance.mouseRB3)
                {
                    case Pressed:
                        return ControlState.Pressed;
                    case Released:
                        return ControlState.Released;
                    case Held:
                        return ControlState.Held;
                    case None:
                        return ControlState.None;
                    default:
                        return ControlState.None;
                }
            }
        }
        public static ControlState MouseCenter
        {
            get
            {
                switch (instance.mouseCB3)
                {
                    case Pressed:
                        return ControlState.Pressed;
                    case Released:
                        return ControlState.Released;
                    case Held:
                        return ControlState.Held;
                    case None:
                        return ControlState.None;
                    default:
                        return ControlState.None;
                }
            }
        }
        public static Vector2 MousePos
        {
            get
            {
                return mousePos;
            }
        }
        public static Vector2 MouseVelocity
        {
            get
            {
                return mousePos - mousePosPrev;
            }
        }
        public static int ScrollWheel
        {
            get { return mouse.ScrollWheelValue; }
        }
        public bool jump = false;

        Keys[] keyArray = new Keys[0];
        Buttons[] buttonArray = new Buttons[0];
        String[] keyString = new String[0];
        String[] buttonString = new String[0];

        int[] stateArray;
        int[] stateArray2;
        int[] stateArray3;
        int[] stateArray4;
        int[] stateArray5;
        int[] buttonStateArray;
        int[] buttonStateArray2;
        int[] buttonStateArray3;
        int[] buttonStateArray4;
        int[] buttonStateArray5;

        float vibration = 0;
        float[] vibrationArray = new float[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        float[] vibrationSpeedArray = new float[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int vibrationIndex = 0;

        public int controlMode = 0;
        private static Controls instance;
        static Vector2 leftstick, rightstick;

        public static Vector2 LeftStick
        {
            get { return leftstick; }
        }
        public static Vector2 RightStick
        {
            get { return rightstick; }
        }

        //all of the keys and buttons
        public Controls()
        {
            instance = this;
            addKey(Keys.Z);
            addKey(Keys.X);
            addKey(Keys.Up);
            addKey(Keys.Down);
            addKey(Keys.Left);
            addKey(Keys.Right);
            addKey(Keys.A);
            addKey(Keys.S);
            addKey(Keys.D);
            addKey(Keys.F);
            addKey(Keys.C);
            addKey(Keys.V);
            addKey(Keys.B);
            addKey(Keys.Escape);
            addKey(Keys.W);
            addKey(Keys.Q);
            addKey(Keys.Q);
            addKey(Keys.E);
            addKey(Keys.R);
            addKey(Keys.Space);
            addKey(Keys.Enter);
            addKey(Keys.LeftShift);
            addKey(Keys.Delete);
            addKey(Keys.T);
            addKey(Keys.P);
            addKey(Keys.D1);
            addKey(Keys.D2);
            addKey(Keys.N);
            addKey(Keys.M);
            addKey(Keys.LeftControl);
            addButton(Buttons.A);
            addButton(Buttons.B);
            addButton(Buttons.X);
            addButton(Buttons.Y);
            addButton(Buttons.DPadUp);
            addButton(Buttons.DPadDown);
            addButton(Buttons.DPadLeft);
            addButton(Buttons.DPadRight);
            addButton(Buttons.RightShoulder);
            addButton(Buttons.LeftStick);
            addButton(Buttons.RightStick);
            addButton(Buttons.LeftShoulder);
            addButton(Buttons.LeftTrigger);
            addButton(Buttons.RightTrigger);
        }

        public void update()
        {
            //get state gamepad/mouse
            leftstick = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left; 
            rightstick = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right;
            mouseLB3 = 0;
            mouseLB2 = mouseLB;
            mouseRB3 = 0;
            mouseRB2 = mouseRB;
            //Console.WriteLine(getButton(Buttons.DPadRight));
            mouse = Mouse.GetState();
            mousePosPrev = mousePos;
            mousePos.X = mouse.X;
            mousePos.Y = mouse.Y;
            mousePos *= mouseMul / ScreenSize;
            if (mouse.LeftButton == ButtonState.Pressed)
            mouseLB = 1 ;
                else
            mouseLB = 0;
            if (mouse.RightButton == ButtonState.Pressed)
                mouseRB = 1;
            else
                mouseRB = 0;
            if (mouse.MiddleButton == ButtonState.Pressed)
                mouseCB = 1;
            else mouseCB = 0;

            vibration = 0;

            //vibration
            for (int i = 0; i < vibrationArray.Length; i++)
            {
                vibration += vibrationArray[i];
                vibrationArray[i] += (0-vibrationArray[i]) * vibrationSpeedArray[i];
                //if (vibrationArray[i] < 0)
                {
                   // vibrationArray[i] = 0;
                    //vibrationSpeedArray[i] = 0;
                }
            }
            //vibration *= vibrationArray.Length;
            if (vibration > 1)
                vibration = 1;
            if (vibration < 0)
                vibration = 0;
            //Console.WriteLine(vibration);
            GamePad.SetVibration(PlayerIndex.One, vibration, vibration);
            if (mouseLB == 1 && mouseLB2 == 1)
                mouseLB3 = 1;
            if (mouseLB == 1 && mouseLB2 == 0)
                mouseLB3 = 3;
            if (mouseLB == 0 && mouseLB2 == 1)
                mouseLB3 = 2;
            if (mouseLB == 0 && mouseLB2 == 0)
                mouseLB3 = 0;

            if (mouseRB == 1 && mouseRB2 == 1)
                mouseRB3 = 1;
            if (mouseRB == 1 && mouseRB2 == 0)
                mouseRB3 = 3;
            if (mouseRB == 0 && mouseRB2 == 1)
                mouseRB3 = 2;
            if (mouseRB == 0 && mouseRB2 == 0)
                mouseRB3 = 0;

            if (mouseCB == 1 && mouseCB2 == 1)
                mouseCB3 = 1;
            if (mouseCB == 1 && mouseCB2 == 0)
                mouseCB3 = 3;
            if (mouseCB == 0 && mouseCB2 == 1)
                mouseCB3 = 2;
            if (mouseCB == 0 && mouseCB2 == 0)
                mouseCB3 = 0;
            
            //get key states
            for (int i = 0; i < keyArray.Length; i += 1)
            {
                stateArray3[i] = 0;
                stateArray2[i] = stateArray[i];
                if (Keyboard.GetState().IsKeyDown(keyArray[i]))
                {
                    stateArray[i] = 1;
                }
                if (Keyboard.GetState().IsKeyUp(keyArray[i]))
                {
                    stateArray[i] = 0;
                }
                if (stateArray[i] == 1 && stateArray2[i] == 0)
                {
                    controlMode = 0;
                    stateArray3[i] = 3;
                }
                if (stateArray[i] == 0 && stateArray2[i] == 1)
                {
                    stateArray3[i] = 2;
                    stateArray5[i] = 0;
                }
                if (stateArray[i] == 0 && stateArray2[i] == 0)
                {
                    stateArray3[i] = 0;
                    stateArray4[i] = 0;
                    stateArray5[i]++;
                }
                if (stateArray[i] == 1 && stateArray2[i] == 1)
                {
                    stateArray3[i] = 1;
                    stateArray4[i] += 1;
                    stateArray5[i] = 0;
                }
            }

            //get button states
            for (int i = 0; i < buttonArray.Length; i += 1)
            {
                buttonStateArray3[i] = 0;
                buttonStateArray2[i] = buttonStateArray[i];
                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(buttonArray[i]))
                {
                    buttonStateArray[i] = 1;
                }
                if (GamePad.GetState(PlayerIndex.One).IsButtonUp(buttonArray[i]))
                {
                    buttonStateArray[i] = 0;
                }
                if (buttonStateArray[i] == 1 && buttonStateArray2[i] == 0)
                {
                    controlMode = 1;
                    //buttonStateArray5[i] = 0;
                    buttonStateArray3[i] = 3;
                }
                if (buttonStateArray[i] == 0 && buttonStateArray2[i] == 1)
                {
                    buttonStateArray5[i] = 0;
                    buttonStateArray3[i] = 2;
                }
                if (buttonStateArray[i] == 0 && buttonStateArray2[i] == 0)
                {
                    buttonStateArray3[i] = 0;
                    buttonStateArray4[i] = 0;
                    buttonStateArray5[i]++;
                }
                if (buttonStateArray[i] == 1 && buttonStateArray2[i] == 1)
                {
                    buttonStateArray3[i] = 1;
                    buttonStateArray4[i] += 1;
                    buttonStateArray5[i] = 0;
                }
            }

            
        }

        //vibrate controler
        public void vibrate(float intensity, float speed)
        {
            if (intensity < 0)
                intensity = 0;
            if (intensity > 1)
                intensity = 1;
            vibrationArray[vibrationIndex] = intensity;
            vibrationSpeedArray[vibrationIndex] = speed;
            vibrationIndex += 1;
            if (vibrationIndex >= vibrationArray.Length)
                vibrationIndex = 0;
        }

        //get particular key binding from string
        public int getKey(String s)
        {
            int ind = Array.IndexOf(keyString, s);
            if (controlMode == 1)
                return -1;
            try
            {
                return stateArray3[ind];
            }
            catch (IndexOutOfRangeException)
            {
                //key not in array
                return 0;
            }
        }

        //get key binding from key
        public int getKey(Keys k)
        {
            int ind = Array.IndexOf(keyString, k.ToString());
            if (controlMode == 1)
                return -1;
            try
            {
                return stateArray3[ind];
            }
            catch (IndexOutOfRangeException)
            {
                //key not in array
                addKey(k);
                return getKey(k);
            }
        }

        //get instance of key
        public static ControlState GetKey(Keys k)
        {
            return intToState(instance.getKey(k));
        }
        public static int GetKeyTime(Keys k)
        {
            return instance.getKeyTime(k.ToString());
        }
        public static int GetKeyReleaseTime(Keys k)
        {
            return instance.getKeyReleaseTime(k.ToString());
        }
        //get instance of button
        public static ControlState GetButton(Buttons b)
        {
            return intToState(instance.getButton(b));
        }
        
        public static int GetButtonTime(Buttons b)
        {
            return instance.getButtonTime(b.ToString());
        }
        public static int GetButtonReleaseTime(Buttons b)
        {
            return instance.getButtonReleaseTime(b.ToString());
        }
        //Changes int variable to a control state
        private static ControlState intToState(int i)
        {
            switch (i)
            {
                case 0:
                    return ControlState.None;
                    break;
                case 1:
                    return ControlState.Held;
                    break;
                case 2:
                    return ControlState.Released;
                    break;
                case 3: 
                    return ControlState.Pressed;
                    break;
                default:
                    return ControlState.None;
                    break;
            }
        }
        //get button binding from string
        public int getButton(String b)
        {
            if (controlMode == 0)
                return -1;
            int ind = Array.IndexOf(buttonString, b);
            try
            {
                return buttonStateArray3[ind];
            }
            catch (IndexOutOfRangeException)
            {
                //button not in array
                return 0;
            }
        }

        //get button binding from button
        public int getButton(Buttons b)
        {
            if (controlMode == 0)
                return -1;
            int ind = Array.IndexOf(buttonString, b.ToString());
            try
            {
                return buttonStateArray3[ind];
            }
            catch (IndexOutOfRangeException)
            {
                //button not in array
                addButton(b);
                return getButton(b);
            }
        }

        //get time key has been held?
        public int getKeyTime(String s)
        {
            int ind = Array.IndexOf(keyString, s);
            return stateArray4[ind];
        }
        public int getKeyReleaseTime(String s)
        {
            int ind = Array.IndexOf(keyString, s);
            return stateArray5[ind];
        }
        public int getButtonTime(String s)
        {
            int ind = Array.IndexOf(buttonString, s);
            return buttonStateArray4[ind];
        }
        public int getButtonReleaseTime(String s)
        {
            int ind = Array.IndexOf(buttonString, s);
            return buttonStateArray5[ind];
        }

        //add key to the end of keyArray
        private void addKey(Keys k)
        {
            Keys[] temp = new Keys[keyArray.Length + 1];

            //copy over old array to temp
            for (int i = 0; i < keyArray.Length; i += 1)
            {
                temp[i] = keyArray[i];
            }

            temp[keyArray.Length] = k;
            keyArray = temp;
            
            keyString = new String[keyArray.Length];

            //use new keyArray to replace the string array
            for (int i = 0; i < keyArray.Length; i += 1)
                keyString[i] = keyArray[i].ToString();

            //replace state arrays
            stateArray = new int[keyArray.Length];
            stateArray2 = new int[keyArray.Length];
            stateArray3 = new int[keyArray.Length];
            stateArray4 = new int[keyArray.Length];
            stateArray5 = new int[keyArray.Length];

            //reset stateArrays
            for (int i = 0; i < keyArray.Length; i += 1)
            {
                stateArray[i] = 0;
                stateArray2[i] = 0;
                stateArray3[i] = 0;
                stateArray4[i] = 0;
                stateArray4[i] = 0;
            }


        }

        //add button on the end of buttonArray
        private void addButton(Buttons b)
        {
            Buttons[] temp = new Buttons[buttonArray.Length + 1];

            //copy old array to temp
            for (int i = 0; i < buttonArray.Length; i += 1)
            {
                temp[i] = buttonArray[i];
            }

            temp[buttonArray.Length] = b;
            buttonArray = temp;

            buttonString = new String[buttonArray.Length];

            //make new string array from buttonArray
            for (int i = 0; i < buttonArray.Length; i += 1)
                buttonString[i] = buttonArray[i].ToString();

            //replace state arrays
            buttonStateArray = new int[buttonArray.Length];
            buttonStateArray2 = new int[buttonArray.Length];
            buttonStateArray3 = new int[buttonArray.Length];
            buttonStateArray4 = new int[buttonArray.Length];
            buttonStateArray5 = new int[buttonArray.Length];

            //reset state arrays
            for (int i = 0; i < buttonArray.Length; i += 1)
            {
                buttonStateArray[i] = 0;
                buttonStateArray2[i] = 0;
                buttonStateArray3[i] = 0;
                buttonStateArray4[i] = 0;
                buttonStateArray5[i] = 0;
            }


        }
        


    }
}
