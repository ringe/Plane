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

namespace Plane
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private BasicEffect effect;

        //WVP-matrisene:
        private Matrix world;
        private Matrix projection;
        private Matrix view;

        //Kameraposisjon:
        private Vector3 cameraPosition = new Vector3(5f, 6f, 5.0f); 
        private Vector3 cameraTarget = Vector3.Zero;
        private Vector3 cameraUpVector = new Vector3(0.0f, 1.0f, 0.0f);


        private SpriteBatch spriteBatch;
        //private SpriteFont spriteFont;

        // Prepare vertices
        VertexPositionColor[] cubeVertices;
        VertexPositionColor[] planeVertices;
        VertexPositionColor[] propVertices;
        VertexPositionColor[] xAxis = new VertexPositionColor[2];
        VertexPositionColor[] yAxis = new VertexPositionColor[2];
        VertexPositionColor[] zAxis = new VertexPositionColor[2];

        // Rotation
        float propRot = 0.0f;
        float speedAngle;

        // Movement
        float BOUNDARY = 5f;
        Vector3 movement = new Vector3(0.5f, 0, -1.5f);
        Vector3 position = new Vector3(0, 0, 0);

        // Matrixstack
        private Stack<Matrix> plane = new Stack<Matrix>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            InitDevice();
            InitCamera();
            InitVertices();
        }

        /// <summary>
        /// Initialize the graphics device.
        /// </summary>
        private void InitDevice()
        {

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            Window.Title = "Flight!";

            effect = new BasicEffect(GraphicsDevice);

            effect.VertexColorEnabled = true;
        }

        /// <summary>
        /// Position the camera.
        /// </summary>
        private void InitCamera()
        {
            //Projeksjon:
            float aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Height;

            //Oppretter view-matrisa:
            Matrix.CreateLookAt(ref cameraPosition, ref cameraTarget, ref cameraUpVector, out view);

            //Oppretter projeksjonsmatrisa:
            Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 0.01f, 1000.0f, out projection);

            //Gir matrisene til shader:
            effect.Projection = projection;
            effect.View = view;
        }

        /// <summary>
        /// Prepare the object vertices
        /// </summary>
        protected void InitVertices()
        {
            // Initialize a Cube
            cubeVertices = new VertexPositionColor[17]
            {
                new VertexPositionColor(new Vector3(-1,  1,  1), Color.Red),
                new VertexPositionColor(new Vector3( 1,  1,  1), Color.Blue),
                new VertexPositionColor(new Vector3(-1, -1,  1), Color.Yellow),
                new VertexPositionColor(new Vector3(1, -1,  1), Color.Orange),
                new VertexPositionColor(new Vector3(-1, -1, -1), Color.Blue),
                new VertexPositionColor(new Vector3(1, -1, -1), Color.Green),
                new VertexPositionColor(new Vector3(-1,  1, -1), Color.Yellow),
                new VertexPositionColor(new Vector3(1,  1, -1), Color.Red),
                new VertexPositionColor(new Vector3(-1,  1,  1), Color.Yellow),
                new VertexPositionColor(new Vector3(1,  1,  1), Color.Green),
                new VertexPositionColor(new Vector3(1, -1,  1), Color.Yellow),
                new VertexPositionColor(new Vector3(1,  1, -1), Color.Blue),
                new VertexPositionColor(new Vector3(1, -1, -1), Color.Orange),
                new VertexPositionColor(new Vector3(-1, -1, -1), Color.Red),
                new VertexPositionColor(new Vector3(-1,  1, -1), Color.Pink),
                new VertexPositionColor(new Vector3(-1, -1,  1), Color.Green),
                new VertexPositionColor(new Vector3(-1,  1,  1), Color.Yellow)
            };
                
            // Initialize a "plane"
            planeVertices = new VertexPositionColor[7]
            {
                new VertexPositionColor(new Vector3(-2,     0.7f,   -1),    Color.Yellow),
                new VertexPositionColor(new Vector3(-0.5f,  0.7f,   -1), Color.Yellow),
                new VertexPositionColor(new Vector3(0,      0.7f,    3),    Color.Red),
                new VertexPositionColor(new Vector3(0,      0,       -0.2f),    Color.Yellow),
                new VertexPositionColor(new Vector3(0.5f,     0.7f,    -1), Color.Yellow),
                new VertexPositionColor(new Vector3(0,      0.7f,    3),    Color.Blue),
                new VertexPositionColor(new Vector3(2,     0.7f,    -1),    Color.Yellow)
            };
            
            // Initialize a propeller
            propVertices = new VertexPositionColor[4]
            {
                new VertexPositionColor(new Vector3(-0.6f, -0.1f, -0.2f), Color.Blue),
                new VertexPositionColor(new Vector3(-0.6f,  0.1f, -0.2f), Color.Blue),
                new VertexPositionColor(new Vector3( 0.6f, -0.1f, -0.2f), Color.Blue),
                new VertexPositionColor(new Vector3( 0.6f,  0.1f, -0.2f), Color.Blue)
            };

            // Set axis lines
            xAxis[0] = new VertexPositionColor(new Vector3(-100.0f, 0f, 0f), Color.Red);
            xAxis[1] = new VertexPositionColor(new Vector3(100.0f, 0f, 0f), Color.Red);
            yAxis[0] = new VertexPositionColor(new Vector3(0f, -100.0f, 0f), Color.White);
            yAxis[1] = new VertexPositionColor(new Vector3(0f, 100.0f, 0f), Color.White);
            zAxis[0] = new VertexPositionColor(new Vector3(0f, 0f, -100.0f), Color.Black);
            zAxis[1] = new VertexPositionColor(new Vector3(0f, 0f, 100.0f), Color.Black);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            GetRotationAngle();
            HandleKeyboardInput();
            UpdatePosition();

            base.Update(gameTime);
        }

        /// <summary>
        /// Update the current position by the current movement
        /// </summary>
        private void UpdatePosition()
        {
            if (position.X > BOUNDARY || position.X < -BOUNDARY)
                movement.X *= -1;
            if (position.Z > BOUNDARY || position.Z < -BOUNDARY)
                movement.Z *= -1;

            position += movement * (float)TargetElapsedTime.Milliseconds / 1000f;
        }

        /// <summary>
        /// React to key press
        /// </summary>
        private void HandleKeyboardInput()
        {
            KeyboardState keys = Keyboard.GetState();

            // Determine change in direction
            if (keys.IsKeyDown(Keys.Left))
                SetSpeed(false);
            else if (keys.IsKeyDown(Keys.Right))
                SetSpeed(true);

            // Determine change in speed
            if (keys.IsKeyDown(Keys.Up))
                movement = movement + movement * 0.02f;
            else if (keys.IsKeyDown(Keys.Down))
                movement = movement - movement * 0.02f;

            // Determine change in direction up/down
            //if (keys.IsKeyDown(Keys.W))
            //    SetAngle(false);
            //else if (keys.IsKeyDown(Keys.S))
            //    SetAngle(true);

            // Exit
            if (keys.IsKeyDown(Keys.Escape))
                this.Exit();
        }

        /// <summary>
        /// Set the speed vector
        /// </summary>
        private void SetSpeed(bool right)
        {
            float x, z, angle;
            float r = movement.Length();

            if (right)
                angle = speedAngle - (float)Math.PI / 100.0f;
            else
                angle = speedAngle + (float)Math.PI / 100.0f;

            x = 0.0f + r * (float)Math.Sin(angle);
            z = 0.0f + r * (float)Math.Cos(angle);

            movement.X = x;
            movement.Z = z;
        }

        /// <summary>
        /// Get the rotation angle (turning angle)
        /// </summary>
        private float GetRotationAngle()
        {
            float fRotY;

            fRotY = (float)Math.Atan2(movement.X, movement.Z);
            speedAngle = fRotY;

            return fRotY - (float)Math.PI / 2;
        }

        private void DrawPlane()
        {
            Matrix scale, rotatY, move;
            
            Matrix.CreateScale(0.5f, 0.5f, 0.5f, out scale);

            // Set rotation according to the movement angle
            rotatY = Matrix.CreateRotationY(speedAngle);

            // Set translation to current position
            move = Matrix.CreateTranslation(position);

            plane.Push(scale * rotatY * move);

            world = Matrix.Identity * plane.Peek();

            effect.World = world;

            //Starter tegning
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, planeVertices, 0, 5, VertexPositionColor.VertexDeclaration);
            }
            
        }

        /// <summary>
        /// Draw the propeller at the back of the plane
        /// </summary>
        /// <param name="speed">How fast to spin the propeller</param>
        private void DrawPropeller()
        {
            Matrix rotZ;
            
            // Rotasjon
            rotZ = Matrix.CreateRotationZ(propRot);
            propRot += (float)TargetElapsedTime.Milliseconds / 100 * Math.Abs(movement.Length());
            propRot = propRot % (float)(2 * Math.PI);

            world = Matrix.Identity * rotZ * plane.Pop();

            effect.World = world;

            //Starter tegning
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, propVertices, 0, 2, VertexPositionColor.VertexDeclaration);
            }
        }

        /// <summary>
        /// Draw the axis lines.
        /// </summary>
        private void DrawAxis()
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, xAxis, 0, 1);
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, yAxis, 0, 1);
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, zAxis, 0, 1);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            RasterizerState rasterizerState1 = new RasterizerState();
            rasterizerState1.CullMode = CullMode.None;
            rasterizerState1.FillMode = FillMode.Solid;
            GraphicsDevice.RasterizerState = rasterizerState1;

            GraphicsDevice.Clear(Color.DeepSkyBlue);

            //Setter world=I:
            world = Matrix.Identity;

            // Setter world-matrisa på effect-objektet (verteks-shaderen):
            effect.World = world;

            DrawAxis();                 // Draw the axis lines
            DrawPlane();                // Draw the plane
            DrawPropeller();            // Draw the propeller

            base.Draw(gameTime);
        }
    }
}
