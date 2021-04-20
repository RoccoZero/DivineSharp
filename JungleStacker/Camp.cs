using Divine;
using Divine.SDK.Extensions;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JungleStacker
{
    internal sealed class Camp
    {
        public string Name = "";
        public bool IsActive;
        public Unit Unit = null;
        public Vector3 Position = Vector3.Zero;
        public float StackTime = 55f;
        public float TimeOffset = 0f;
        public Vector3 WaitPosition = Vector3.Zero;
        public Vector3 StackPosition = Vector3.Zero;
        public Vector2 MousePosition = Vector2.Zero;
        public RectangleF mainRect = RectangleF.Empty;
        public RectangleF imgBorderRect = RectangleF.Empty;
        public Color backgroundColor;
        public Color borderColor;
        public Color textColor;
        public int backgroundAlpha = 105;
        public int mainAlpha = 255;
        public bool IsKeyDown;
        public StackState StackState = StackState.WaitPosition;

        public Camp()
        {
            backgroundColor = new Color(0, 0, 0, backgroundAlpha);
            borderColor = new Color(255, 0, 0, mainAlpha);
            textColor = new Color(255, 255, 255, mainAlpha);
            CreateCallbacks();
        }

        private void CreateCallbacks()
        {
            InputManager.MouseKeyDown += InputManager_MouseKeyDown;
            InputManager.MouseKeyUp += InputManager_MouseKeyUp;
            InputManager.MouseMove += InputManager_MouseMove;
            InputManager.KeyDown += InputManager_KeyDown;
            InputManager.KeyUp += InputManager_KeyUp;
        }

        private void InputManager_KeyDown(KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.LeftCtrl)
            {
                return;
            }

            IsKeyDown = true;
        }

        private void InputManager_KeyUp(KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.LeftCtrl)
            {
                return;
            }

            IsKeyDown = false;
        }

        private void InputManager_MouseMove(MouseMoveEventArgs e)
        {
            MousePosition = e.Position;
        }

        private void InputManager_MouseKeyDown(MouseEventArgs e)
        {
            if (e.MouseKey != MouseKey.Left)
            {
                return;
            }
        }

        private void InputManager_MouseKeyUp(MouseEventArgs e)
        {
            if (e.MouseKey != MouseKey.Left)
            {
                return;
            }


            if (MousePosition.IsUnderRectangle(imgBorderRect))
            {
                e.Process = false;
                if (!IsKeyDown)
                {
                    //if (IsActive)
                    //{
                    Unit = (Unit)EntityManager.LocalPlayer.SelectedUnits.Where<Unit>(x => x.IsControllable && x.Handle != Unit?.Handle).FirstOrDefault();
                    if (Unit != null)
                    {
                        StackerOrderManager.LastOrders[Unit] = (OrderType.None, (Vector3.Zero, null));
                    }
                    //};
                }
                else
                {
                    StackerOrderManager.LastOrders[Unit] = (OrderType.None, (Vector3.Zero, null));
                    Unit = null;
                }
            }

            if (MousePosition.IsUnderRectangle(mainRect) && e.Process)
            {
                e.Process = false;
                IsActive = !IsActive;
                borderColor = IsActive ? new Color(0, 255, 0, mainAlpha) : new Color(255, 0, 0, mainAlpha);
            }
        }

        public void DrawInfo()
        {
            if (Unit != null && (!Unit.IsValid || !Unit.IsAlive))
            {
                Unit = null;
            }
            var screenPos = RendererManager.WorldToScreen(MapManager.GetAbsolutePosition(Position), true);
            if (screenPos.IsZero)
            {
                return;
            }

            var textSize = RendererManager.MeasureText($"Camp Name\n{Name}\nCamp Unit\n" /*+ "{Unit?.Name ?? "None"}"*/, 20);
            mainRect = new RectangleF(screenPos.X - ((textSize.X * 1.1f) * 0.5f), screenPos.Y - (textSize.Y * 0.5f), textSize.X * 1.1f, textSize.Y);
            if (MousePosition.IsUnderRectangle(mainRect))
            {
                //Anim();



                RendererManager.DrawFilledRectangle(mainRect, backgroundColor);
                RendererManager.DrawRectangle(mainRect, borderColor);

                var textRect = new RectangleF(mainRect.X, mainRect.Y, mainRect.Width, textSize.Y);
                RendererManager.DrawText($"Camp Name\n{Name}\nCamp Unit\n" /*+ "{Unit?.Name ?? "None"}"*/, textRect, textColor, FontFlags.Center, 20);

                imgBorderRect = new RectangleF(mainRect.X + (mainRect.Width * 0.5f) - (mainRect.Height * 0.1f) * (16f / 9f), mainRect.Y + mainRect.Height - (mainRect.Height * 0.23f), (mainRect.Height * 0.2f) * (16f / 9f), mainRect.Height * 0.2f);

                if (Unit == null)
                {
                    var imgRect = new RectangleF(mainRect.X + (mainRect.Width * 0.5f) - (mainRect.Height * 0.1f), mainRect.Y + mainRect.Height - (mainRect.Height * 0.23f), mainRect.Height * 0.2f, mainRect.Height * 0.2f);
                    RendererManager.DrawTexture("panorama/images/control_icons/question_mark_png.vtex_c", imgRect, TextureType.Default, true);
                }
                else
                {
                    RendererManager.DrawTexture("panorama/images/heroes/" + Unit.Name + "_png.vtex_c", imgBorderRect, TextureType.Default, true);
                }
                RendererManager.DrawFilledRectangle(imgBorderRect, new Color(0, 0, 0, 105));
                RendererManager.DrawRectangle(imgBorderRect, Color.Black);



                //RendererManager.DrawText($"Camp Name: {Name}\nCamp Unit: {Unit?.Name ?? "None"}", screenPos, Color.White, 20);
            }


        }

        private void Anim()
        {
            if (MousePosition.IsUnderRectangle(mainRect))
            {
                if (mainAlpha < 255)
                {
                    mainAlpha += 15;
                    borderColor.A = (byte)mainAlpha;
                    textColor.A = (byte)mainAlpha;
                }
                if (backgroundAlpha < 105)
                {
                    backgroundAlpha += 15;
                    backgroundColor.A = (byte)backgroundAlpha;
                }
            }
            else
            {
                if (mainAlpha > 0)
                {
                    mainAlpha -= 15;
                    borderColor.A = (byte)mainAlpha;
                    textColor.A = (byte)mainAlpha;
                }
                if (mainAlpha <= backgroundAlpha && backgroundAlpha > 0)
                {
                    backgroundAlpha -= 15;
                    backgroundColor.A = (byte)backgroundAlpha;
                }
            }
        }



    }
}
