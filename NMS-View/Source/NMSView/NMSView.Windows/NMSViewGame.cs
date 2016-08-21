using NMSView;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Graphics;
using SiliconStudio.Xenko.Rendering;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace NMSView
{
    public class NMSViewGame : Game
    {
        public static string ReqFname = null;

        public FreeCamera CamController;

        public delegate void ModelLoadedEventHandler(NMSModel Model, NMSEntity Entity, string Name);
        public event ModelLoadedEventHandler OnModelLoaded;

        protected override Task LoadContent()
        {
            EntityAddQueue = new ConcurrentQueue<Entity>();
            EntityRemoveQueue = new ConcurrentQueue<Entity>();

            // Make camera
            CamController = new FreeCamera();
            Script.Add(CamController);

            if (ReqFname != null)
            {
                // Load the model...
                var modelFname = ReqFname;
                LoadModel(modelFname);
            }

            Script.AddTask(UpdateFrame);

            return base.LoadContent();
        }

        public bool LoadModel(string FileName)
        {
            if (!File.Exists(FileName))
                return false;

            Console.WriteLine("Loading {0}...", Path.GetFileNameWithoutExtension(FileName));

            var model = NMSModel.Read(FileName);
            model.PrintStats();

            var Desc = RasterizerStateDescription.Default;
            Desc.CullMode = CullMode.None;

            var entity = CreateCustomMeshModel(
                GraphicsDevice,
                model.ToXenkoVertices(),
                model.Indices.ToArray(),
                PrimitiveType.TriangleList,
                Desc,
                Asset.Load<Material>("Sphere Material"),
                Vector3.Zero,
                Quaternion.Identity,
                false);

            EntityAddQueue.Enqueue(entity);

            Console.WriteLine("OK");

            if (OnModelLoaded != null)
            {
                OnModelLoaded(model, entity, FileName);
            }

            return true;
        }

        public void ClearNMSEntities()
        {
            var entities = SceneSystem.SceneInstance.Scene.Entities.Where(x => x is NMSEntity);

            foreach (var entity in entities)
                EntityRemoveQueue.Enqueue(entity);
        }

        public NMSEntity CreateCustomMeshModel(GraphicsDevice graphicsDevice, VertexPositionNormalTexture[] Vertexes, int[] Indices, PrimitiveType PrimType, RasterizerStateDescription RasterizerDesc, Material Mat, Vector3 SpawnPos, Quaternion SpawnRot, bool SpawnNow = true)
        {
            var meshMesh = new Mesh();
            var meshDraw = new MeshDraw();
            var meshMod = new Model();
            var meshComp = new ModelComponent();

            var buffIdx = SiliconStudio.Xenko.Graphics.Buffer.Index.New(graphicsDevice, Indices);
            var buffVert = SiliconStudio.Xenko.Graphics.Buffer.Vertex.New(graphicsDevice, Vertexes);

            var RasterizerStateDesc = RasterizerDesc;

            meshDraw.IndexBuffer = new IndexBufferBinding(buffIdx, true, Indices.Length);
            meshDraw.VertexBuffers = new VertexBufferBinding[] { new VertexBufferBinding(buffVert, VertexPositionNormalTexture.Layout, Vertexes.Length) };
            meshDraw.PrimitiveType = PrimType;
            meshDraw.DrawCount = Indices.Length;

            meshMesh.Draw = meshDraw;
            meshMesh.Parameters.Set(Effect.RasterizerStateKey, RasterizerState.New(graphicsDevice, RasterizerStateDesc));
            meshMesh.MaterialIndex = 0;

            var mat = Mat;

            Vector3 min = new Vector3(float.MaxValue), max = new Vector3(float.MinValue);
            foreach (var vert in Vertexes)
            {
                if (vert.Position.X < min.X)
                    min.X = vert.Position.X;
                if (vert.Position.Y < min.Y)
                    min.Y = vert.Position.Y;
                if (vert.Position.Z < min.Z)
                    min.Z = vert.Position.Z;

                if (vert.Position.X > max.X)
                    max.X = vert.Position.X;
                if (vert.Position.Y > max.Y)
                    max.Y = vert.Position.Y;
                if (vert.Position.Z > max.Z)
                    max.Z = vert.Position.Z;
            }
            meshMod.BoundingBox = new BoundingBox(min, max);

            meshMod.Materials.Add(mat);

            meshMod.Meshes.Add(meshMesh);
            meshComp.Model = meshMod;

            var outEntity = new NMSEntity();
            outEntity.Transform.Position = SpawnPos;
            outEntity.Transform.Rotation = SpawnRot;
            outEntity.Add(meshComp);
            outEntity.Group = EntityGroup.Group29;

            outEntity.Transform.UpdateLocalMatrix();
            meshMesh.Parameters.Set(TransformationKeys.World, outEntity.Transform.LocalMatrix);

            if (SpawnNow)
            {
                SceneSystem.SceneInstance.Scene.Entities.Add(outEntity);
            }

            return outEntity;
        }

        private async Task UpdateFrame()
        {
            while (IsRunning)
            {
                await Script.NextFrame();
                ProcessQueues();
            }
        }

        public ConcurrentQueue<Entity> EntityAddQueue;
        public ConcurrentQueue<Entity> EntityRemoveQueue;
        private void ProcessQueues()
        {
            while (!EntityRemoveQueue.IsEmpty)
            {
                Entity ent;
                if (EntityRemoveQueue.TryDequeue(out ent))
                {
                    SceneSystem.SceneInstance.Scene.Entities.Remove(ent);
                }
                else
                    break;
            }

            while (!EntityAddQueue.IsEmpty)
            {
                Entity ent;
                if (EntityAddQueue.TryDequeue(out ent))
                {
                    if (ent.IsDisposed)
                        continue;

                    SceneSystem.SceneInstance.Scene.Entities.Add(ent);
                }
                else
                    break;
            }
        }
    }
}
