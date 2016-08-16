using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Extensions;
using SiliconStudio.Xenko.Graphics;
using SiliconStudio.Xenko.Input;
using NMSView;

namespace NMSView
{
    // old and kinda shit but it'll do ¯\_(ツ)_/¯
    public class FreeCamera : AsyncScript
    {
        public Entity Camera;
        public CameraComponent CamComponent;

        private int SpeedMode = 1;
        private Vector2 MouseStartPos = Vector2.Zero;
        private Quaternion StartRot = Quaternion.Zero;
        private Vector2 DeltaRot = Vector2.Zero;
        public bool MasterEnable = true;

        public static Vector2 DebugRot;

        public override async Task Execute()
        {
            var game = (NMSViewGame)Game;
            var camera = game.SceneSystem.SceneInstance.Scene.Entities.Where(x => x.Name == "Camera").First().Get<CameraComponent>();
            CamComponent = camera;

            camera.FarClipPlane = 99999;

            while (MasterEnable)
            {
                await Script.NextFrame();

                var Input = game.Input;

                if (Input.IsKeyPressed(Keys.D1))
                    SpeedMode = 1;
                if (Input.IsKeyPressed(Keys.D2))
                    SpeedMode = 2;
                if (Input.IsKeyPressed(Keys.D3))
                    SpeedMode = 3;
                if (Input.IsKeyPressed(Keys.D4))
                    SpeedMode = 4;
                if (Input.IsKeyPressed(Keys.D5))
                    SpeedMode = 6;
                if (Input.IsKeyPressed(Keys.D6))
                    SpeedMode = 8;
                if (Input.IsKeyPressed(Keys.D7))
                    SpeedMode = 10;
                if (Input.IsKeyPressed(Keys.D8))
                    SpeedMode = 30;
                if (Input.IsKeyPressed(Keys.D9))
                    SpeedMode = 100;
                if (Input.IsKeyPressed(Keys.D0))
                    SpeedMode = 200;

                if (Input.IsKeyDown(Keys.O))
                    camera.VerticalFieldOfView += 0.3f;
                if (Input.IsKeyDown(Keys.P))
                    camera.VerticalFieldOfView -= 0.3f;


                float speed = game.Input.IsKeyDown(Keys.LeftShift) ? 0.1f : 0.005f; // SpeedMode * 5;

                Vector3 localTranslation = new Vector3();
                if (Input.IsKeyDown(Keys.W))
                    localTranslation.Z -= speed * SpeedMode;
                if (Input.IsKeyDown(Keys.A))
                    localTranslation.X -= speed * SpeedMode;
                if (Input.IsKeyDown(Keys.S))
                    localTranslation.Z += speed * SpeedMode;
                if (Input.IsKeyDown(Keys.D))
                    localTranslation.X += speed * SpeedMode;

                TransformComponent transformation = camera.Entity.Transform;
                Matrix view, projection;
                camera.Update();
                projection = camera.ProjectionMatrix;
                view = camera.ViewMatrix;
                view.Invert();
                Vector3 globalTranslation = Vector3.TransformNormal(localTranslation, view);
                transformation.Position += globalTranslation;

                if (Input.IsMouseButtonPressed(MouseButton.Right))
                {
                    MouseStartPos = Input.MousePosition - new Vector2(.5f, .5f);
                    StartRot = camera.Entity.Transform.Rotation;
                }

                if (Input.IsMouseButtonDown(MouseButton.Right))
                {
                    Vector2 MouseFromMiddle = Input.MousePosition - new Vector2(.5f, .5f);
                    Vector2 RelMousePos = MouseFromMiddle - MouseStartPos;

                    float Sensitivity = 3.5f;

                    DeltaRot += RelMousePos;

                    Quaternion localRotation = Quaternion.RotationX(-DeltaRot.Y * Sensitivity) * Quaternion.RotationY(-DeltaRot.X * Sensitivity);
                    transformation.Rotation = localRotation; 

                    MouseStartPos = MouseFromMiddle;
                }

                DebugRot = Input.MouseDelta;

                if (Input.IsMouseButtonReleased(MouseButton.Left))
                {
                    StartRot = camera.Entity.Transform.Rotation;
                }
            }
        }

        public void ResetCam()
        {
            if (CamComponent == null)
                return;

            CamComponent.Entity.Transform.Position = Vector3.Zero;
            CamComponent.Entity.Transform.Rotation = Quaternion.Identity;
        }
    }
}
