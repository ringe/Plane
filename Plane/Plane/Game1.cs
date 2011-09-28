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
        float rotY = 0.0f;

        // Movement
        Vector3 speed = new Vector3(3, 1, 0);
        Vector3 position = new Vector3(0, 0, 0);

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
                new VertexPositionColor(new Vector3(-1,  0.7f,  -2), Color.Yellow),
                new VertexPositionColor(new Vector3(-1,  0.7f,  -0.5f), Color.Yellow),
                new VertexPositionColor(new Vector3(3,  0,  0), Color.Yellow),
                new VertexPositionColor(new Vector3(-0.2f,  0,  0), Color.Yellow),
                new VertexPositionColor(new Vector3(-1,  0.7f,  0.5f), Color.Yellow),
                new VertexPositionColor(new Vector3(3,  0,  0), Color.Yellow),
                new VertexPositionColor(new Vector3(-1,  0.7f,  2), Color.Yellow)
            };
            
            // Initialize a propeller
            propVertices = new VertexPositionColor[4]
            {
                new VertexPositionColor(new Vector3(-0.2f, -0.1f, -0.6f), Color.Blue),
                new VertexPositionColor(new Vector3(-0.2f, 0.1f, -0.6f), Color.Blue),
                new VertexPositionColor(new Vector3(-0.2f,-0.1f, 0.6f), Color.Blue),
                new VertexPositionColor(new Vector3(-0.2f, 0.1f, 0.6f), Color.Blue)
            };

            // Set axis lines
            xAxis[0] = new VertexPositionColor(new Vector3(-100.0f, 0f, 0f), Color.Yellow);
            xAxis[1] = new VertexPositionColor(new Vector3(100.0f, 0f, 0f), Color.Yellow);
            yAxis[0] = new VertexPositionColor(new Vector3(0f, -100.0f, 0f), Color.Green);
            yAxis[1] = new VertexPositionColor(new Vector3(0f, 100.0f, 0f), Color.Green);
            zAxis[0] = new VertexPositionColor(new Vector3(0f, 0f, -100.0f), Color.Pink);
            zAxis[1] = new VertexPositionColor(new Vector3(0f, 0f, 100.0f), Color.Pink);
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

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private void DrawPlane()
        {
            Matrix scale, rotatY, move;
            
            Matrix.CreateScale(0.5f, 0.5f, 0.5f, out scale);

            // Rotasjon om egen akse
            rotatY = Matrix.CreateRotationY(rotY);
            rotY += (float)TargetElapsedTime.Milliseconds / -1000f;
            rotY = rotY % (float)(2 * Math.PI);

            //position += speed / 100;
            move = Matrix.CreateTranslation(position);

            world = Matrix.Identity * scale * rotatY * move;

            // Draw earth
            effect.World = world;

            this.DrawPa();
        }


        private void DrawPa()
        {
            //Starter tegning - må bruke effect-objektet:
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, planeVertices, 0, 12);        
                //GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, cubeVertices, 0, 15, VertexPositionColor.VertexDeclaration);
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, planeVertices, 0, 5, VertexPositionColor.VertexDeclaration);
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
            rasterizerState1.FillMode = FillMode.WireFrame;
            GraphicsDevice.RasterizerState = rasterizerState1;

            GraphicsDevice.Clear(Color.DeepSkyBlue);

            //Setter world=I:
            world = Matrix.Identity;

            // Setter world-matrisa på effect-objektet (verteks-shaderen):
            effect.World = world;

            DrawAxis();                 // Draw the axis lines
            DrawPlane();                // Draw the plane

            base.Draw(gameTime);
        }
    }
}
